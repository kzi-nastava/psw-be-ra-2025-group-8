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
    private readonly IMapper _mapper;

    public TourExecutionService(ITourExecutionRepository tourExecutionRepository, ICrudRepository<TourExecution> crudRepository, IMapper mapper)
    {
        _tourExecutionRepository = tourExecutionRepository;
        _crudRepository = crudRepository;
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
}