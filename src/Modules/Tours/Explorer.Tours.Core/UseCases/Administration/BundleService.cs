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
        ValidateTourOwnership(authorId, dto.TourIds);

        var bundle = new Bundle(dto.Name, dto.Price, authorId, dto.TourIds);
        var created = _bundleRepository.Create(bundle);
        return ToDto(created);
    }

    public BundleDto Update(long id, UpdateBundleDto dto, int authorId)
    {
        var bundle = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new ForbiddenException("You can only modify your own bundles.");
        if (bundle.Status != BundleStatus.Draft) throw new InvalidOperationException("Only draft bundle can be updated.");

        ValidateTourOwnership(authorId, dto.TourIds);

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

    private void ValidateTourOwnership(int authorId, List<long> tourIds)
    {
        var ids = tourIds?.Distinct().ToList() ?? new List<long>();
        if (ids.Count < 2) throw new ArgumentException("Bundle must contain at least 2 tours.");

        // Uzimamo samo ture autora i proverimo da li su SVE iz liste njegove
        var myTours = _tourRepository.GetByAuthor(authorId);
        var myIds = myTours.Select(t => t.Id).ToHashSet();

        if (ids.Any(id => !myIds.Contains(id)))
            throw new ForbiddenException("You can only add your own tours to bundle.");
    }

    private BundleDto ToDto(Bundle bundle) => _mapper.Map<BundleDto>(bundle);
}

