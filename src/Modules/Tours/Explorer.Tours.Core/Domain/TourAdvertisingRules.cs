namespace Explorer.Tours.Core.Domain;

public static class TourAdvertisingRules
{
    public static (int CostInAdventureCoins, int DurationDays, int Priority) ForTier(TourAdvertisementTier tier)
    {
        return tier switch
        {
            TourAdvertisementTier.Basic => (50, 7, 1),
            TourAdvertisementTier.Standard => (100, 14, 2),
            TourAdvertisementTier.Premium => (200, 30, 3),
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, "Unknown advertising tier")
        };
    }
}
