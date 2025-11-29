using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class TourService : ITourService
{
    private readonly ICrudRepository<Tour> _crudRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourService(ICrudRepository<Tour> crudRepository, ITourRepository tourRepository, IMapper mapper)
    {
        _crudRepository = crudRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _crudRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }

    public TourDto Create(TourDto tourDto)
    {
        var tour = new Tour(
            tourDto.Name,
            tourDto.Description,
            tourDto.Difficulty,
            tourDto.Tags ?? new List<string>(),
            tourDto.AuthorId
        );

        var result = _crudRepository.Create(tour);
        return _mapper.Map<TourDto>(result);
    }

    public List<TourDto> GetByAuthor(int authorId)
    {
        var tours = _tourRepository.GetByAuthor(authorId);
        return tours.Select(_mapper.Map<TourDto>).ToList();
    }

    public TourDto Update(TourDto tourDto)
    {
        var existing = _crudRepository.Get(tourDto.Id);

        if (existing.AuthorId != tourDto.AuthorId && tourDto.AuthorId != 0)
        {
            throw new UnauthorizedAccessException("You can only update your own tours.");
        }

        existing.Name = tourDto.Name;
        existing.Description = tourDto.Description;
        existing.Difficulty = tourDto.Difficulty;
        existing.Tags = tourDto.Tags ?? new List<string>();
        existing.Status = Enum.Parse<TourStatus>(tourDto.Status);
        existing.Price = tourDto.Price;

        var result = _crudRepository.Update(existing);
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id, int authorId)
    {
        var tour = _crudRepository.Get(id);
        if (tour.AuthorId != authorId) throw new UnauthorizedAccessException("You can only delete your own tours.");
        if (tour.Status != TourStatus.Draft) throw new InvalidOperationException("Only draft tours can be deleted.");
        _crudRepository.Delete(id);
    }

    // ============================================================
    // Authoring use-cases over Tour aggregate
    // ============================================================

    public TourDto AddKeyPoint(long tourId, KeyPointDto keyPointDto, int authorId)
    {
        // loading aggregate with KeyPoints through ITourRepository
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        // creation of Value Object from DTO and delegation to aggregate root
        var location = new GeoCoordinate(keyPointDto.Latitude, keyPointDto.Longitude);

        tour.AddKeyPoint(
            keyPointDto.Name,
            keyPointDto.Description ?? string.Empty,
            keyPointDto.ImageUrl ?? string.Empty,
            keyPointDto.Secret ?? string.Empty,
            location
        );

        // saving changes through repository
        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto Publish(long tourId, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only publish your own tours.");

        tour.Publish();                   // domen logic inside aggregate root(status, min 2 KeyPoints)

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto Archive(long tourId, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only archive your own tours.");

        tour.Archive();

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }
}
