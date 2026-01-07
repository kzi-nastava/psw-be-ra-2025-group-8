using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class OrderItem : Entity
    {
        public long TourId { get; private set; }

        public decimal OriginalPrice { get; private set; }
        public decimal DiscountedPrice { get; private set; }

        public bool IsBundle { get; private set; }
        public long? BundleId { get; private set; }

        public long? CouponId { get; private set; }
        public long? SaleId { get; private set; }

        private OrderItem() { }

        internal OrderItem(long tourId)
        {
            TourId = tourId;
            OriginalPrice = 0;
            DiscountedPrice = 0;
            IsBundle = false;
        }

        internal OrderItem(long tourId, decimal originalPrice, decimal discountedPrice)
        {
            TourId = tourId;
            OriginalPrice = originalPrice;
            DiscountedPrice = discountedPrice;
            IsBundle = false;
        }
    }
}
