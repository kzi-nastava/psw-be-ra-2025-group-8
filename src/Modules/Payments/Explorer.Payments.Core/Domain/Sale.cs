using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Sale : AggregateRoot
    {
        private List<SaleTour> _saleTours = new();
        public IReadOnlyCollection<SaleTour> SaleTours => _saleTours.AsReadOnly();
        
        public IReadOnlyCollection<long> TourIds => _saleTours.Select(st => st.TourId).ToList().AsReadOnly();
        
        public long AuthorId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int DiscountPercentage { get; private set; }

        private Sale() { }

        public Sale(long authorId, DateTime startDate, DateTime endDate, int discountPercentage, List<long> tourIds)
        {
            AuthorId = authorId;
            StartDate = startDate;
            EndDate = endDate;
            DiscountPercentage = discountPercentage;
            SetTours(tourIds ?? new List<long>());
            Validate();
        }

        public void Update(DateTime startDate, DateTime endDate, int discountPercentage, List<long> tourIds)
        {
            StartDate = startDate;
            EndDate = endDate;
            DiscountPercentage = discountPercentage;
            SetTours(tourIds ?? new List<long>());
            Validate();
        }

        private void SetTours(List<long> tourIds)
        {
            _saleTours.Clear();
            foreach (var tourId in tourIds)
            {
                _saleTours.Add(new SaleTour(Id, tourId));
            }
        }

        private void Validate()
        {
            if (AuthorId == 0)
                throw new ArgumentException("Invalid author ID.");

            if (StartDate < DateTime.UtcNow.Date)
                throw new ArgumentException("Start date cannot be in the past.");

            if (EndDate <= StartDate)
                throw new ArgumentException("End date must be after start date.");

            var maxEndDate = StartDate.AddDays(14);
            if (EndDate > maxEndDate)
                throw new ArgumentException("Sale duration cannot exceed 2 weeks from start date.");

            if (DiscountPercentage <= 0 || DiscountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 1 and 100.");

            if (_saleTours == null || _saleTours.Count == 0)
                throw new ArgumentException("Sale must include at least one tour.");
        }

        public bool IsActive()
        {
            var now = DateTime.UtcNow;
            return now >= StartDate && now <= EndDate;
        }

        public bool AppliesTo(long tourId)
        {
            return _saleTours.Any(st => st.TourId == tourId);
        }

        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            return originalPrice * (100 - DiscountPercentage) / 100;
        }
    }
}
