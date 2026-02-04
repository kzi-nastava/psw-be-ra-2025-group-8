using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PurchasedItemDatabaseRepository : CrudDatabaseRepository<PurchasedItem, PaymentsContext>, IPurchasedItemRepository
    {
        public PurchasedItemDatabaseRepository(PaymentsContext dbContext) : base(dbContext) { }

        public List<PurchasedItem> GetByTourId(long tourId)
        {
            return DbContext.PurchasedItems
                .Where(pi => pi.TourId == tourId)
                .ToList();
        }

        public List<PurchasedItem> GetAll()
        {
            return DbContext.PurchasedItems.ToList();
        }
    }
}