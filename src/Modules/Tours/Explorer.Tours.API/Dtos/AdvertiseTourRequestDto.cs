namespace Explorer.Tours.API.Dtos;

public class AdvertiseTourRequestDto
{
    // "Basic", "Standard", "Premium" (case-insensitive)
    public string Tier { get; set; } = string.Empty;
}
