using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IPurchasedItemRepository : ICrudRepository<PurchasedItem>
    {
        List<PurchasedItem> GetByTourId(long tourId);
        List<PurchasedItem> GetAll();
    }
}