using AutoMapper;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public interface ITouristTourService
{
    List<TouristTourPreviewDto> GetPublishedTours();
    TouristTourDetailsDto GetPublishedTourDetails(long id);
    List<KeyPointDto> GetTourKeyPoints(long tourId);
}

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourRatingService _tourRatingService;
    private readonly IInternalPersonService _profileProvider;
    private readonly IMapper _mapper;

    public TouristTourService(
        ITourRepository tourRepository,
        ITourRatingService tourRatingService,
        IInternalPersonService profileProvider,
        IMapper mapper)
    {
        _tourRepository = tourRepository;
        _tourRatingService = tourRatingService;
        _profileProvider = profileProvider;
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
            return null;

        var dto = MapDetails(tour);
        dto.Reviews = MapReviews(id, out var avg);
        dto.AverageRating = avg;
        dto.FirstKeyPoint = MapFirstKeyPoint(tour);

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
            AverageRating = CalculateAverage(tour.Id),
            Author = _profileProvider.GetByUserId(tour.AuthorId).Name
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
            Author = _profileProvider.GetByUserId(tour.AuthorId).Name
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

        return ratings.Select(r => new TourReviewDto
        {
            Rating = r.Rating,
            Comment = r.Comment,
            AuthorName = _profileProvider.GetByUserId(r.IdTourist).Name,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    private double CalculateAverage(long tourId)
    {
        var ratings = _tourRatingService.GetByTour((int)tourId);
        return ratings.Any() ? ratings.Average(r => r.Rating) : 0;
    }

    public List<KeyPointDto> GetTourKeyPoints(long tourId)
    {
        var tour = _tourRepository.Get(tourId);

        if (tour == null || tour.Status != TourStatus.Published)
            return new List<KeyPointDto>();

        return tour.KeyPoints
            .OrderBy(kp => kp.Order)
            .Select(_mapper.Map<KeyPointDto>)
            .ToList();
    }
}
