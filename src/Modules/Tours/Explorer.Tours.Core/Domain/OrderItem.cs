using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
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
