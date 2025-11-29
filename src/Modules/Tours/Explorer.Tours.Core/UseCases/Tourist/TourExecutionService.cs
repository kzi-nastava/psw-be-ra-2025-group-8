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
    private const double KEYPOINT_PROXIMITY_METERS = 50.0; // Radius u metrima za proveru blizine

    public TourExecutionService(ITourExecutionRepository tourExecutionRepository, ICrudRepository<TourExecution> crudRepository, IKeyPointRepository keyPointRepository, IKeyPointReachedRepository keyPointReachedRepository,IMapper mapper)
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
        var status = Enum.Parse<TourExecutionStatus>(tourExecutionDto.Status);
        var tourExecution = new TourExecution(
        tourExecutionDto.IdTour,
        tourExecutionDto.Longitude,
        tourExecutionDto.Latitude,
        tourExecutionDto.IdTourist,
        status
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
        var tourExecution = _crudRepository.Get(request.TourExecutionId);
        if (tourExecution == null)
        throw new KeyNotFoundException($"TourExecution with id {request.TourExecutionId} not found.");

        tourExecution.LastActivity = DateTime.UtcNow;
        _crudRepository.Update(tourExecution);
    
        var keyPoints = _keyPointRepository.GetByTour(tourExecution.IdTour);

        if (!keyPoints.Any())
        {
            return new CheckKeyPointResponseDto
            {
                KeyPointReached = false,
                LastActivity = tourExecution.LastActivity
            };
        }

        var reachedKeyPointOrders = _keyPointReachedRepository.GetReachedKeyPointOrders(request.TourExecutionId);

        var nextKeyPoint = keyPoints.FirstOrDefault(kp => !reachedKeyPointOrders.Contains(kp.OrderNum));
        
        if (nextKeyPoint == null)
        {
            return new CheckKeyPointResponseDto
            {
                KeyPointReached = false,
                 LastActivity = tourExecution.LastActivity
            };
        }

        var distance = CalculateDistance(
            request.Latitude, request.Longitude,
            nextKeyPoint.Latitude, nextKeyPoint.Longitude
        );

        if (distance <= KEYPOINT_PROXIMITY_METERS)
        {
            var keyPointReached = new KeyPointReached(
                request.TourExecutionId,
                nextKeyPoint.OrderNum,
                nextKeyPoint.Latitude,
                nextKeyPoint.Longitude
            );

            _keyPointReachedRepository.Create(keyPointReached);

            return new CheckKeyPointResponseDto
            {
                KeyPointReached = true,
                KeyPointOrder = nextKeyPoint.OrderNum,
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

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Radijus Zemlje u metrima
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Vraća udaljenost u metrima
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}