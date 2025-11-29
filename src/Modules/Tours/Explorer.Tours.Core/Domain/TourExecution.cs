using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourExecution : Entity
{
    public int IdTour { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int IdTourist { get; set; }
    public TourExecutionStatus Status { get; set; }
    public DateTime LastActivity { get; set; }

    public TourExecution(int idTour, double longitude, double latitude, int idTourist, TourExecutionStatus status)
    {
        IdTour = idTour;
        Longitude = longitude;
        Latitude = latitude;
        IdTourist = idTourist;
        Status = status;
        LastActivity = DateTime.UtcNow;
        Validate();
    }

    public TourExecution() { }

    public void UpdatePosition(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
        LastActivity = DateTime.UtcNow;
        Validate();
    }

    public void UpdateStatus(TourExecutionStatus status)
    {
        Status = status;
        LastActivity = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");

        if (IdTour <= 0)
            throw new ArgumentException("IdTour must be a positive integer");

        if (IdTourist <= 0)
            throw new ArgumentException("IdTourist must be a positive integer");
    }
    public enum TourExecutionStatus
    {
        Completed,
        Abandoned
    }
}