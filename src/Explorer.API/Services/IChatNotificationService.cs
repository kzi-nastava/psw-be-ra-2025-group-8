namespace Explorer.API.Services;

public interface IChatNotificationService
{
    Task SendMessageToUser(long userId, string message, object? data = null);
    Task SendMessageToRoom(string roomId, string message, object? data = null);
    Task SendPrivateMessage(long senderId, long recipientId, string message, object? data = null);
    Task NotifyUserJoined(string roomId, long userId);
    Task NotifyUserLeft(string roomId, long userId);
    Task NotifyTyping(string roomId, long userId, bool isTyping);
    Task BroadcastMessage(string message, object? data = null);
}
