using System.Linq;
using System.Security.Claims;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private long GetCurrentUserId()
        {
            // ?ita 'id' claim koji sadrži UserId (ne personId!)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Unable to determine user ID from token");
            }
            return userId;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetCurrentUserId();
            var notifications = _notificationService.GetByUserId(userId);
            return Ok(notifications);
        }

        [HttpGet("unread")]
        public IActionResult GetUnread()
        {
            var userId = GetCurrentUserId();
            var notifications = _notificationService.GetUnreadByUserId(userId);
            return Ok(notifications);
        }

        [HttpPut("{notificationId}/read")]
        public IActionResult MarkAsRead(long notificationId)
        {
            var notification = _notificationService.MarkAsRead(notificationId);
            return Ok(notification);
        }

        [HttpPut("read-all")]
        public IActionResult MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            _notificationService.MarkAllAsRead(userId);
            return Ok();
        }
    }
}
