using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Coupon : Entity
    {
        public string Code { get; private set; }
        public int DiscountPercentage { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public long? TourId { get; private set; }
        public long AuthorId { get; private set; }

        private Coupon() { }

        public Coupon(string code, int discountPercentage, long authorId, DateTime? expiryDate = null, long? tourId = null)
        {
            Code = code;
            DiscountPercentage = discountPercentage;
            ExpiryDate = expiryDate;
            TourId = tourId;
            AuthorId = authorId;
            Validate();
        }

        public void Update(int discountPercentage, DateTime? expiryDate, long? tourId)
        {
            DiscountPercentage = discountPercentage;
            ExpiryDate = expiryDate;
            TourId = tourId;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code) || Code.Length != 8)
                throw new ArgumentException("Coupon code must be exactly 8 characters.");

            if (DiscountPercentage <= 0 || DiscountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 1 and 100.");

            if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow)
                throw new ArgumentException("Expiry date cannot be in the past.");

            if (AuthorId == 0)
                throw new ArgumentException("Invalid author ID.");
        }

        public bool IsValid()
        {
            return !ExpiryDate.HasValue || ExpiryDate.Value >= DateTime.UtcNow;
        }

        public bool AppliesTo(long tourId, long authorId)
        {
            if (AuthorId != authorId)
                return false;

            return !TourId.HasValue || TourId.Value == tourId;
        }
    }
}
