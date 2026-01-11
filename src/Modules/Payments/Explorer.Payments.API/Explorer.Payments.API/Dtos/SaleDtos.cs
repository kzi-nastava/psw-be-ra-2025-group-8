namespace Explorer.Payments.API.Dtos
{
    public class SaleDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercentage { get; set; }
        public List<long> TourIds { get; set; } = new();
    }

    public class CreateSaleDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercentage { get; set; }
        public List<long> TourIds { get; set; } = new();
    }

    public class UpdateSaleDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercentage { get; set; }
        public List<long> TourIds { get; set; } = new();
    }

    public class TourSaleInfoDto
    {
        public long TourId { get; set; }
        public bool IsOnSale { get; set; }
        public int? DiscountPercentage { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
