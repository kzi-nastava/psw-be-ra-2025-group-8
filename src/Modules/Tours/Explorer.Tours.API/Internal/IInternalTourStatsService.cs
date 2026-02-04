namespace Explorer.Tours.API.Internal;

public interface IInternalTourStatsService
{
    void RegisterPurchase(long tourId, double pricePaid);
}
