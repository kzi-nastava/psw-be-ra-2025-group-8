using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Position : Entity
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TouristId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Position(double latitude, double longitude, int touristId)
    {
        Latitude = latitude;
        Longitude = longitude;
        TouristId = touristId;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    public Position() { }

    public void UpdatePosition(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");
    }
}