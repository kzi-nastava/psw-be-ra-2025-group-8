using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.User
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/club-messages")]
    [ApiController]
    public class ClubMessagesController : ControllerBase
    {
        private readonly IClubMessageService _clubMessageService;

        public ClubMessagesController(IClubMessageService clubMessageService)
        {
            _clubMessageService = clubMessageService;
        }

        [HttpPost]
        public Task<ActionResult<ClubMessageDto>> PostMessage([FromBody] CreateClubMessageDto dto)
        {
            long userId = ExtractUserId();
            var result = _clubMessageService.PostMessage(dto, userId);
            return Task.FromResult<ActionResult<ClubMessageDto>>(Ok(result));
        }

        [HttpGet("club/{clubId:long}")]
        [AllowAnonymous]
        public Task<ActionResult<IEnumerable<ClubMessageDto>>> GetClubMessages(long clubId)
        {
            var messages = _clubMessageService.GetClubMessages(clubId);
            return Task.FromResult<ActionResult<IEnumerable<ClubMessageDto>>>(Ok(messages));
        }

        [HttpPut("{messageId:long}")]
        public Task<ActionResult<ClubMessageDto>> UpdateMessage(long messageId, [FromBody] UpdateClubMessageDto dto)
        {
            long userId = ExtractUserId();
            var result = _clubMessageService.UpdateMessage(messageId, dto, userId);
            return Task.FromResult<ActionResult<ClubMessageDto>>(Ok(result));
        }

        [HttpDelete("{messageId:long}")]
        public ActionResult DeleteMessage(long messageId)
        {
            long userId = ExtractUserId();
            _clubMessageService.DeleteMessage(messageId, userId);
            return NoContent();
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
}
