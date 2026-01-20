using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourStatsRepository : ITourStatsRepository
{
    private readonly ToursContext _context;

    public TourStatsRepository(ToursContext context)
    {
        _context = context;
    }

    public TourStats? GetByTourId(int tourId)
    {
        return _context.TourStats.FirstOrDefault(ts => ts.TourId == tourId);
    }

    public TourStats Create(TourStats tourStats)
    {
        _context.TourStats.Add(tourStats);
        _context.SaveChanges();
        return tourStats;
    }

    public TourStats Update(TourStats tourStats)
    {
        _context.TourStats.Update(tourStats);
        _context.SaveChanges();
        return tourStats;
    }
}
