using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.User;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/clubs/{clubId}/messages")]
[ApiController]
public class ClubMessagesController : ControllerBase
{
    private readonly IClubMessageService _clubMessageService;

    public ClubMessagesController(IClubMessageService clubMessageService)
    {
        _clubMessageService = clubMessageService;
    }

    // POST /api/tourist/clubs/{clubId}/messages
    [HttpPost]
    public ActionResult<ClubMessageDto> PostMessage(long clubId, [FromBody] CreateClubMessageDto dto)
    {
        try
        {
            var userId = ExtractUserId();
            dto.ClubId = clubId; // Set from route parameter
            var result = _clubMessageService.PostMessage(dto, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // GET /api/tourist/clubs/{clubId}/messages
    [HttpGet]
    [AllowAnonymous] // Members can view messages
    public ActionResult<IEnumerable<ClubMessageDto>> GetClubMessages(long clubId)
    {
        var messages = _clubMessageService.GetClubMessages(clubId);
        return Ok(messages);
    }

    // PUT /api/tourist/clubs/{clubId}/messages/{messageId}
    [HttpPut("{messageId}")]
    public ActionResult<ClubMessageDto> UpdateMessage(long clubId, long messageId, [FromBody] UpdateClubMessageDto dto)
    {
        try
        {
            var userId = ExtractUserId();
            var result = _clubMessageService.UpdateMessage(messageId, dto, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE /api/tourist/clubs/{clubId}/messages/{messageId}
    [HttpDelete("{messageId}")]
    public ActionResult DeleteMessage(long clubId, long messageId)
    {
        try
        {
            var userId = ExtractUserId();
            _clubMessageService.DeleteMessage(messageId, userId);
            return NoContent(); // RESTful standard for successful DELETE
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private long ExtractUserId()
    {
        var claim = User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "id" ||
            c.Type == "sub" ||
            c.Type == "personId"
        );

        if (claim == null || !long.TryParse(claim.Value, out var userId))
            throw new UnauthorizedAccessException("User id claim is missing");

        return userId;
    }
}
