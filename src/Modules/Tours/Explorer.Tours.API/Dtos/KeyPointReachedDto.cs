namespace Explorer.Tours.API.Dtos;

public class KeyPointReachedDto
{
    public long Id { get; set; }
    public long TourExecutionId { get; set; }
    public int KeyPointOrder { get; set; }
    public DateTime ReachedAt { get; set; }
}
