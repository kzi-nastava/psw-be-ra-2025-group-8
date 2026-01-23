using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourAdvertisement : Entity
{
    public long TourId { get; private set; }
    public int AuthorId { get; private set; }
    public TourAdvertisementTier Tier { get; private set; }
    public int AdventureCoinsSpent { get; private set; }
    public DateTime PurchasedAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }

    protected TourAdvertisement() { }

    public TourAdvertisement(long tourId, int authorId, TourAdvertisementTier tier, int coinsSpent, DateTime purchasedAtUtc, DateTime endsAtUtc)
    {
        TourId = tourId;
        AuthorId = authorId;
        Tier = tier;
        AdventureCoinsSpent = coinsSpent;
        PurchasedAtUtc = purchasedAtUtc;
        EndsAtUtc = endsAtUtc;
    }

    // otkazivanje 
    public void Cancel(DateTime utcNow)
    {
        if (utcNow < PurchasedAtUtc) utcNow = PurchasedAtUtc;
        if (utcNow >= EndsAtUtc) return; // već istekao ili već "prekinut"
        EndsAtUtc = utcNow;
    }

}
