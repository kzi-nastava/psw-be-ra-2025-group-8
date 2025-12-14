using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist.Profile;

[Authorize]
[Route("api/followers")]
[ApiController]
public class FollowerController : ControllerBase
{
private readonly IFollowerService _followerService;

    public FollowerController(IFollowerService followerService)
    {
        _followerService = followerService;
    }

    // POST /api/followers/{followingUserId}
    [HttpPost("{followingUserId}")]
    public ActionResult<FollowerDto> Follow(long followingUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = _followerService.Follow(userId, followingUserId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE /api/followers/{followingUserId}
    [HttpDelete("{followingUserId}")]
    public ActionResult Unfollow(long followingUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            _followerService.Unfollow(userId, followingUserId);
            return Ok(new { message = "Unfollowed successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // GET /api/followers/my-followers
    [HttpGet("my-followers")]
    public ActionResult<List<FollowerDto>> GetMyFollowers()
    {
        var userId = GetCurrentUserId();
        var followers = _followerService.GetFollowers(userId);
        return Ok(followers);
    }

    // GET /api/followers/my-following
    [HttpGet("my-following")]
    public ActionResult<List<FollowerDto>> GetMyFollowing()
    {
        var userId = GetCurrentUserId();
        var following = _followerService.GetFollowing(userId);
        return Ok(following);
    }

    // POST /api/followers/send-message
    [HttpPost("send-message")]
    public ActionResult SendMessageToFollowers([FromBody] SendFollowerMessageDto messageDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            _followerService.SendMessageToFollowers(userId, messageDto);
            return Ok(new { message = "Message sent to all followers successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/followers/notifications
    [HttpGet("notifications")]
    public ActionResult<List<NotificationDto>> GetNotifications()
    {
        var userId = GetCurrentUserId();
        var notifications = _followerService.GetNotifications(userId);
        return Ok(notifications);
    }

    // GET /api/followers/notifications/unread
    [HttpGet("notifications/unread")]
    public ActionResult<List<NotificationDto>> GetUnreadNotifications()
    {
        var userId = GetCurrentUserId();
        var notifications = _followerService.GetUnreadNotifications(userId);
        return Ok(notifications);
    }

    // PUT /api/followers/notifications/{notificationId}/read
    [HttpPut("notifications/{notificationId}/read")]
    public ActionResult MarkNotificationAsRead(long notificationId)
    {
        try
        {
            _followerService.MarkNotificationAsRead(notificationId);
            return Ok(new { message = "Notification marked as read" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    private long GetCurrentUserId()
    {
        var idClaim = User.FindFirst("id")
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("personId")
                    ?? User.FindFirst("sub");

        if (idClaim != null && long.TryParse(idClaim.Value, out long userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Unable to determine user ID from token");
    }
}
