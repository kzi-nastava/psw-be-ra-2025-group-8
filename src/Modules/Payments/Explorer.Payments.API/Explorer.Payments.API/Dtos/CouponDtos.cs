namespace Explorer.Payments.API.Dtos
{
    public class CouponDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long? TourId { get; set; }
        public long AuthorId { get; set; }
    }

    public class CreateCouponDto
    {
        public int DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long? TourId { get; set; }
    }

    public class UpdateCouponDto
    {
        public int DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long? TourId { get; set; }
    }
}
