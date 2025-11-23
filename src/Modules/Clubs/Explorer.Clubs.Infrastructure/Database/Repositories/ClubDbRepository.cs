using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Clubs.Core.Domain;
using Explorer.Clubs.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Clubs.Infrastructure.Database.Repositories
{
    public class ClubDbRepository : IClubRepository
    {
        protected readonly ClubsContext _dbContext;
        private readonly DbSet<Club> _dbSet;
        public ClubDbRepository(ClubsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Club>();
        }

        public PagedResult<Club> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }
        public Club Create(Club club)
        {
            _dbSet.Add(club);
            _dbContext.SaveChanges();
            return club;
        }
        public Club Get(long id)
        {
            return _dbSet.FirstOrDefault(c => c.Id == id) ?? throw new KeyNotFoundException("Club not found");
        }
        public IEnumerable<Club> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }
        public Club Update(Club club)
        {
            _dbSet.Update(club);
            _dbContext.SaveChanges();
            return club;
        }
        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
