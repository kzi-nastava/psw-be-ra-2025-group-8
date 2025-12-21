using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterRepository : IEncounterRepository
    {
        private readonly EncountersContext _context;
        public EncounterRepository(EncountersContext context)
        {
            _context = context;
        }
        public Encounter? GetById(long id)
        {
            return _context.Encounters.FirstOrDefault(e => e.Id == id);
        }
        public Encounter Create(Encounter encounter)
        {
            _context.Encounters.Add(encounter);
            _context.SaveChanges();
            return encounter;
        }
        public Encounter Update(Encounter encounter)
        {
            _context.Encounters.Update(encounter);
            _context.SaveChanges();
            return encounter;
        }
        public IEnumerable<Encounter> GetAll()
        {
            return _context.Encounters.ToList();
        }

    }
}
