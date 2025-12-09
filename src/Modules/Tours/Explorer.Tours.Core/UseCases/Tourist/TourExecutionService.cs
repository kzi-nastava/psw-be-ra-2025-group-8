using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using static Explorer.Tours.Core.Domain.TourExecution;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ICrudRepository<TourExecution> _crudRepository;
    private readonly IKeyPointRepository _keyPointRepository;
    private readonly IKeyPointReachedRepository _keyPointReachedRepository;
    private readonly IMapper _mapper;

    // Proximity threshold (meters)
    private const double KEYPOINT_PROXIMITY_METERS = 60.0;

    public TourExecutionService(
        ITourExecutionRepository tourExecutionRepository,
        ICrudRepository<TourExecution> crudRepository,
        IKeyPointRepository keyPointRepository,
        IKeyPointReachedRepository keyPointReachedRepository,
        IMapper mapper)
    {
        _tourExecutionRepository = tourExecutionRepository;
        _crudRepository = crudRepository;
        _keyPointRepository = keyPointRepository;
        _keyPointReachedRepository = keyPointReachedRepository;
        _mapper = mapper;
    }

    public PagedResult<TourExecutionDto> GetPaged(int page, int pageSize)
    {
        var result = _crudRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<TourExecutionDto>).ToList();
        return new PagedResult<TourExecutionDto>(items, result.TotalCount);
    }

    public TourExecutionDto Get(int id)
    {
        var tourExecution = _tourExecutionRepository.Get(id);
        return tourExecution != null ? _mapper.Map<TourExecutionDto>(tourExecution) : null;
    }

    public TourExecutionDto Create(TourExecutionDto tourExecutionDto)
    {
        // Validation: Check if tourist already has an active TourExecution
        var activeTourExecutions = _tourExecutionRepository.GetByTourist(tourExecutionDto.IdTourist)
            .Where(te => te.Status == TourExecutionStatus.InProgress) // Only InProgress tours are considered active
            .ToList();

        if (activeTourExecutions.Any())
        {
            throw new ArgumentException(
                $"Tourist {tourExecutionDto.IdTourist} already has an active tour execution (ID: {activeTourExecutions.First().Id}). " +
                "Please complete or abandon the current tour before starting a new one.");
        }

        var status = Enum.Parse<TourExecutionStatus>(tourExecutionDto.Status);
        var tourExecution = new TourExecution(
            tourExecutionDto.IdTour,
            tourExecutionDto.Longitude,
            tourExecutionDto.Latitude,
            tourExecutionDto.CompletionPercentage,
            tourExecutionDto.IdTourist,
            status,
            tourExecutionDto.CompletionPercentage
        );

        var result = _tourExecutionRepository.Create(tourExecution);
        return _mapper.Map<TourExecutionDto>(result);
    }

    public TourExecutionDto Update(TourExecutionDto tourExecutionDto)
    {
        var existing = _tourExecutionRepository.Get(tourExecutionDto.Id);
        if (existing == null)
            throw new KeyNotFoundException($"TourExecution with id {tourExecutionDto.Id} not found.");

        existing.UpdatePosition(tourExecutionDto.Longitude, tourExecutionDto.Latitude);
        existing.UpdateStatus(Enum.Parse<TourExecutionStatus>(tourExecutionDto.Status));
        existing.UpdateCompletionPercentage(tourExecutionDto.CompletionPercentage);

        var result = _tourExecutionRepository.Update(existing);
        return _mapper.Map<TourExecutionDto>(result);
    }

    public TourExecutionDto GetByTouristAndTour(int touristId, int tourId)
    {
        var tourExecution = _tourExecutionRepository.GetByTouristAndTour(touristId, tourId);
        return tourExecution != null ? _mapper.Map<TourExecutionDto>(tourExecution) : null;
    }

    public List<TourExecutionDto> GetByTourist(int touristId)
    {
        var executions = _tourExecutionRepository.GetByTourist(touristId);
        return executions.Select(_mapper.Map<TourExecutionDto>).ToList();
    }

    public List<TourExecutionDto> GetByTour(int tourId)
    {
        var executions = _tourExecutionRepository.GetByTour(tourId);
        return executions.Select(_mapper.Map<TourExecutionDto>).ToList();
    }

    public void Delete(int id)
    {
        _tourExecutionRepository.Delete(id);
    }

    public CheckKeyPointResponseDto CheckKeyPoint(CheckKeyPointRequestDto request)
    {
        // Load TourExecution (throws NotFoundException in repository if missing)
        var tourExecution = _crudRepository.Get(request.TourExecutionId);

        // Always update LastActivity
        tourExecution.LastActivity = DateTime.UtcNow;
        _crudRepository.Update(tourExecution);

        // Get all keypoints for this tour (ordered by Order)
        var keyPoints = _keyPointRepository.GetByTour(tourExecution.IdTour);
        if (!keyPoints.Any())
        {
            return new CheckKeyPointResponseDto
                {
                    KeyPointReached = false,
                    LastActivity = tourExecution.LastActivity
                };
         }

        // Already reached orders for this execution
        var reachedOrders = _keyPointReachedRepository.GetReachedKeyPointOrders(request.TourExecutionId)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        // Find first not reached keypoint in order (Order property instead of OrderNum)
        var nextKeyPoint = keyPoints.FirstOrDefault(kp => !reachedOrders.Contains(kp.Order));
        if (nextKeyPoint == null)
        {
            return new CheckKeyPointResponseDto
            {
                KeyPointReached = false,
                LastActivity = tourExecution.LastActivity
            };
        }

        // Calculate distance using Haversine (now using Location.Latitude and Location.Longitude)
        var distance = CalculateDistance(
        request.Latitude, request.Longitude,
        nextKeyPoint.Location.Latitude, nextKeyPoint.Location.Longitude);

        if (distance <= KEYPOINT_PROXIMITY_METERS)
        {
            var keyPointReached = new KeyPointReached(
                request.TourExecutionId,
                nextKeyPoint.Order,  // Use Order instead of OrderNum
                nextKeyPoint.Location.Latitude,
                nextKeyPoint.Location.Longitude
            );

            _keyPointReachedRepository.Create(keyPointReached);

            return new CheckKeyPointResponseDto
                {
                    KeyPointReached = true,
                    KeyPointOrder = nextKeyPoint.Order,  // Use Order instead of OrderNum
                    ReachedAt = keyPointReached.ReachedAt,
                    LastActivity = tourExecution.LastActivity
                };
        }

        return new CheckKeyPointResponseDto
        {
            KeyPointReached = false,
            LastActivity = tourExecution.LastActivity
        };
    }

    public List<KeyPointReachedDto> GetReachedKeyPoints(long tourExecutionId)
    {
        var reachedKeyPoints = _keyPointReachedRepository.GetByTourExecution(tourExecutionId);
        return reachedKeyPoints.Select(_mapper.Map<KeyPointReachedDto>).ToList();
    }

    public KeyPointSecretDto GetKeyPointSecret(long tourExecutionId, int keyPointOrder)
    {
        // 1. Load TourExecution
        var tourExecution = _crudRepository.Get(tourExecutionId);
        if (tourExecution == null)
            throw new KeyNotFoundException($"TourExecution with id {tourExecutionId} not found.");

        // 2. Check if KeyPoint has been reached
        var reachedKeyPoints = _keyPointReachedRepository.GetByTourExecution(tourExecutionId);
        var keyPointReached = reachedKeyPoints.FirstOrDefault(kpr => kpr.KeyPointOrder == keyPointOrder);
        
        if (keyPointReached == null)
            throw new InvalidOperationException($"KeyPoint {keyPointOrder} has not been reached yet. Complete the keypoint to unlock the secret.");

        // 3. Get KeyPoint with secret
        var keyPoint = _keyPointRepository.GetByTourAndOrder(tourExecution.IdTour, keyPointOrder);
        if (keyPoint == null)
            throw new KeyNotFoundException($"KeyPoint with order {keyPointOrder} not found for this tour.");

        // 4. Return secret
        return new KeyPointSecretDto
        {
            Order = keyPointOrder,
            Secret = keyPoint.Secret,
            UnlockedAt = keyPointReached.ReachedAt
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // meters
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}