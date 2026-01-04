using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Encounters.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/challenges")]
[ApiController]
public class ChallengeController : ControllerBase
{
    private readonly IChallengeService _service;
    public ChallengeController(IChallengeService service) { _service = service; }

    [HttpPost]
    public ActionResult<ChallengeDto> Create([FromBody] CreateChallengeDto dto)
    {
        var personIdClaim = User.FindFirst("id");
        if (personIdClaim == null) return Forbid();
        var personId = long.Parse(personIdClaim.Value);

        var result = _service.CreateChallenge(dto, personId);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet("{id:long}")]
    public ActionResult<ChallengeDto> Get(long id)
    {
        var result = _service.GetById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
