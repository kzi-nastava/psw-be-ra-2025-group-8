using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
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
    private readonly ITagsRepository _tagsRepository;
    private readonly ICrudRepository<Equipment> _equipmentRepository;

    public TourService(
        ICrudRepository<Tour> crudRepository,
        ITourRepository tourRepository,
        ITagsRepository tagsRepository,
        ICrudRepository<Equipment> equipmentRepository,
        IMapper mapper)
    {
        _crudRepository = crudRepository;
        _tourRepository = tourRepository;
        _tagsRepository = tagsRepository;
        _equipmentRepository = equipmentRepository;
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

    public TourDto Reactivate(long tourId, int authorId)  
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only reactivate your own tours.");

        tour.Reactivate();

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }


    public TourDto AddEquipment(long tourId, long equipmentId, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        // domen logika unutar agregata
        tour.AddRequiredEquipment(equipmentId);

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto RemoveEquipment(long tourId, long equipmentId, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        tour.RemoveRequiredEquipment(equipmentId);

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    // TOUR TAGS MANAGEMENT
    public TourDto AddTag(long tourId, string tag, int authorId)
    {
        var tour = _tourRepository.Get(tourId)
            ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag name cannot be empty.");

        // normalize
        string normalized = tag.Trim().ToLowerInvariant();

        // find or create tag
        var existingTag = _tagsRepository.GetByName(normalized);
        if (existingTag == null)
        {
            existingTag = _tagsRepository.Create(new Tags(tag.Trim()));
        }

        // add to tour domain logic
        try
        {
            tour.AddTag(existingTag.Id);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto RemoveTag(long tourId, string tag, int authorId)
    {
        var tour = _tourRepository.Get(tourId)
            ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Tag name cannot be empty.");

        string normalized = tag.Trim().ToLowerInvariant();
        var existingTag = _tagsRepository.GetByName(normalized);

        if (existingTag == null)
            return _mapper.Map<TourDto>(tour);  // tag ne postoji → nema šta da se briše

        tour.RemoveTag(existingTag.Id);

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto UpdateTags(long tourId, List<string> tags, int authorId)
    {
        var tour = _tourRepository.Get(tourId)
            ?? throw new KeyNotFoundException("Tour not found.");

        if (tour.AuthorId != authorId)
            throw new UnauthorizedAccessException("You can only modify your own tours.");

        if (tags == null) tags = new List<string>();

        // normalize input list
        var normalizedInput = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        // current tags on tour
        var currentTagIds = tour.TourTags.Select(tt => tt.TagsId).ToList();

        // fetch all needed tags from DB or create missing ones
        var tagEntities = new Dictionary<string, Tags>();

        foreach (var normalized in normalizedInput)
        {
            var tagEntity = _tagsRepository.GetByName(normalized);
            if (tagEntity == null)
                tagEntity = _tagsRepository.Create(new Tags(normalized));

            tagEntities[normalized] = tagEntity;
        }

        // REMOVE tags that are no longer in the input list
        foreach (var tt in tour.TourTags.ToList())
        {
            var relatedTag = tagEntities.Values.FirstOrDefault(x => x.Id == tt.TagsId);
            if (relatedTag == null)
            {
                tour.RemoveTag(tt.TagsId);
            }
        }

        // ADD missing tags
        foreach (var tagEntity in tagEntities.Values)
        {
            tour.AddTag(tagEntity.Id);
        }

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public List<EquipmentForTourDto> GetEquipmentForTour(long tourId, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");
        if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

        var allEquipment = _equipmentRepository.GetPaged(0, int.MaxValue).Results;
        var requiredIds = tour.RequiredEquipment.Select(e => e.EquipmentId).ToList();

        return allEquipment.Select(e => new EquipmentForTourDto
        {
            EquipmentId = e.Id,
            Name = e.Name,
            Description = e.Description,
            IsRequired = requiredIds.Contains(e.Id)
        }).ToList();
    }

    public TourDto UpdateEquipment(long tourId, List<long> equipmentIds, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");
        if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

        tour.RequiredEquipment.Clear(); // reset

        foreach (var eqId in equipmentIds.Distinct())
            tour.AddRequiredEquipment(eqId); // koristi već postojeću domen logiku!

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

    public TourDto UpdateTransportTimes(long tourId, List<TourTransportTimeDto> times, int authorId)
    {
        var tour = _tourRepository.Get(tourId) ?? throw new KeyNotFoundException("Tour not found.");
        if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

        // Delete existing transport times and add new ones
        tour.ClearTransportTimes();

        if (times != null)
        {
            foreach (var dto in times)
            {
                if (!Enum.TryParse<TransportType>(dto.Transport, true, out var transport))
                {
                    throw new EntityValidationException($"Invalid transport value: '{dto.Transport}'.");
                }

                if (dto.DurationMinutes <= 0)
                {
                    throw new EntityValidationException("Duration must be a positive number of minutes.");
                }

                tour.SetTransportTime(transport, dto.DurationMinutes);
            }
        }

        var updated = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(updated);
    }

}
