using Explorer.Encounters.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterParticipationService
    {
        EncounterParticipationDto ActivateEncounter(ActivateEncounterDto activateDto);
        EncounterParticipationDto CompleteEncounter(CompleteEncounterDto completeDto);
        EncounterParticipationDto AbandonEncounter(long personId, long encounterId);
        EncounterParticipationDto ReactivateEncounter(long personId, long encounterId);
        List<EncounterParticipationDto> GetParticipationsByPerson(long personId);
        List<EncounterParticipationDto> GetActiveEncountersByPerson(long personId);
        EncounterParticipationDto GetParticipation(long personId, long encounterId);
        CheckEncounterResponseDto CheckEncounterActiveStatus(CheckEncounterRequestDto request);
    }
}