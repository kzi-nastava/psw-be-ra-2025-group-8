using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubJoinRequestRepository : IClubJoinRequestRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubJoinRequest> _dbSet;

        public ClubJoinRequestRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubJoinRequest>();
        }

        public ClubJoinRequest Create(ClubJoinRequest request)
        {
            _dbSet.Add(request);
            _dbContext.SaveChanges();
            return request;
        }

        public ClubJoinRequest Get(long id)
        {
            return _dbSet.FirstOrDefault(r => r.Id == id) 
                ?? throw new KeyNotFoundException("ClubJoinRequest not found");
        }

        public ClubJoinRequest Update(ClubJoinRequest request)
        {
            _dbSet.Update(request);
            _dbContext.SaveChanges();
            return request;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<ClubJoinRequest> GetByClubId(long clubId)
        {
            return _dbSet.Where(r => r.ClubId == clubId).ToList();
        }

        public IEnumerable<ClubJoinRequest> GetByTouristId(long touristId)
        {
            return _dbSet.Where(r => r.TouristId == touristId).ToList();
        }

        public ClubJoinRequest? GetPendingRequest(long clubId, long touristId)
        {
            return _dbSet.FirstOrDefault(r => 
                r.ClubId == clubId && 
                r.TouristId == touristId && 
                r.Status == ClubJoinRequestStatus.Pending);
        }
    }
}
