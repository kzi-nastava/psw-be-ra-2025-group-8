using AutoMapper;
using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Weather;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristTourService : ITouristTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourRatingService _tourRatingService;
    private readonly IInternalPersonService _profileProvider;
    private readonly IMapper _mapper;
    private readonly IPersonEquipmentRepository _personEquipmentRepository;
    private readonly IPreferenceTagsRepository _preferenceTagsRepository;
    private readonly ITouristPreferencesRepository _touristPreferencesRepository;
    private readonly IInternalSaleService _saleService;
    private readonly IWeatherForecastService _weatherForecastService;



    public TouristTourService(
        ITourRepository tourRepository,
        ITourRatingService tourRatingService,
        IInternalPersonService profileProvider,
        IPersonEquipmentRepository personEquipmentRepository,
        IPreferenceTagsRepository preferenceTagsRepository,
        ITouristPreferencesRepository touristPreferencesRepository,
        IInternalSaleService saleService,
        IMapper mapper,
        IWeatherForecastService weatherForecastService)
    {
        _tourRepository = tourRepository;
        _tourRatingService = tourRatingService;
        _profileProvider = profileProvider;
        _personEquipmentRepository = personEquipmentRepository;
        _preferenceTagsRepository = preferenceTagsRepository;
        _touristPreferencesRepository = touristPreferencesRepository;
        _saleService = saleService;
        _mapper = mapper;
        _weatherForecastService = weatherForecastService;
    }


    public List<TouristTourPreviewDto> GetPublishedTours()
    {
        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published)
            .ToList();

        return tours.Select(MapPreview).ToList();
    }

    public List<TouristTourPreviewDto> GetPublishedTours(int? minPrice, int? maxPrice)
    {
        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published);

        if (minPrice.HasValue)
            tours = tours.Where(t => t.Price >= (decimal)minPrice.Value);

        if (maxPrice.HasValue)
            tours = tours.Where(t => t.Price <= (decimal)maxPrice.Value);

        return tours.ToList().Select(MapPreview).ToList();
    }

    public List<TouristTourPreviewDto> GetPublishedTours(List<int> difficulties, int? minPrice, int? maxPrice)
    {
        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published);

        var set = difficulties.ToHashSet();
        tours = tours.Where(t => set.Contains(t.Difficulty));

        if (minPrice.HasValue)
            tours = tours.Where(t => t.Price >= (decimal)minPrice.Value);

        if (maxPrice.HasValue)
            tours = tours.Where(t => t.Price <= (decimal)maxPrice.Value);

        return tours.ToList().Select(MapPreview).ToList();
    }




    public List<TouristTourPreviewDto> GetPublishedTours(
            long personId,
            bool searchByOwnedEquipment,
            bool searchByPreferenceTags,
            bool searchByPreferenceDifficulty,
            List<int>? difficulties,
            int? minPrice,
            int? maxPrice)
    {
        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published)
            .ToList();

        if (searchByOwnedEquipment)
        {
            var ownedEquipmentIds = _personEquipmentRepository
                .GetByPersonId(personId)
                .Select(pe => pe.EquipmentId)
                .ToHashSet();

            tours = tours
                .Where(t => t.RequiredEquipment.All(req => ownedEquipmentIds.Contains(req.EquipmentId)))
                .ToList();
        }

        if (searchByPreferenceTags)
        {
            var preferenceTagIds = _preferenceTagsRepository
                .GetTagsForPerson(personId)
                .Select(t => t.Id)
                .ToHashSet();

            tours = tours
                .Where(t => t.TourTags.Any(tt => preferenceTagIds.Contains(tt.TagsId)))
                .ToList();
        }

        
        if (searchByPreferenceDifficulty)
        {
            var prefs = _touristPreferencesRepository.GetByPersonId(personId);

            if (prefs == null)
                return new List<TouristTourPreviewDto>();

            var allowed = MapPreferenceDifficultyToStars(prefs.Difficulty);

            tours = tours
                .Where(t => allowed.Contains(t.Difficulty))
                .ToList();
        }

        if (difficulties is { Count: > 0 })
        {
            var set = difficulties.ToHashSet();
            tours = tours.Where(t => set.Contains(t.Difficulty)).ToList();
        }

        if (minPrice.HasValue)
            tours = tours.Where(t => t.Price >= (decimal)minPrice.Value).ToList();

        if (maxPrice.HasValue)
            tours = tours.Where(t => t.Price <= (decimal)maxPrice.Value).ToList();


        return tours.Select(MapPreview).ToList();
    }

    private static HashSet<int> MapPreferenceDifficultyToStars(DifficultyLevel level)
    {
        return level switch
        {
            DifficultyLevel.Beginner => new HashSet<int> { 1, 2 },
            DifficultyLevel.Intermediate => new HashSet<int> { 3, 4 },
            DifficultyLevel.Professional => new HashSet<int> { 5 },
            _ => new HashSet<int>()
        };
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

    public WeatherDailyForecastDto GetPublishedTourDailyForecast(long id, int days = 7)
    {
        var tour = _tourRepository.Get(id);
        if (tour == null || tour.Status != TourStatus.Published)
            return null;

        var kp = tour.KeyPoints.OrderBy(k => k.Order).FirstOrDefault();
        if (kp == null)
            return null;

        return _weatherForecastService.GetDailyForecast(kp.Location.Latitude, kp.Location.Longitude, days);
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

    public List<TouristTourPreviewDto> SearchToursByLocation(TourSearchByLocationDto searchDto)
    {
        var tours = _tourRepository.SearchByLocation(
            searchDto.Latitude,
            searchDto.Longitude,
            searchDto.DistanceInKilometers);

        return tours.Select(MapPreview).ToList();
    }

    public List<TouristTourPreviewDto> GetToursOnSale(bool sortByDiscount = false)
    {
        var activeSales = _saleService.GetActiveSales();
        
        var tourIds = activeSales
            .SelectMany(s => s.TourIds)
            .Distinct()
            .ToList();

        var tours = _tourRepository
            .GetAll()
            .Where(t => t.Status == TourStatus.Published && tourIds.Contains(t.Id))
            .ToList();

        var result = tours.Select(MapPreview).ToList();

        if (sortByDiscount)
        {
            result = result
                .OrderByDescending(t => t.DiscountPercentage ?? 0)
                .ToList();
        }

        return result;
    }

    // =======================================================
    // Private helpers
    // =======================================================

    private TouristTourPreviewDto MapPreview(Tour tour)
    {
        var saleInfo = _saleService.GetTourSaleInfo(tour.Id);
        
        return new TouristTourPreviewDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Price = saleInfo.IsOnSale ? saleInfo.DiscountedPrice.Value : tour.Price,
            Tags = tour.TourTags.Select(tt => tt.Tags.Tag).ToList(),
            RequiredEquipment = tour.RequiredEquipment.Select(eq => eq.Equipment.Name).ToList(),
            FirstKeyPoint = MapFirstKeyPoint(tour),
            AverageRating = CalculateAverage(tour.Id),
            Author = _profileProvider.GetByUserId(tour.AuthorId).Name,
            IsOnSale = saleInfo.IsOnSale,
            OriginalPrice = saleInfo.IsOnSale ? saleInfo.OriginalPrice : null,
            DiscountPercentage = saleInfo.DiscountPercentage
        };
    }

    private TouristTourDetailsDto MapDetails(Tour tour)
    {
        var saleInfo = _saleService.GetTourSaleInfo(tour.Id);
        
        return new TouristTourDetailsDto
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            Difficulty = tour.Difficulty,
            Price = saleInfo.IsOnSale ? saleInfo.DiscountedPrice.Value : tour.Price,
            LengthInKilometers = tour.LengthInKilometers,
            Tags = tour.TourTags.Select(tt => tt.Tags.Tag).ToList(),
            RequiredEquipment = tour.RequiredEquipment.Select(eq => eq.Equipment.Name).ToList(),
            Author = _profileProvider.GetByUserId(tour.AuthorId).Name,
            IsOnSale = saleInfo.IsOnSale,
            OriginalPrice = saleInfo.IsOnSale ? saleInfo.OriginalPrice : null,
            DiscountPercentage = saleInfo.DiscountPercentage
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
}
