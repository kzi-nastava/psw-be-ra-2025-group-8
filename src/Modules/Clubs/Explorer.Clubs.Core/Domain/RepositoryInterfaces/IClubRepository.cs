using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Clubs.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        Club Create(Club club);
        Club Get(long id);
        IEnumerable<Club> GetAll();
        Club Update(Club club);
        void Delete(long id);

        PagedResult<Club> GetPaged(int page, int pageSize);
    }
}
