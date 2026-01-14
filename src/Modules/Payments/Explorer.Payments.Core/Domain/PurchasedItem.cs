using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class PurchasedItem : Entity
    {
        public long UserId { get; private set; }
        public long TourId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public decimal Price { get; private set; }
        public int AdventureCoinsSpent { get; private set; }
        public long? SaleId { get; private set; }
        public long? CouponId { get; private set; }

        private PurchasedItem() { }

        // New constructor with UserId and AdventureCoinsSpent
        internal PurchasedItem(long userId, long tourId, decimal originalPrice, decimal price, int adventureCoinsSpent, long? saleId = null, long? couponId = null)
        {
            UserId = userId;
            TourId = tourId;
            OriginalPrice = originalPrice;
            Price = price;
            AdventureCoinsSpent = adventureCoinsSpent;
            SaleId = saleId;
            CouponId = couponId;
            PurchaseDate = DateTime.UtcNow;
        }

        // Old constructor for backward compatibility (used by existing code without wallet)
        internal PurchasedItem(long tourId, decimal originalPrice, decimal price, long? saleId = null, long? couponId = null)
        {
            UserId = 0; // Default value for backward compatibility
            TourId = tourId;
            OriginalPrice = originalPrice;
            Price = price;
            AdventureCoinsSpent = (int)Math.Ceiling(price); // Calculate from price
            SaleId = saleId;
            CouponId = couponId;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
