using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using AutoMapper;

namespace Explorer.Tours.Core.UseCases;

public class InternalTourService : IInternalTourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IKeyPointRepository _keyPointRepository;
    private readonly IMapper _mapper;

    public InternalTourService(ITourRepository tourRepository, IKeyPointRepository keyPointRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _keyPointRepository = keyPointRepository;
        _mapper = mapper;
    }

    public string GetTourNameById(int tourId)
    {
        var tour = _tourRepository.Get(tourId);
        return tour?.Name ?? $"Tour#{tourId}";
    }

    public string GetKeyPointNameByTourAndOrder(int tourId, int order)
    {
        var keyPoint = _keyPointRepository.GetByTourAndOrder(tourId, order);
        return keyPoint?.Name ?? $"KeyPoint#{order}";
    }

    public KeyPointDto GetKeyPointByTourAndOrder(int tourId, int order)
    {
        var keyPoint = _keyPointRepository.GetByTourAndOrder(tourId, order);
        return keyPoint != null ? _mapper.Map<KeyPointDto>(keyPoint) : null;
    }
}
