using Explorer.Encounters.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Encounters.Core.Domain.ReposotoryInterfaces
{
    public interface IEncounterParticipationRepository
    {
        EncounterParticipation? GetByPersonAndEncounter(long personId, long encounterId);
        EncounterParticipation Create(EncounterParticipation participation);
        EncounterParticipation Update(EncounterParticipation participation);
        IEnumerable<EncounterParticipation> GetAll();
        IEnumerable<EncounterParticipation> GetByPersonId(long personId);
        IEnumerable<EncounterParticipation> GetByEncounterId(long encounterId);
        IEnumerable<EncounterParticipation> GetActiveByPersonId(long personId);
        void Delete(long personId, long encounterId);
    }
}