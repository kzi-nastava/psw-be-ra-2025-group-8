using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterParticipationService : IEncounterParticipationService
    {
        private readonly IEncounterParticipationRepository _participationRepository;
        private readonly IEncounterRepository _encounterRepository;

        public EncounterParticipationService(
            IEncounterParticipationRepository participationRepository,
            IEncounterRepository encounterRepository)
        {
            _participationRepository = participationRepository;
            _encounterRepository = encounterRepository;
        }

        public EncounterParticipationDto ActivateEncounter(ActivateEncounterDto activateDto)
        {
            // Verify encounter exists and is published
            var encounter = _encounterRepository.GetById(activateDto.EncounterId);
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {activateDto.EncounterId} not found.");

            if (encounter.Status != EncouterStatus.Published)
                throw new InvalidOperationException("Only published encounters can be activated.");

            // Check if person already has this encounter activated
            var existing = _participationRepository.GetByPersonAndEncounter(
                activateDto.PersonId, activateDto.EncounterId);

            if (existing != null)
                throw new InvalidOperationException(
                    $"Person {activateDto.PersonId} has already activated encounter {activateDto.EncounterId}.");

            // Create new participation
            var participation = new EncounterParticipation(activateDto.PersonId, activateDto.EncounterId);
            var created = _participationRepository.Create(participation);

            return MapToDto(created);
        }

        public EncounterParticipationDto CompleteEncounter(CompleteEncounterDto completeDto)
        {
            var participation = _participationRepository.GetByPersonAndEncounter(
                completeDto.PersonId, completeDto.EncounterId);

            if (participation == null)
                throw new KeyNotFoundException(
                    $"Participation not found for person {completeDto.PersonId} and encounter {completeDto.EncounterId}.");

            // Get encounter to retrieve XP reward
            var encounter = _encounterRepository.GetById(participation.EncounterId);
            if (encounter == null)
                throw new KeyNotFoundException($"Encounter with ID {participation.EncounterId} not found.");

            participation.Complete(encounter.XPReward);
            var updated = _participationRepository.Update(participation);

            return MapToDto(updated);
        }

        public EncounterParticipationDto AbandonEncounter(long personId, long encounterId)
        {
            var participation = _participationRepository.GetByPersonAndEncounter(personId, encounterId);
            if (participation == null)
                throw new KeyNotFoundException(
                    $"Participation not found for person {personId} and encounter {encounterId}.");

            participation.Abandon();
            var updated = _participationRepository.Update(participation);

            return MapToDto(updated);
        }

        public EncounterParticipationDto ReactivateEncounter(long personId, long encounterId)
        {
            var participation = _participationRepository.GetByPersonAndEncounter(personId, encounterId);
            if (participation == null)
                throw new KeyNotFoundException(
                    $"Participation not found for person {personId} and encounter {encounterId}.");

            participation.Reactivate();
            var updated = _participationRepository.Update(participation);

            return MapToDto(updated);
        }

        public List<EncounterParticipationDto> GetParticipationsByPerson(long personId)
        {
            var participations = _participationRepository.GetByPersonId(personId);
            return participations.Select(MapToDto).ToList();
        }

        public List<EncounterParticipationDto> GetActiveEncountersByPerson(long personId)
        {
            var participations = _participationRepository.GetActiveByPersonId(personId);
            return participations.Select(MapToDto).ToList();
        }

        public EncounterParticipationDto GetParticipation(long personId, long encounterId)
        {
            var participation = _participationRepository.GetByPersonAndEncounter(personId, encounterId);
            if (participation == null)
                throw new KeyNotFoundException(
                    $"Participation not found for person {personId} and encounter {encounterId}.");

            return MapToDto(participation);
        }

        // Manual mapping
        private EncounterParticipationDto MapToDto(EncounterParticipation participation)
        {
            return new EncounterParticipationDto
            {
                PersonId = participation.PersonId,
                EncounterId = participation.EncounterId,
                Status = participation.Status.ToString(),
                ActivatedAt = participation.ActivatedAt,
                CompletedAt = participation.CompletedAt,
                XPAwarded = participation.XPAwarded
            };
        }
    }
}