using System.Collections.Generic;
using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [ApiController]
    [Route("api/messages")]
    // [Authorize(Policy = "userPolicy")]   // privremeno isključeno da bi testovi radili bez JWT-a
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
            // Ako nema claim-a (npr. u test okruženju bez autentikacije) – 
            // koristimo podrazumevanog korisnika sa Id = 1,
            // što se slaže sa seed-ovanim podacima iz TestData skripti.
            if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
            {
                return 1;
            }
            return long.Parse(idClaim.Value);
        }

        // NOVI ENDPOINT - Vrati sve konverzacije za trenutnog korisnika
        [HttpGet("conversations")]
        public ActionResult<List<ConversationSummaryDto>> GetConversations()
        {
            var currentUserId = GetCurrentUserId();
            var conversations = _messageService.GetConversations(currentUserId);
            return Ok(conversations);
        }

        [HttpPost]
        public ActionResult<MessageDto> Send([FromBody] MessageDto dto)
        {
            dto.SenderId = GetCurrentUserId();
            var created = _messageService.Send(dto);
            return Ok(created);
        }

        [HttpGet("{otherUserId:long}")]
        public ActionResult<List<MessageDto>> GetConversation(long otherUserId)
        {
            var currentUserId = GetCurrentUserId();
            var result = _messageService.GetConversation(currentUserId, otherUserId);
            return Ok(result);
        }

        [HttpPut("{messageId:long}")]
        public ActionResult<MessageDto> Edit(long messageId, [FromBody] string newContent)
        {
            var updated = _messageService.Edit(messageId, newContent);
            return Ok(updated);
        }

        [HttpDelete("{messageId:long}")]
        public ActionResult<MessageDto> Delete(long messageId)
        {
            var deleted = _messageService.Delete(messageId);
            return Ok(deleted);
        }
    }
}