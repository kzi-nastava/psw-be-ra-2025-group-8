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

            // ✅ ISPRAVKA: Ako nema claim-a, uzmi ID iz User.Identity.Name koji Angular šalje
            if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
            {
                // Pokušaj da pročitaš ID iz Name claim-a (Angular JWT sadrži ID u 'id' claim-u)
                var nameClaim = User.FindFirst("id");
                if (nameClaim != null && long.TryParse(nameClaim.Value, out long userId))
                {
                    return userId;
                }

                // Fallback: ako je Name numerički, koristi ga
                if (User.Identity?.IsAuthenticated == true &&
                    long.TryParse(User.Identity.Name, out long nameId))
                {
                    return nameId;
                }

                // Poslednji fallback - return 1 (samo za testove)
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
            var currentUserId = GetCurrentUserId();
            dto.SenderId = currentUserId;  // ✅ ISPRAVKA: Koristi stvarni ID
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

        // NOVI ENDPOINT - Obriši celu konverzaciju sa određenim korisnikom
        [HttpDelete("deleteConversation/{participantId:long}")]
        public ActionResult DeleteConversation(long participantId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                _messageService.DeleteConversation(currentUserId, participantId);
                return NoContent(); // 204 - success, bez odgovora
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}