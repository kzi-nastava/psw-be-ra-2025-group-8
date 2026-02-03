namespace Explorer.Tours.API.Dtos;

public class TourRatingTrendDto
{
    public int TourId { get; set; }
    public double CurrentMonthRating { get; set; }
    public double PreviousMonthRating { get; set; }
    public TrendDirection Trend { get; set; }
    public double PercentageChange { get; set; }
    public int CurrentMonthReviewCount { get; set; }
    public int PreviousMonthReviewCount { get; set; }
}

public enum TrendDirection
{
    Up,
    Down,
    Stable,
    NoData
}

public class AuthorTourTrendsDto
{
    public int AuthorId { get; set; }
    public List<TourRatingTrendDto> TourTrends { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
