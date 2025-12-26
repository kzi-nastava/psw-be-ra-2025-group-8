using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class OrderItem : Entity
    {
        public long TourId { get; private set; }

        private OrderItem() { }

        internal OrderItem(long tourId)
        {

            //if (tourId <= 0) throw new ArgumentException("Invalid Tour ID");

            TourId = tourId;
        }
    }
}
