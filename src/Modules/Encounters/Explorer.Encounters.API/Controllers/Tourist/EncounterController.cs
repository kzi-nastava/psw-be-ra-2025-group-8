using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Encounters.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/encounters")]
[ApiController]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _service;

    public EncounterController(IEncounterService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<EncounterDto> Create([FromBody] CreateEncounterDto dto)
    {
        var personIdClaim = User.FindFirst("id");
        if (personIdClaim == null) return Forbid();
        if (!long.TryParse(personIdClaim.Value, out var personId)) return Forbid();

        var createDto = new EncounterDto
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Type = dto.Type,
            XPReward = dto.XPReward,
            Status = "Draft",
            CreatorPersonId = personId,
            SocialRequiredCount = dto.SocialRequiredCount,
            SocialRangeMeters = dto.SocialRangeMeters
        };

        var created = _service.CreateEncounter(createDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:long}")]
    public ActionResult<EncounterDto> GetById(long id)
    {
        var result = _service.GetEncounterById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
