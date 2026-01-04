using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Encounters.API.Controllers.Administrator;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/encounters")]
[ApiController]
public class EncounterAdminController : ControllerBase
{
    private readonly IEncounterService _service;
    public EncounterAdminController(IEncounterService service) { _service = service; }

    [HttpPut("{id:long}/approve")]
    public ActionResult<EncounterDto> Approve(long id)
    {
        var adminIdClaim = User.FindFirst("id");
        if (adminIdClaim == null) return Forbid();

        var result = _service.PublishEncounter(id);
        return Ok(result);
    }
}
