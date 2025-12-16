using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubMessageRepository : IClubMessageRepository
    {
        private readonly StakeholdersContext _context;
        private readonly DbSet<ClubMessage> _dbSet;

        public ClubMessageRepository(StakeholdersContext context)
        {
            _context = context;
            _dbSet = _context.Set<ClubMessage>();
        }

        public ClubMessage Create(ClubMessage message)
        {
            _dbSet.Add(message);
            _context.SaveChanges();
            return message;
        }

        public ClubMessage Get(long id)
        {
            return _dbSet.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<ClubMessage> GetByClubId(long clubId)
        {
            return _dbSet
                .Where(m => m.ClubId == clubId)
                .OrderByDescending(m => m.TimestampCreated)
                .ToList();
        }

        public ClubMessage Update(ClubMessage message)
        {
            _dbSet.Update(message);
            _context.SaveChanges();
            return message;
        }

        public void Delete(long id)
        {
            var message = Get(id);
            if (message != null)
            {
                _dbSet.Remove(message);
                _context.SaveChanges();
            }
        }
    }
}
