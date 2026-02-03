namespace Explorer.Payments.API.Dtos;

public class AuthorRevenueStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalPurchases { get; set; }
    public Dictionary<long, TourRevenueDto> RevenueByTour { get; set; } = new();
}

public class TourRevenueDto
{
    public long TourId { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PurchaseCount { get; set; }
}
