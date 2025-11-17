using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [ApiController]
    [Route("api/messages")]
    //[Authorize(Policy = "userPolicy")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return idClaim != null ? long.Parse(idClaim.Value) : 0;
        }

        [HttpPost]
        public IActionResult SendMessage([FromBody] MessageDto dto)
        {
            var userId = GetCurrentUserId();
            dto.SenderId = userId;
            var result = _messageService.Send(dto);
            return Ok(result);
        }

        [HttpGet("{otherUserId:long}")]
        public IActionResult GetConversation(long otherUserId)
        {
            var userId = GetCurrentUserId();
            var result = _messageService.GetConversation(userId, otherUserId);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public IActionResult EditMessage(long id, [FromBody] MessageDto dto)
        {
            var result = _messageService.Edit(id, dto.Content);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public IActionResult DeleteMessage(long id)
        {
            _messageService.Delete(id);
            return Ok();
        }
    }
}
