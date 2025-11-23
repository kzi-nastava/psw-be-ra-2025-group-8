using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class RatingDbRepository : CrudDatabaseRepository<Rating, StakeholdersContext>, IRatingRepository
    {
        private readonly StakeholdersContext _context;
        public RatingDbRepository(StakeholdersContext context) : base(context)
        {
            _context = context;
        }

        public Rating GetByUserId(long userId)
        {
            return _context.Ratings
                .FirstOrDefault(r => r.UserId == userId);
        }
    }
}