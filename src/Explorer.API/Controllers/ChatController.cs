using Explorer.API.Hubs;
using Explorer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/chat")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IChatNotificationService _chatService;

    public ChatController(IChatNotificationService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("send-to-user/{userId}")]
    public async Task<IActionResult> SendToUser(long userId, [FromBody] ChatMessageRequest request)
    {
        await _chatService.SendMessageToUser(userId, request.Message, request.Data);
        return Ok(new { success = true, message = "Message sent to user" });
    }

    [HttpPost("send-to-room/{roomId}")]
    public async Task<IActionResult> SendToRoom(string roomId, [FromBody] ChatMessageRequest request)
    {
        await _chatService.SendMessageToRoom(roomId, request.Message, request.Data);
        return Ok(new { success = true, message = "Message sent to room" });
    }

    [HttpPost("send-private/{recipientId}")]
    public async Task<IActionResult> SendPrivate(long recipientId, [FromBody] ChatMessageRequest request)
    {
        var senderId = GetUserIdFromToken();
        await _chatService.SendPrivateMessage(senderId, recipientId, request.Message, request.Data);
        return Ok(new { success = true, message = "Private message sent" });
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] ChatMessageRequest request)
    {
        await _chatService.BroadcastMessage(request.Message, request.Data);
        return Ok(new { success = true, message = "Broadcast sent to all users" });
    }

    [HttpGet("user-status/{userId}")]
    public IActionResult GetUserStatus(long userId)
    {
        var isOnline = ChatHub.IsUserOnline(userId);
        var connectionId = ChatHub.GetConnectionId(userId);
        
        return Ok(new
        {
            userId,
            isOnline,
            connectionId,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("room-users/{roomId}")]
    public IActionResult GetRoomUsers(string roomId)
    {
        var users = ChatHub.GetUsersInRoom(roomId);
        
        return Ok(new
        {
            roomId,
            users,
            userCount = users.Count(),
            timestamp = DateTime.UtcNow
        });
    }

    private long GetUserIdFromToken()
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

public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}
