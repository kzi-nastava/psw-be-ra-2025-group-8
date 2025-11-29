using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPointReached : Entity
{
    public long TourExecutionId { get; private set; }
    public int KeyPointOrder { get; private set; }
    public DateTime ReachedAt { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public KeyPointReached(long tourExecutionId, int keyPointOrder, double latitude, double longitude)
    {
        TourExecutionId = tourExecutionId;
        KeyPointOrder = keyPointOrder;
        ReachedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        Latitude = latitude;
        Longitude = longitude;
        Validate();
    }

    private KeyPointReached() { }

    private void Validate()
    {
        if (TourExecutionId == 0)
            throw new ArgumentException("TourExecutionId cannot be zero");

        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");
    }
}
