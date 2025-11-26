using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface IObjectService
{
    PagedResult<MonumentDto> GetActiveMonuments(int page, int pageSize);
    PagedResult<FacilityDto> GetActiveFacilities(int page, int pageSize);
}