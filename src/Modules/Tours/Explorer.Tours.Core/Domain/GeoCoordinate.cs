using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json.Serialization;

namespace Explorer.Tours.Core.Domain;

public class GeoCoordinate : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }

    [JsonConstructor]
    public GeoCoordinate(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }

    public double DistanceTo(GeoCoordinate other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));

        // Haversine formula - result in kilometers
        const double EarthRadiusKm = 6371;

        double dLat = DegreesToRadians(other.Latitude - Latitude);
        double dLon = DegreesToRadians(other.Longitude - Longitude);

        double lat1Rad = DegreesToRadians(Latitude);
        double lat2Rad = DegreesToRadians(other.Latitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
        => degrees * Math.PI / 180.0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}