using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class ObjectService : IObjectService
{
    private readonly IMonumentRepository _monumentRepository;
    private readonly ICrudRepository<Facility> _facilityRepository;
    private readonly IMapper _mapper;

    public ObjectService(IMonumentRepository monumentRepository, ICrudRepository<Facility> facilityRepository, IMapper mapper)
    {
        _monumentRepository = monumentRepository;
        _facilityRepository = facilityRepository;
        _mapper = mapper;
    }

    public PagedResult<MonumentDto> GetActiveMonuments(int page, int pageSize)
    {
        // Dohvati sve spomenike sa paginacijom
        var monuments = _monumentRepository.GetPaged(page, pageSize);
        
        // Filtriraj samo aktivne spomenike
        var activeMonuments = monuments.Results
            .Where(m => m.Status == MonumentStatus.Active)
            .ToList();
        
        // Mapiranje na DTO
        var dtos = activeMonuments.Select(_mapper.Map<MonumentDto>).ToList();
        
        return new PagedResult<MonumentDto>(dtos, activeMonuments.Count);
    }

    public PagedResult<FacilityDto> GetActiveFacilities(int page, int pageSize)
    {
        var facilities = _facilityRepository.GetPaged(page, pageSize);
        var dtos = facilities.Results.Select(_mapper.Map<FacilityDto>).ToList();
        return new PagedResult<FacilityDto>(dtos, facilities.TotalCount);
    }
}