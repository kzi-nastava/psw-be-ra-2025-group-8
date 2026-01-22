namespace Explorer.Tours.API.Dtos;

public class TourStatsDto
{
    public int TourId { get; set; }
    public double CompletionRate { get; set; }
    public double AverageCompletionPercentage { get; set; }
    public string MostCommonDifficultyLevel { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
