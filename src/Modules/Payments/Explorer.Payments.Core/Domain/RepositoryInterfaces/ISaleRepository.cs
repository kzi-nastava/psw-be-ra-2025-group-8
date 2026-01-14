using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ISaleRepository
    {
        Sale Create(Sale sale);
        Sale Update(Sale sale);
        void Delete(long id);
        Sale Get(long id);
        Sale GetByIdAndAuthor(long id, long authorId);
        List<Sale> GetByAuthor(long authorId);
        List<Sale> GetActiveSales();
        List<Sale> GetActiveSalesForTour(long tourId);
        PagedResult<Sale> GetPaged(int page, int pageSize);
    }
}
