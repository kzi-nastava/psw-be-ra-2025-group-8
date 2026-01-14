using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubInvitationRepository : IClubInvitationRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubInvitation> _dbSet;

        public ClubInvitationRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubInvitation>();
        }

        public ClubInvitation Create(ClubInvitation invitation)
        {
            _dbSet.Add(invitation);
            _dbContext.SaveChanges();
            return invitation;
        }

        public ClubInvitation Get(long id)
        {
            return _dbSet.FirstOrDefault(i => i.Id == id) 
                ?? throw new KeyNotFoundException("ClubInvitation not found");
        }

        public ClubInvitation Update(ClubInvitation invitation)
        {
            _dbSet.Update(invitation);
            _dbContext.SaveChanges();
            return invitation;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<ClubInvitation> GetByClubId(long clubId)
        {
            return _dbSet.Where(i => i.ClubId == clubId).ToList();
        }

        public IEnumerable<ClubInvitation> GetByTouristId(long touristId)
        {
            return _dbSet.Where(i => i.TouristId == touristId).ToList();
        }

        public ClubInvitation? GetPendingInvitation(long clubId, long touristId)
        {
            return _dbSet.FirstOrDefault(i => 
                i.ClubId == clubId && 
                i.TouristId == touristId && 
                i.Status == ClubInvitationStatus.Pending);
        }
    }
}
