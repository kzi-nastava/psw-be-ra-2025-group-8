using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class KeyPointReachedRepository : IKeyPointReachedRepository
{
    private readonly ToursContext _context;

    public KeyPointReachedRepository(ToursContext context)
    {
        _context = context;
    }

    public KeyPointReached Create(KeyPointReached keyPointReached)
    {
        _context.KeyPointsReached.Add(keyPointReached);
        _context.SaveChanges();
        return keyPointReached;
    }

    public List<KeyPointReached> GetByTourExecution(long tourExecutionId)
    {
        return _context.KeyPointsReached
            .Where(kpr => kpr.TourExecutionId == tourExecutionId)
            .OrderBy(kpr => kpr.KeyPointOrder)
            .ToList();
    }

    public List<int> GetReachedKeyPointOrders(long tourExecutionId)
    {
        return _context.KeyPointsReached
            .Where(kpr => kpr.TourExecutionId == tourExecutionId)
            .Select(kpr => kpr.KeyPointOrder)
            .ToList();
    }
}
