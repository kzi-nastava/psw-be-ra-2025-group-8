using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class PurchasedItem : Entity
    {
        public long TourId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal Price { get; private set; }

        private PurchasedItem() { }

        internal PurchasedItem(long tourId, decimal price)
        {
            TourId = tourId;
            Price = price;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
