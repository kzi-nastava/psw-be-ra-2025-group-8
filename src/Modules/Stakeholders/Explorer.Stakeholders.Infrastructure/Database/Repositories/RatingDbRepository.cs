using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class RatingDbRepository : CrudDatabaseRepository<Rating, StakeholdersContext>, IRatingRepository
    {
        public RatingDbRepository(StakeholdersContext context) : base(context)
        {

        }
    }
}