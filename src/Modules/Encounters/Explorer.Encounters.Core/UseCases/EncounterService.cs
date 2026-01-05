using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using Explorer.Encounters.Core.Utils;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IInternalPersonService _personService;
        private readonly IInternalPositionService _positionService;

        private const double NearbyRangeMeters = 1000.0; // 1km

        public EncounterService(
            IEncounterRepository encounterRepository,
            IInternalPersonService personService,
            IInternalPositionService positionService)
        {
            _encounterRepository = encounterRepository;
            _personService = personService;
            _positionService = positionService; // temp naming
        }

        public List<EncounterDto> GetAllEncounters()
        {
            var encounters = _encounterRepository.GetAll();
            return encounters.Select(MapToDto).ToList();
        }

        public EncounterDto GetEncounterById(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            return MapToDto(encounter);
        }

        public List<EncounterDto> GetNearbyEncounters(long personId)
        {
            var person = _personService.GetByUserId(personId);
            if (person == null)
                throw new KeyNotFoundException($"Person with ID {personId} not found.");

            var position = _positionService.GetByTouristId((int)person.UserId);
            if (position == null)
                throw new InvalidOperationException($"Position not found for user {person.UserId}.");

            var allEncounters = _encounterRepository.GetAll()
                .Where(e => e.Status == EncouterStatus.Published && e.Latitude.HasValue && e.Longitude.HasValue)
                .ToList();

            var nearbyEncounters = allEncounters
                .Where(e => DistanceCalculator.IsWithinRange(
                    position.Latitude, position.Longitude,
                    e.Latitude.Value, e.Longitude.Value,
                    NearbyRangeMeters))
                .Select(MapToDto)
                .ToList();

            return nearbyEncounters;
        }

        public EncounterDto CreateEncounter(EncounterDto createDto, bool skipLevelCheck = false)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            var creatorId = createDto.CreatorPersonId;

            // If creator exists and not skipping check, validate level
            if (creatorId != 0 && !skipLevelCheck)
            {
                var person = _personService.GetByPersonId(creatorId);
                if (person == null) throw new KeyNotFoundException("Creator person not found");
                if (person.Level < 10) throw new UnauthorizedAccessException("You must be level 10 to create an encounter.");
            }

            var encounter = MapToDomain(createDto);

            // If creator is not admin (skipLevelCheck == false) and not set to published, mark Pending
            if (!skipLevelCheck)
            {
                // keep Draft if caller set Draft explicitly, otherwise set to Pending
                if (encounter.Status == EncouterStatus.Draft)
                {
                    // set to Pending for non-admin creators
                    encounter.Status = EncouterStatus.Pending;
                }
            }

            encounter.CreatorPersonId = creatorId != 0 ? creatorId : (long?)null;

            var created = _encounterRepository.Create(encounter);
            return MapToDto(created);
        }

        public EncounterDto UpdateEncounter(long id, EncounterUpdateDto updateDto)
        {
            var existing = _encounterRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            UpdateDomain(existing, updateDto);
            var updated = _encounterRepository.Update(existing);
            return MapToDto(updated);
        }

        public void DeleteEncounter(long id)
        {
            var existingEncounter = _encounterRepository.GetById(id);
            if (existingEncounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            _encounterRepository.Delete(id);
        }

        public EncounterDto PublishEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null) throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Publish();
            var updated = _encounterRepository.Update(encounter);
            return MapToDto(updated);
        }

        public EncounterDto ArchiveEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null) throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Archive();
            var updated = _encounterRepository.Update(encounter);
            return MapToDto(updated);
        }

        public EncounterDto ReactivateEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null) throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Reactivate();
            var updated = _encounterRepository.Update(encounter);
            return MapToDto(updated);
        }

        public EncounterDto ApproveEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null) throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            if (encounter.Status != EncouterStatus.Pending)
                throw new InvalidOperationException("Only pending encounters can be approved.");

            encounter.Publish();
            var updated = _encounterRepository.Update(encounter);
            return MapToDto(updated);
        }

        private EncounterDto MapToDto(Encounter encounter)
        {
            return new EncounterDto
            {
                Id = encounter.Id,
                Name = encounter.Name,
                Description = encounter.Description,
                Location = encounter.Location,
                Latitude = encounter.Latitude ?? 0,
                Longitude = encounter.Longitude ?? 0,
                Status = encounter.Status.ToString(),
                Type = encounter.Type.ToString(),
                XPReward = encounter.XPReward,
                PublishedAt = encounter.PublishedAt,
                ArchivedAt = encounter.ArchivedAt,
                CreatorPersonId = encounter.CreatorPersonId ?? 0,
                SocialRequiredCount = encounter.SocialRequiredCount,
                SocialRangeMeters = encounter.SocialRangeMeters
            };
        }

        private Encounter MapToDomain(EncounterDto dto)
        {
            var enc = new Encounter(
                dto.Name,
                dto.Description,
                dto.Location,
                dto.Latitude,
                dto.Longitude,
                Enum.Parse<EncouterType>(dto.Type),
                dto.XPReward,
                dto.SocialRequiredCount,
                dto.SocialRangeMeters
            );

            // attempt to set status from dto if provided
            if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<EncouterStatus>(dto.Status, out var status))
            {
                enc.Status = status;
            }

            return enc;
        }

        private void UpdateDomain(Encounter existing, EncounterUpdateDto dto)
        {
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Location = dto.Location;

            existing.SetCoordinates(dto.Latitude, dto.Longitude);

            existing.XPReward = dto.XPReward;
            existing.Type = Enum.Parse<EncouterType>(dto.Type);
            if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = Enum.Parse<EncouterStatus>(dto.Status);
        }
    }
}