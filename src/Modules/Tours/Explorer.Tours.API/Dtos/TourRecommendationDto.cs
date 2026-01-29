namespace Explorer.Tours.API.Dtos;

public class TourRecommendationDto
{
    public int TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public RecommendationSentiment Sentiment { get; set; }
    public RecommendationCategory Category { get; set; }
}

public enum RecommendationSentiment
{
    Positive,
    Negative
}

public enum RecommendationCategory
{
    General,
    Economic
}

public class AuthorRecommendationsDto
{
    public int AuthorId { get; set; }
    public List<TourRecommendationDto> Recommendations { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
