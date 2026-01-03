using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristBundleService : ITouristBundleService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly IMapper _mapper;

    public TouristBundleService(IBundleRepository bundleRepository, IMapper mapper)
    {
        _bundleRepository = bundleRepository;
        _mapper = mapper;
    }

    public List<BundleDto> GetPublished()
        => _bundleRepository.GetPublished().Select(b => _mapper.Map<BundleDto>(b)).ToList();

    public BundleDto GetPublishedById(long id)
    {
        var b = _bundleRepository.Get(id) ?? throw new NotFoundException("Bundle not found.");
        if (b.Status != Core.Domain.BundleStatus.Published) throw new NotFoundException("Bundle not published.");
        return _mapper.Map<BundleDto>(b);
    }
}

