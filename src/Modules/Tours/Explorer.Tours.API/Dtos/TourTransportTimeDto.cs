namespace Explorer.Tours.API.Dtos;

public class TourTransportTimeDto
{
    // "Walk", "Bicycle", "Car", "Boat"
    public string Transport { get; set; }
    public int DurationMinutes { get; set; }
}
