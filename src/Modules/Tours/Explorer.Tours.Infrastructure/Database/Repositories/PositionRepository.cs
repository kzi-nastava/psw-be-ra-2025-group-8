using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

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
        return _context.Positions
            .FirstOrDefault(p => p.TouristId == touristId) 
            ?? throw new KeyNotFoundException($"Position not found for tourist with ID {touristId}");
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
