public class TourEconomicStatisticsDto
{
    public DateTime TimePeriod { get; set; }
    public decimal TotalRevenue { get; set; }
    public int SalesCount { get; set; }
    public decimal RevenueFromCoupons { get; set; }
    public decimal RevenueFromSales { get; set; }
    public decimal RevenueFullPrice { get; set; }
}