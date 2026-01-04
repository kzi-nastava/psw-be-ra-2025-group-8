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
            _positionService = positionService;
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
                return null;

            return MapToDto(encounter);
        }

        public List<EncounterDto> GetNearbyEncounters(long personId)
        {
            var person = _personService.GetByUserId(personId);
            if (person == null)
                throw new KeyNotFoundException("Person not found");

            var position = _positionService.GetByTouristId((int)person.UserId);
            if (position == null)
                throw new InvalidOperationException("Position not found for person");

            var allEncounters = _encounterRepository.GetAll()
                .Where(e => e.Status == EncouterStatus.Published && e.Latitude.HasValue && e.Longitude.HasValue)
                .ToList();

            var nearby = allEncounters
                .Where(e => DistanceCalculator.IsWithinRange(position.Latitude, position.Longitude, e.Latitude.Value, e.Longitude.Value, NearbyRangeMeters))
                .Select(MapToDto)
                .ToList();

            return nearby;
        }

        public EncounterDto CreateEncounter(EncounterDto createDto)
        {
            if (createDto.CreatorPersonId == 0)
                throw new ArgumentException("CreatorPersonId must be provided");

            var person = _personService.GetByUserId(createDto.CreatorPersonId);
            if (person == null)
                throw new KeyNotFoundException("Creator person not found");

            if (person.Level < 10)
                throw new UnauthorizedAccessException("You must be level 10 to create an encounter.");

            var encounter = MapToDomain(createDto);
            encounter.CreatorPersonId = createDto.CreatorPersonId;

            var created = _encounterRepository.Create(encounter);
            return MapToDto(created);
        }

        public EncounterDto UpdateEncounter(long id, EncounterUpdateDto updateDto)
        {
            var existing = _encounterRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("Encounter not found");

            UpdateDomain(existing, updateDto);
            var updated = _encounterRepository.Update(existing);
            return MapToDto(updated);
        }

        public void DeleteEncounter(long id)
        {
            _encounterRepository.Delete(id);
        }

        public EncounterDto PublishEncounter(long id)
        {
            var existing = _encounterRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("Encounter not found");
            existing.Publish();
            var updated = _encounterRepository.Update(existing);
            return MapToDto(updated);
        }

        public EncounterDto ArchiveEncounter(long id)
        {
            var existing = _encounterRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("Encounter not found");
            existing.Archive();
            var updated = _encounterRepository.Update(existing);
            return MapToDto(updated);
        }

        public EncounterDto ReactivateEncounter(long id)
        {
            var existing = _encounterRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("Encounter not found");
            existing.Reactivate();
            var updated = _encounterRepository.Update(existing);
            return MapToDto(updated);
        }

        private EncounterDto MapToDto(Encounter e)
        {
            return new EncounterDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Location = e.Location,
                Latitude = e.Latitude ?? 0,
                Longitude = e.Longitude ?? 0,
                Status = e.Status.ToString(),
                Type = e.Type.ToString(),
                XPReward = e.XPReward,
                PublishedAt = e.PublishedAt,
                ArchivedAt = e.ArchivedAt,
                CreatorPersonId = e.CreatorPersonId ?? 0
            };
        }

        private Encounter MapToDomain(EncounterDto dto)
        {
            var enc = new Encounter(dto.Name, dto.Description, dto.Location, dto.Latitude, dto.Longitude, Enum.Parse<EncouterType>(dto.Type), dto.XPReward);
            return enc;
        }

        private void UpdateDomain(Encounter existing, EncounterUpdateDto dto)
        {
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Location = dto.Location;
            existing.SetCoordinates(dto.Latitude, dto.Longitude);
            existing.Type = Enum.Parse<EncouterType>(dto.Type);
            existing.XPReward = dto.XPReward;
            if (!string.IsNullOrWhiteSpace(dto.Status)) existing.Status = Enum.Parse<EncouterStatus>(dto.Status);
        }
    }
}