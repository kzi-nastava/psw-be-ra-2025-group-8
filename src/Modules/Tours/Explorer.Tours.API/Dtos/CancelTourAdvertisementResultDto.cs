namespace Explorer.Tours.API.Dtos;

public class CancelTourAdvertisementResultDto
{
    public long TourId { get; set; }
    public int RefundedAdventureCoins { get; set; }
    public DateTime CancelledAtUtc { get; set; }
}
