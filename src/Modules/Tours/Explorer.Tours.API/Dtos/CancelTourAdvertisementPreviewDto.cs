namespace Explorer.Tours.API.Dtos;

public class CancelTourAdvertisementPreviewDto
{
    public long TourId { get; set; }
    public int RefundedAdventureCoins { get; set; }
    public DateTime CalculatedAtUtc { get; set; }

    // opcionalno korisno za prikaz na FE (može i bez ovoga)
    public int TotalAdventureCoinsSpent { get; set; }
    public int SetupFeeAdventureCoins { get; set; }
    public DateTime PurchasedAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public string Tier { get; set; } = "";
}
