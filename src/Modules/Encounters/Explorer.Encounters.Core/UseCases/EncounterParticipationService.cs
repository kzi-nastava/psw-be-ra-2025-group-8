using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using Explorer.Encounters.Core.Utils;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterParticipationService : IEncounterParticipationService
    {
        private readonly IEncounterParticipationRepository _participationRepository;
        private readonly IEncounterRepository _encounterRepository;
        private readonly IInternalPersonService _personService;
        private readonly IInternalPositionService _positionService;

        private const double ActivationRangeMeters = 200.0;
        private const int DefaultSocialRequiredCount = 3;
        private const double DefaultSocialRangeMeters = 200.0;

        public EncounterParticipationService(
            IEncounterParticipationRepository participationRepository,
            IEncounterRepository encounterRepository,
            IInternalPersonService personService,
            IInternalPositionService positionService)
        {
            _participationRepository = participationRepository ?? throw new ArgumentNullException(nameof(participationRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
            _positionService = positionService ?? throw new ArgumentNullException(nameof(positionService));
        }

        public EncounterParticipationDto ActivateEncounter(ActivateEncounterDto activateDto)
        {
            var encounter = _encounter_repository_get(activateDto.EncounterId);
            if (encounter.Status != EncouterStatus.Published)
                throw new InvalidOperationException("Only published encounters can be activated.");

            if (!encounter.Latitude.HasValue || !encounter.Longitude.HasValue)
                throw new InvalidOperationException("Encounter does not have valid coordinates.");

            var person = _personService.GetByUserId(activateDto.PersonId) ?? throw new KeyNotFoundException($"Person with ID {activateDto.PersonId} not found.");

            var pos = GetPositionForPerson(person);
            if (pos == null)
                throw new InvalidOperationException($"Position not found for person {person.Id} (user {person.UserId}).");

            var isWithinRange = DistanceCalculator.IsWithinRange(
                pos.Latitude, pos.Longitude,
                encounter.Latitude.Value, encounter.Longitude.Value,
                ActivationRangeMeters);

            if (!isWithinRange)
            {
                var distance = DistanceCalculator.CalculateDistance(pos.Latitude, pos.Longitude, encounter.Latitude.Value, encounter.Longitude.Value);
                throw new InvalidOperationException($"You must be within {ActivationRangeMeters}m of the encounter to activate it. Current distance: {distance:F0}m");
            }

            var existing = _participationRepository.GetByPersonAndEncounter(activateDto.PersonId, activateDto.EncounterId);
            if (existing != null)
                return MapToDto(existing);

            var participation = new EncounterParticipation(activateDto.PersonId, activateDto.EncounterId);
            var created = _participationRepository.Create(participation);

            if (encounter.Type == EncouterType.SocialBased)
            {
                EvaluateAndCompleteSocialEncounter(encounter);
            }

            return MapToDto(created);
        }

        public EncounterParticipationDto CompleteEncounter(CompleteEncounterDto completeDto)
        {
            var participation = _participation_repository_get_by_person_and_encounter(completeDto.PersonId, completeDto.EncounterId);
            var encounter = _encounter_repository_get(participation.EncounterId);

            if (!encounter.Latitude.HasValue || !encounter.Longitude.HasValue)
                throw new InvalidOperationException("Encounter does not have valid coordinates.");

            var person = _personService.GetByUserId(completeDto.PersonId) ?? throw new KeyNotFoundException($"Person with ID {completeDto.PersonId} not found.");
            var pos = GetPositionForPerson(person);
            if (pos == null)
                throw new InvalidOperationException($"Position not found for person {person.Id} (user {person.UserId}).");

            var isWithinRange = DistanceCalculator.IsWithinRange(pos.Latitude, pos.Longitude, encounter.Latitude.Value, encounter.Longitude.Value, ActivationRangeMeters);
            if (!isWithinRange)
            {
                var distance = DistanceCalculator.CalculateDistance(pos.Latitude, pos.Longitude, encounter.Latitude.Value, encounter.Longitude.Value);
                throw new InvalidOperationException($"You must be within {ActivationRangeMeters}m of the encounter to complete it. Current distance: {distance:F0}m");
            }

            participation.Complete(encounter.XPReward);
            var updated = _participationRepository.Update(participation);

            return MapToDto(updated);
        }

        public EncounterParticipationDto AbandonEncounter(long personId, long encounterId)
        {
            var participation = _participation_repository_get_by_person_and_encounter(personId, encounterId);
            participation.Abandon();
            var updated = _participationRepository.Update(participation);
            return MapToDto(updated);
        }

        public EncounterParticipationDto ReactivateEncounter(long personId, long encounterId)
        {
            var participation = _participation_repository_get_by_person_and_encounter(personId, encounterId);
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
            var participation = _participation_repository_get_by_person_and_encounter(personId, encounterId);
            return MapToDto(participation);
        }

        // Evaluate social encounter using encounter settings and complete if threshold met
        private void EvaluateAndCompleteSocialEncounter(Encounter encounter)
        {
            var requiredCount = encounter.SocialRequiredCount ?? DefaultSocialRequiredCount;
            var socialRange = encounter.SocialRangeMeters ?? DefaultSocialRangeMeters;

            var activeParticipations = _participationRepository.GetByEncounterId(encounter.Id)
                .Where(p => p.Status == ParticipationStatus.Active)
                .ToList();

            var inRange = new List<EncounterParticipation>();

            foreach (var p in activeParticipations)
            {
                try
                {
                    var pos = GetPositionForParticipant(p);
                    if (pos == null) continue;

                    var isWithin = DistanceCalculator.IsWithinRange(pos.Latitude, pos.Longitude, encounter.Latitude.Value, encounter.Longitude.Value, socialRange);
                    Console.WriteLine($"[SocialEval] Person={p.PersonId} pos=({pos.Latitude},{pos.Longitude}) within={isWithin}");

                    if (isWithin)
                        inRange.Add(p);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SocialEval] error for person {p.PersonId}: {ex.Message}");
                }
            }

            if (inRange.Count >= requiredCount)
            {
                foreach (var p in inRange)
                {
                    try
                    {
                        p.Complete(encounter.XPReward);
                        _participationRepository.Update(p);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SocialEval] complete error for person {p.PersonId}: {ex.Message}");
                    }
                }
            }
        }

        public CheckEncounterResponseDto CheckEncounterActiveStatus(CheckEncounterRequestDto request)
        {
            var encounter = _encounter_repository_get(request.EncounterId);

            var requiredCount = encounter.SocialRequiredCount ?? DefaultSocialRequiredCount;
            var socialRange = encounter.SocialRangeMeters ?? DefaultSocialRangeMeters;

            var activeParticipations = _participationRepository.GetByEncounterId(encounter.Id)
                .Where(p => p.Status == ParticipationStatus.Active)
                .ToList();

            var inRange = new List<EncounterParticipation>();

            foreach (var p in activeParticipations)
            {
                try
                {
                    double lat, lon;
                    if (p.PersonId == request.PersonId)
                    {
                        lat = request.Latitude;
                        lon = request.Longitude;
                    }
                    else
                    {
                        var pos = GetPositionForParticipant(p);
                        if (pos == null)
                        {
                            Console.WriteLine($"[Check] No position for person {p.PersonId}");
                            continue;
                        }

                        lat = pos.Latitude;
                        lon = pos.Longitude;
                    }

                    var isWithin = DistanceCalculator.IsWithinRange(lat, lon, encounter.Latitude.Value, encounter.Longitude.Value, socialRange);
                    Console.WriteLine($"[Check] Person={p.PersonId} lat={lat} lon={lon} within={isWithin}");

                    if (isWithin)
                        inRange.Add(p);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Check] error for person {p.PersonId}: {ex.Message}");
                }
            }

            var response = new CheckEncounterResponseDto
            {
                ActiveCount = inRange.Count,
                ThresholdReached = inRange.Count >= requiredCount,
                CompletedPersonIds = new List<long>()
            };

            if (response.ThresholdReached)
            {
                foreach (var p in inRange)
                {
                    try
                    {
                        if (p.Status == ParticipationStatus.Active)
                        {
                            p.Complete(encounter.XPReward);
                            _participationRepository.Update(p);
                            response.CompletedPersonIds.Add(p.PersonId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Check] complete error for person {p.PersonId}: {ex.Message}");
                    }
                }
            }

            return response;
        }

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

        // Helper: attempt multiple ways to find a position for a given Person
        private PositionDto GetPositionForPerson(PersonDto person)
        {
            // 1) try Person.Id
            try
            {
                var pos = _positionService.GetByTouristId((int)person.Id);
                if (pos != null) return pos;
            }
            catch { }

            // 2) try Person.UserId
            try
            {
                var pos = _positionService.GetByTouristId((int)person.UserId);
                if (pos != null) return pos;
            }
            catch { }

            // 3) last resort: try GetByUserId using UserId then lookup by returned person's id (covers mismatches)
            try
            {
                var dto = _personService.GetByUserId(person.UserId);
                if (dto != null)
                {
                    var pos = _positionService.GetByTouristId((int)dto.Id);
                    if (pos != null) return pos;

                    pos = _positionService.GetByTouristId((int)dto.UserId);
                    if (pos != null) return pos;
                }
            }
            catch { }

            return null;
        }

        // Helper for participant-only case
        private PositionDto GetPositionForParticipant(EncounterParticipation p)
        {
            // Try direct personId as tourist id
            try
            {
                var pos = _positionService.GetByTouristId((int)p.PersonId);
                if (pos != null) return pos;
            }
            catch { }

            // Try interpreting PersonId as userId and get person DTO
            try
            {
                var personDto = _personService.GetByUserId(p.PersonId);
                if (personDto != null)
                {
                    var pos = _positionService.GetByTouristId((int)personDto.Id);
                    if (pos != null) return pos;

                    pos = _positionService.GetByTouristId((int)personDto.UserId);
                    if (pos != null) return pos;
                }
            }
            catch { }

            // nothing found
            return null;
        }

        private Encounter _encounter_repository_get(long id) => _encounterRepository.GetById(id) ?? throw new KeyNotFoundException($"Encounter with ID {id} not found.");
        private EncounterParticipation _participation_repository_get_by_person_and_encounter(long personId, long encounterId) => _participationRepository.GetByPersonAndEncounter(personId, encounterId) ?? throw new KeyNotFoundException($"Participation not found for person {personId} and encounter {encounterId}.");
    }
}