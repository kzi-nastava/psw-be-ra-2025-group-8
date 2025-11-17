using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourRepository : ITourRepository
{
    private readonly ToursContext _context;

    public TourRepository(ToursContext context)
    {
        _context = context;
    }

    public List<Tour> GetByAuthor(int authorId)
    {
        return _context.Tours
            .Where(t => t.AuthorId == authorId)
            .ToList();
    }
}