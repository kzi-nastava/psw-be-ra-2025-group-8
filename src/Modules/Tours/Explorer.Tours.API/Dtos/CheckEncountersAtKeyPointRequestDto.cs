namespace Explorer.Tours.API.Dtos;

public class CheckEncountersAtKeyPointRequestDto
{
    public long TourExecutionId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
