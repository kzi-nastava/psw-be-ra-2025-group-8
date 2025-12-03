using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class OrderItem : Entity
    {
        public long TourId { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }

        internal OrderItem(long tourId, decimal price)
        {

            //if (tourId <= 0) throw new ArgumentException("Invalid Tour ID");
            if (price < 0) throw new ArgumentException("Price cannot be negative");

            TourId = tourId;
            Price = price;
        }
    }
}
