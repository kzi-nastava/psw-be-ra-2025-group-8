using Explorer.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Services;

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToUser(long userId, string message, object? data = null)
    {
        await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
        {
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendMessageToRoom(string roomId, string message, object? data = null)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", new
        {
            roomId,
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendPrivateMessage(long senderId, long recipientId, string message, object? data = null)
    {
        // Send to recipient
        await _hubContext.Clients.Group($"user_{recipientId}").SendAsync("ReceivePrivateMessage", new
        {
            senderId,
            recipientId,
            message,
            data,
            timestamp = DateTime.UtcNow
        });

        // Send confirmation to sender
        await _hubContext.Clients.Group($"user_{senderId}").SendAsync("MessageSent", new
        {
            senderId,
            recipientId,
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyUserJoined(string roomId, long userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("UserJoined", new
        {
            userId,
            roomId,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyUserLeft(string roomId, long userId)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("UserLeft", new
        {
            userId,
            roomId,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyTyping(string roomId, long userId, bool isTyping)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("UserTyping", new
        {
            userId,
            roomId,
            isTyping,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task BroadcastMessage(string message, object? data = null)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveBroadcast", new
        {
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }
}
