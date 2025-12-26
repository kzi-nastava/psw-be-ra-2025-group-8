using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;

        public EncounterService(IEncounterRepository encounterRepository)
        {
            _encounterRepository = encounterRepository;
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

        public EncounterDto CreateEncounter(EncounterDto createDto)
        {
            var encounter = MapToDomain(createDto);
            var createdEncounter = _encounterRepository.Create(encounter);
            return MapToDto(createdEncounter);
        }

        public EncounterDto UpdateEncounter(long id, EncounterUpdateDto updateDto)
        {
            var existingEncounter = _encounterRepository.GetById(id);
            if (existingEncounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            UpdateDomain(existingEncounter, updateDto);
            var savedEncounter = _encounterRepository.Update(existingEncounter);

            return MapToDto(savedEncounter);
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
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Publish();
            var updatedEncounter = _encounterRepository.Update(encounter);
            return MapToDto(updatedEncounter);
        }

        public EncounterDto ArchiveEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Archive();
            var updatedEncounter = _encounterRepository.Update(encounter);
            return MapToDto(updatedEncounter);
        }

        public EncounterDto ReactivateEncounter(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {id} not found.");

            encounter.Reactivate();
            var updatedEncounter = _encounterRepository.Update(encounter);
            return MapToDto(updatedEncounter);
        }

        // --------------------
        // Manual mapping
        // --------------------

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
                Type = encounter.Type.ToString(),
                XPReward = encounter.XPReward,
                Status = encounter.Status.ToString(),
                PublishedAt = encounter.PublishedAt,
                ArchivedAt = encounter.ArchivedAt
            };
        }

        private Encounter MapToDomain(EncounterDto dto)
        {
            return new Encounter(
                dto.Name,
                dto.Description,
                dto.Location,
                dto.Latitude,
                dto.Longitude,
                Enum.Parse<EncouterType>(dto.Type),
                dto.XPReward
            );
        }

        private void UpdateDomain(Encounter existing, EncounterUpdateDto dto)
        {
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Location = dto.Location;

            existing.SetCoordinates(dto.Latitude, dto.Longitude);

            existing.XPReward = dto.XPReward;
            existing.Type = Enum.Parse<EncouterType>(dto.Type);
        }
    }
}