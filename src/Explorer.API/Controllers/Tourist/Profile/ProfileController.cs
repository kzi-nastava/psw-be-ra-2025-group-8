using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist.Profile;

[Authorize(Policy = "personPolicy")]
[Route("api/profile")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IPersonService _personService;

    public ProfileController(IPersonService personService)
    {
        _personService = personService;
    }

    private bool TryGetUserId(out long userId)
    {
        userId = 0;
        var claim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null) return false;
        return long.TryParse(claim.Value, out userId);
    }

    private bool TryGetPersonId(out long personId)
    {
        personId = 0;
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
        if (claim == null) return false;
        return long.TryParse(claim.Value, out personId);
    }

    [HttpGet]
    public ActionResult<PersonDto> GetProfile()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var profile = _personService.GetByUserId(userId);
        return Ok(profile);
    }

    [HttpPut]
    public ActionResult<PersonDto> UpdateProfile([FromBody] UpdatePersonDto dto)
    {
        if (!TryGetPersonId(out var personId))
            return Unauthorized();

        var updatedProfile = _personService.UpdateProfile(personId, dto);
        return Ok(updatedProfile);
    }
}
