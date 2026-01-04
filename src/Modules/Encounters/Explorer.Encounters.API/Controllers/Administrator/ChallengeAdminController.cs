using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Encounters.API.Controllers.Administrator;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/challenges")]
[ApiController]
public class ChallengeAdminController : ControllerBase
{
    private readonly IChallengeService _service;
    public ChallengeAdminController(IChallengeService service) { _service = service; }

    [HttpGet]
    public ActionResult<List<ChallengeDto>> GetPending()
    {
        return Ok(_service.GetPendingChallenges());
    }

    [HttpPut("{id:long}/approve")]
    public ActionResult<ChallengeDto> Approve(long id)
    {
        var adminId = long.Parse(User.FindFirst("id").Value);
        var result = _service.ApproveChallenge(id, adminId);
        return Ok(result);
    }
}
