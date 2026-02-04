namespace Explorer.Tours.API.Dtos;

public class TourAdvertisementDto
{
    public long TourId { get; set; }
    public string Tier { get; set; } = string.Empty;
    public int AdventureCoinsSpent { get; set; }
    public DateTime PurchasedAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public bool IsActive { get; set; }
}
