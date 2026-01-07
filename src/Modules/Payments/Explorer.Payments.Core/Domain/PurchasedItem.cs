using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class PurchasedItem : Entity
    {
        public long UserId { get; private set; }
        public long TourId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal Price { get; private set; }
        public int AdventureCoinsSpent { get; private set; }

        private PurchasedItem() { }

        // New constructor with UserId and AdventureCoinsSpent
        internal PurchasedItem(long userId, long tourId, decimal price, int adventureCoinsSpent)
        {
            UserId = userId;
            TourId = tourId;
            Price = price;
            AdventureCoinsSpent = adventureCoinsSpent;
            PurchaseDate = DateTime.UtcNow;
        }

        // Old constructor for backward compatibility (used by existing code without wallet)
        internal PurchasedItem(long tourId, decimal price)
        {
            UserId = 0; // Default value for backward compatibility
            TourId = tourId;
            Price = price;
            AdventureCoinsSpent = (int)Math.Ceiling(price); // Calculate from price
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
