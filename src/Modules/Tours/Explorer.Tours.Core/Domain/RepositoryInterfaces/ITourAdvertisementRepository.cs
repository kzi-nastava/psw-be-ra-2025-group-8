using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourAdvertisementRepository
{
    TourAdvertisement Create(TourAdvertisement advertisement);

    // Active = EndsAtUtc > utcNow
    TourAdvertisement? GetActiveForTour(long tourId, DateTime utcNow);

    // Samo aktivne reklame (po tourId)
    Dictionary<long, TourAdvertisement> GetActiveByTourIds(IEnumerable<long> tourIds, DateTime utcNow);

    TourAdvertisement Update(TourAdvertisement advertisement);
}
