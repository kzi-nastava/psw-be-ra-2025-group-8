namespace Explorer.Tours.API.Dtos;

public class TourExecutionDto
{
    public int Id { get; set; }
    public int IdTour { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double CompletionPercentage { get; set; }
    public int IdTourist { get; set; }
    public double CompletionPercentage { get; set; }
    public string Status { get; set; }
    public DateTime LastActivity { get; set; }
}