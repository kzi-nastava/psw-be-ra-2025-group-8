using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class KeyPointRepository : IKeyPointRepository
{
    private readonly ToursContext _context;

    public KeyPointRepository(ToursContext context)
    {
        _context = context;
    }

    public List<KeyPoint> GetByTour(long tourId)
    {
        return _context.KeyPoints
            .Where(kp => EF.Property<long>(kp, "TourId") == tourId)
            .OrderBy(kp => kp.Order)
            .ToList();
    }

    public KeyPoint Create(KeyPoint keyPoint)
    {
        _context.KeyPoints.Add(keyPoint);
        _context.SaveChanges();
        return keyPoint;
    }
}
