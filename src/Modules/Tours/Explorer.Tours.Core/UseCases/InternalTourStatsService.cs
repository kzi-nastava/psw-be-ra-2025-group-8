using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

public class InternalTourStatsService : IInternalTourStatsService
{
    private readonly ITourStatsRepository _statsRepository;
    public InternalTourStatsService(ITourStatsRepository statsRepository)
    {
        _statsRepository = statsRepository;
    }

    public void RegisterPurchase(long tourId, double pricePaid)
    {
        var stats = _statsRepository.GetByTourId((int)tourId);
        if (stats != null)
        {
            stats.RecordPurchase(pricePaid);
            _statsRepository.Update(stats);
        }
        // Opciono: ako stats ne postoji, kreiraj novi inicijalni unos
    }
}