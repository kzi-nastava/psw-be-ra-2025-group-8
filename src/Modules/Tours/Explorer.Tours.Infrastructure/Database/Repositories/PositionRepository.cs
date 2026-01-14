using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ToursContext _context;

    public PositionRepository(ToursContext context)
    {
        _context = context;
    }

    public Position GetByTouristId(int touristId)
    {
        // Return the most recently updated position for the tourist to avoid stale records
        return _context.Positions
            .Where(p => p.TouristId == touristId)
            .OrderByDescending(p => p.UpdatedAt)
            .FirstOrDefault();
    }

    public Position Create(Position position)
    {
        _context.Positions.Add(position);
        _context.SaveChanges();
        return position;
    }

    public Position Update(Position position)
    {
        _context.Positions.Update(position);
        _context.SaveChanges();
        return position;
    }
}
