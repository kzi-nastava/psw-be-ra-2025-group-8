using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class BundleService : IBundleService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public BundleService(IBundleRepository bundleRepository, ITourRepository tourRepository, IMapper mapper)
    {
        _bundleRepository = bundleRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public List<BundleDto> GetByAuthor(int authorId)
        => _bundleRepository.GetByAuthor(authorId).Select(ToDto).ToList();

    public BundleDto GetById(long id, int authorId)
    {
        var b = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (b.AuthorId != authorId) throw new ForbiddenException("You can only access your own bundles.");
        return ToDto(b);
    }

    public BundleDto Create(CreateBundleDto dto, int authorId)
    {
        ValidateToursForBundle(authorId, dto.TourIds);

        var bundle = new Bundle(dto.Name, dto.Price, authorId, dto.TourIds);
        var created = _bundleRepository.Create(bundle);
        return ToDto(created);
    }

    public BundleDto Update(long id, UpdateBundleDto dto, int authorId)
    {
        var bundle = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new ForbiddenException("You can only modify your own bundles.");
        if (bundle.Status != BundleStatus.Draft) throw new InvalidOperationException("Only draft bundle can be updated.");

        ValidateToursForBundle(authorId, dto.TourIds);

        bundle.Name = dto.Name;
        bundle.Price = dto.Price;
        bundle.SetTours(dto.TourIds);

        var updated = _bundleRepository.Update(bundle);
        return ToDto(updated);
    }

    public void Delete(long id, int authorId)
    {
        var bundle = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new ForbiddenException("You can only delete your own bundles.");
        if (bundle.Status != BundleStatus.Draft) throw new InvalidOperationException("Published bundle cannot be deleted. Archive it.");

        _bundleRepository.Delete(id);
    }

    public BundleDto Publish(long id, int authorId)
    {
        var bundle = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new ForbiddenException("You can only publish your own bundles.");

        var publishedCount = bundle.BundleTours.Count(bt => bt.Tour?.Status == TourStatus.Published);
        bundle.Publish(publishedCount);

        var updated = _bundleRepository.Update(bundle);
        return ToDto(updated);
    }

    public BundleDto Archive(long id, int authorId)
    {
        var bundle = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new ForbiddenException("You can only archive your own bundles.");

        bundle.Archive();

        var updated = _bundleRepository.Update(bundle);
        return ToDto(updated);
    }

    private void ValidateToursForBundle(int authorId, List<long> tourIds)
    {
        var ids = (tourIds ?? new List<long>()).Distinct().ToList();
        if (ids.Count < 2) throw new ArgumentException("Bundle must contain at least 2 tours.");

        var tours = _tourRepository.GetByIds(ids);

        // 1) sve moraju postojati
        if (tours.Count != ids.Count)
            throw new KeyNotFoundException("Some tours do not exist.");

        // 2) sve moraju biti od autora
        if (tours.Any(t => t.AuthorId != authorId))
            throw new ForbiddenException("You can only add your own tours to bundle.");

        // 3) sve moraju biti PUBLISHED
        if (tours.Any(t => t.Status != TourStatus.Published))
            throw new InvalidOperationException("Only PUBLISHED tours can be added to bundle.");
    }


    private BundleDto ToDto(Bundle bundle) => _mapper.Map<BundleDto>(bundle);
}

