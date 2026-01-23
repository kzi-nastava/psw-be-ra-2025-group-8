using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourAdvertisementRepository : ITourAdvertisementRepository
{
    private readonly ToursContext _context;

    public TourAdvertisementRepository(ToursContext context)
    {
        _context = context;
    }

    public TourAdvertisement Create(TourAdvertisement advertisement)
    {
        _context.TourAdvertisements.Add(advertisement);
        _context.SaveChanges();
        return advertisement;
    }

    public TourAdvertisement? GetActiveForTour(long tourId, DateTime utcNow)
    {
        return _context.TourAdvertisements
            .AsNoTracking()
            .Where(a => a.TourId == tourId && a.EndsAtUtc > utcNow)
            .OrderByDescending(a => a.Tier)
            .ThenByDescending(a => a.PurchasedAtUtc)
            .FirstOrDefault();
    }

    public Dictionary<long, TourAdvertisement> GetActiveByTourIds(IEnumerable<long> tourIds, DateTime utcNow)
    {
        var ids = tourIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<long, TourAdvertisement>();

        var active = _context.TourAdvertisements
            .AsNoTracking()
            .Where(a => ids.Contains(a.TourId) && a.EndsAtUtc > utcNow)
            .ToList();
        return active
            .GroupBy(a => a.TourId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(a => a.Tier).ThenByDescending(a => a.PurchasedAtUtc).First()
            );
    }

    public TourAdvertisement Update(TourAdvertisement advertisement)
    {
        _context.TourAdvertisements.Update(advertisement);
        _context.SaveChanges();
        return advertisement;
    }

}
