using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IMonumentRepository
    {
        PagedResult<Monument> GetPaged(int page, int pageSize);
        Monument Create(Monument map);
        Monument Update(Monument map);
        void Delete(long id);
    }
}
