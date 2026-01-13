using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class SaleTour : Entity
    {
        public long SaleId { get; private set; }
        public long TourId { get; private set; }

        private SaleTour() { }

        public SaleTour(long saleId, long tourId)
        {
            SaleId = saleId;
            TourId = tourId;
        }
    }
}
