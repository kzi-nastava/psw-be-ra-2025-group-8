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
            // Explicit ID generation to avoid relying on database identity configuration.
            // Test databases and existing schemas may not have identity/sequence set up for this table.
            Id = Random.Shared.NextInt64(long.MinValue, -1);
            SaleId = saleId;
            TourId = tourId;
        }
    }
}
