using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/encounters")]
[ApiController]
public class EncounterAdminController : ControllerBase
{
    private readonly IEncounterService _encounterService;

    public EncounterAdminController(IEncounterService encounterService)
    {
        _encounterService = encounterService;
    }

    // PUT api/administration/encounters/{id}/approve
    [HttpPut("{id:long}/approve")]
    public ActionResult<EncounterDto> Approve(long id)
    {
        var approved = _encounterService.ApproveEncounter(id);
        return Ok(approved);
    }
}
