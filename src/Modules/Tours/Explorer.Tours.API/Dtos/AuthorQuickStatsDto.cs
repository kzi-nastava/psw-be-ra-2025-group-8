namespace Explorer.Tours.API.Dtos;

public class AuthorQuickStatsDto
{
    public int TotalTours { get; set; }
    public int PublishedTours { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
    public int TotalTourists { get; set; }
    public int TotalCompletions { get; set; }
    public MostPopularTourDto? MostPopularTour { get; set; }
}

public class MostPopularTourDto
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public int CompletionCount { get; set; }
    public double AverageRating { get; set; }
}
