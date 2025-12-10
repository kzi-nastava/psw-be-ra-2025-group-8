using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public interface ITouristTourService
{
    List<TouristTourPreviewDto> GetPublishedTours();
    TouristTourDetailsDto GetPublishedTourDetails(long id);
}

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourRatingService _tourRatingService;
    private readonly ICrudRepository<Person> _personRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public TouristTourService(
        ITourRepository tourRepository,
        ITourRatingService tourRatingService,
        ICrudRepository<Person> personRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _tourRepository = tourRepository;
        _tourRatingService = tourRatingService;
        _personRepository = personRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public List<TouristTourPreviewDto> GetPublishedTours()
    {
        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published)
            .ToList();

        return tours.Select(MapPreview).ToList();
    }

    public TouristTourDetailsDto GetPublishedTourDetails(long id)
    {
        var tour = _tourRepository.Get(id);

        if (tour == null || tour.Status != TourStatus.Published)
            throw new KeyNotFoundException("Tour not found or not published.");

        var dto = MapDetails(tour);
        dto.Reviews = MapReviews(id, out var avg);
        dto.AverageRating = avg;

        return dto;
    }

    // =======================================================
    // Private helpers
    // =======================================================

    private TouristTourPreviewDto MapPreview(Tour tour)
    {
        return new TouristTourPreviewDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Price = tour.Price,
            Tags = tour.TourTags.Select(tt => tt.Tags.Tag).ToList(),
            RequiredEquipment = tour.RequiredEquipment.Select(eq => eq.Equipment.Name).ToList(),
            FirstKeyPoint = MapFirstKeyPoint(tour),
            AverageRating = CalculateAverage(tour.Id)
        };
    }

    private TouristTourDetailsDto MapDetails(Tour tour)
    {
        return new TouristTourDetailsDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Difficulty = tour.Difficulty,
            Price = tour.Price,
            LengthInKilometers = tour.LengthInKilometers,
            Tags = tour.TourTags.Select(tt => tt.Tags.Tag).ToList(),
            RequiredEquipment = tour.RequiredEquipment.Select(eq => eq.Equipment.Name).ToList(),
            FirstKeyPoint = MapFirstKeyPoint(tour),
        };
    }

    private KeyPointPreviewDto MapFirstKeyPoint(Tour tour)
    {
        var kp = tour.KeyPoints.OrderBy(k => k.Order).FirstOrDefault();
        if (kp == null) return null;

        return new KeyPointPreviewDto
        {
            Name = kp.Name,
            ImageUrl = kp.ImageUrl,
            Latitude = kp.Location.Latitude,
            Longitude = kp.Location.Longitude
        };
    }

    private List<TourReviewDto> MapReviews(long tourId, out double average)
    {
        var ratings = _tourRatingService.GetByTour((int)tourId);
        average = ratings.Any() ? ratings.Average(r => r.Rating) : 0;

        return ratings.Select(r =>
        {
            var personId = _userRepository.GetPersonId(r.IdTourist);
            var person = _personRepository.Get(personId);

            return new TourReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                AuthorName = person != null ? $"{person.Name} {person.Surname}" : "Unknown Tourist",
                CreatedAt = r.CreatedAt
            };
        }).ToList();
    }

    private double CalculateAverage(long tourId)
    {
        var ratings = _tourRatingService.GetByTour((int)tourId);
        return ratings.Any() ? ratings.Average(r => r.Rating) : 0;
    }
}
