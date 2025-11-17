using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Facility : Entity
{
    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public FacilityCategory Category { get; private set; }

    public Facility(string name, double latitude, double longitude, FacilityCategory category)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name is required");
        if (Latitude < -90 || Latitude > 90) throw new ArgumentException("Invalid latitude");
        if (Longitude < -180 || Longitude > 180) throw new ArgumentException("Invalid longitude");
    }
}

public enum FacilityCategory
{
    WC,
    Restaurant,
    Parking,
    Other
}