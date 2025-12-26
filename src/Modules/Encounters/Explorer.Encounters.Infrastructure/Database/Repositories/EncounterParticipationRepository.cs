using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterParticipationRepository : IEncounterParticipationRepository
    {
        private readonly EncountersContext _context;

        public EncounterParticipationRepository(EncountersContext context)
        {
            _context = context;
        }

        public EncounterParticipation? GetByPersonAndEncounter(long personId, long encounterId)
        {
            return _context.EncounterParticipations
                .FirstOrDefault(p => p.PersonId == personId && p.EncounterId == encounterId);
        }

        public EncounterParticipation Create(EncounterParticipation participation)
        {
            _context.EncounterParticipations.Add(participation);
            _context.SaveChanges();
            return participation;
        }

        public EncounterParticipation Update(EncounterParticipation participation)
        {
            _context.EncounterParticipations.Update(participation);
            _context.SaveChanges();
            return participation;
        }

        public IEnumerable<EncounterParticipation> GetAll()
        {
            return _context.EncounterParticipations.ToList();
        }

        public IEnumerable<EncounterParticipation> GetByPersonId(long personId)
        {
            return _context.EncounterParticipations
                .Where(p => p.PersonId == personId)
                .ToList();
        }

        public IEnumerable<EncounterParticipation> GetByEncounterId(long encounterId)
        {
            return _context.EncounterParticipations
                .Where(p => p.EncounterId == encounterId)
                .ToList();
        }

        public IEnumerable<EncounterParticipation> GetActiveByPersonId(long personId)
        {
            return _context.EncounterParticipations
                .Where(p => p.PersonId == personId && p.Status == ParticipationStatus.Active)
                .ToList();
        }

        public void Delete(long personId, long encounterId)
        {
            var participation = _context.EncounterParticipations
                .FirstOrDefault(p => p.PersonId == personId && p.EncounterId == encounterId);

            if (participation != null)
            {
                _context.EncounterParticipations.Remove(participation);
                _context.SaveChanges();
            }
        }
    }
}