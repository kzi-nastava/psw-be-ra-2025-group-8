using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[ApiController]
[Route("api/tourist/objects")]
[AllowAnonymous]  // Dozvoljava pristup bez autentifikacije
public class ObjectController : ControllerBase
{
    private readonly IObjectService _objectService;

    public ObjectController(IObjectService objectService)
    {
        _objectService = objectService;
    }

    [HttpGet("monuments")]
    public ActionResult<PagedResult<MonumentDto>> GetPublicMonuments([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _objectService.GetActiveMonuments(page, pageSize);
        return Ok(result);
    }

    
     [HttpGet("facilities")]
     public ActionResult<PagedResult<FacilityDto>> GetPublicFacilities([FromQuery] int page, [FromQuery] int pageSize)
     {
         var result = _objectService.GetActiveFacilities(page, pageSize);
         return Ok(result);
     }
}