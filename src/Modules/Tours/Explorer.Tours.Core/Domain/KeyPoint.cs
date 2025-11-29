using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPoint : Entity
{
    public int TourId { get; private set; }
    public int OrderNum { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    public KeyPoint(int tourId, int orderNum, double latitude, double longitude, string? name = null, string? description = null)
    {
        TourId = tourId;
        OrderNum = orderNum;
        Latitude = latitude;
        Longitude = longitude;
        Name = name;
        Description = description;
        Validate();
    }

    private KeyPoint() { }

    private void Validate()
    {
        if (TourId <= 0)
            throw new ArgumentException("TourId must be a positive integer");

        if (OrderNum < 0)
            throw new ArgumentException("OrderNum cannot be negative");

        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");
    }
}
