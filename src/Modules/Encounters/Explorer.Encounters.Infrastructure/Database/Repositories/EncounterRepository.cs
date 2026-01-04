using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;
using Microsoft.EntityFrameworkCore;
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
            try
            {
                _context.Encounters.Add(encounter);
                _context.SaveChanges();
                return encounter;
            }
            catch (DbUpdateException dbEx)
            {
                // If duplicate key occurred because DB sequence is out of sync, try to fix sequence and retry once
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                if (msg != null && msg.Contains("duplicate key value"))
                {
                    // Sync sequence to max(id)
                    var sql = "SELECT setval(pg_get_serial_sequence('encounters.\"Encounters\"','\"Id\"'), (SELECT COALESCE(MAX(\"Id\"), 1) FROM encounters.\"Encounters\"));";
                    try
                    {
                        _context.Database.ExecuteSqlRaw(sql);
                        // retry
                        _context.SaveChanges();
                        return encounter;
                    }
                    catch (Exception retryEx)
                    {
                        throw new InvalidOperationException($"Failed to create Encounter after syncing sequence (Name={encounter?.Name}). See inner exception.", retryEx);
                    }
                }

                throw new InvalidOperationException($"Failed to create Encounter (Name={encounter?.Name}). See inner exception.", dbEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create Encounter (Name={encounter?.Name}). See inner exception.", ex);
            }
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
        public void Delete(long id)
        {
            var encounter = _context.Encounters.FirstOrDefault(e => e.Id == id);
            if (encounter != null)
            {
                _context.Encounters.Remove(encounter);
                _context.SaveChanges();
            }
        }

    }
}
