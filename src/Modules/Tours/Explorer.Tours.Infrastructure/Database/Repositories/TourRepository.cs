using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourRepository : ITourRepository
{
    private readonly ToursContext _context;

    public TourRepository(ToursContext context)
    {
        _context = context;
    }

    // Helper method to include related entities. Aggregate roots should be loaded with their related entities.
    private IQueryable<Tour> ToursWithIncludes()
    {
        return _context.Tours
            .Include(t => t.KeyPoints)
            .Include(t => t.RequiredEquipment)
                .ThenInclude(te => te.Equipment)
            .Include(t => t.TourTags)
                .ThenInclude(tt => tt.Tags)
            .Include(t => t.TransportTimes);
    }


    public Tour Get(long id)
    {
        return ToursWithIncludes()
            .FirstOrDefault(t => t.Id == id);
    }

    public Tour Create(Tour tour)
    {
        _context.Tours.Add(tour);
        _context.SaveChanges();
        return tour;
    }

    public Tour Update(Tour tour)
    {
        _context.Tours.Attach(tour);
        _context.SaveChanges();
        return tour;
    }

    public void Delete(long id)
    {
        var tour = ToursWithIncludes()
            .FirstOrDefault(t => t.Id == id);

        if (tour != null)
        {
            _context.Tours.Remove(tour);
            _context.SaveChanges();
        }
    }

    public List<Tour> GetByAuthor(int authorId)
    {
        return ToursWithIncludes()
            .Where(t => t.AuthorId == authorId)
            .ToList();
    }

    public List<Tour> GetAll()
    {
        return ToursWithIncludes().ToList();
    }


    //Maksim: Dodao sam Get po ID-ju zato sto su mi potrebni podaci Tour-a za ShoppingCart
    public Tour GetById(long id)
    {
        return _context.Tours
            .FirstOrDefault(t => t.Id == id);
    }

    public List<Tour> SearchByLocation(double latitude, double longitude, double distanceInKm)
    {
        var allPublishedTours = ToursWithIncludes()
            .Where(t => t.Status == TourStatus.Published)
            .ToList();

        var searchLocation = new GeoCoordinate(latitude, longitude);

        var toursWithinRange = allPublishedTours
            .Where(tour => tour.KeyPoints.Any(kp => 
                kp.Location.DistanceTo(searchLocation) <= distanceInKm))
            .ToList();

        return toursWithinRange;
    }
}