namespace Explorer.Tours.API.Dtos;

public class UpdateTourTransportTimesDto
{
    public List<TourTransportTimeDto> TransportTimes { get; set; } = new();
}
