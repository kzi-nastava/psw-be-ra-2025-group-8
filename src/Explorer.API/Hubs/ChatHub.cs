using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Explorer.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<long, string> _userConnections = new();
    private static readonly Dictionary<string, HashSet<long>> _chatRooms = new();

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserIdFromToken();
        if (userId.HasValue)
        {
            _userConnections[userId.Value] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId.Value}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdFromToken();
        if (userId.HasValue && _userConnections.ContainsKey(userId.Value))
        {
            _userConnections.Remove(userId.Value);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId.Value}");
            
            // Remove user from all chat rooms
            foreach (var room in _chatRooms.Where(r => r.Value.Contains(userId.Value)).ToList())
            {
                room.Value.Remove(userId.Value);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Key);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChatRoom(string roomId)
    {
        var userId = GetUserIdFromToken();
        if (!userId.HasValue) return;

        if (!_chatRooms.ContainsKey(roomId))
        {
            _chatRooms[roomId] = new HashSet<long>();
        }

        _chatRooms[roomId].Add(userId.Value);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        
        await Clients.Group(roomId).SendAsync("UserJoined", new
        {
            userId = userId.Value,
            roomId,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task LeaveChatRoom(string roomId)
    {
        var userId = GetUserIdFromToken();
        if (!userId.HasValue) return;

        if (_chatRooms.ContainsKey(roomId))
        {
            _chatRooms[roomId].Remove(userId.Value);
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        
        await Clients.Group(roomId).SendAsync("UserLeft", new
        {
            userId = userId.Value,
            roomId,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendMessageToRoom(string roomId, string message)
    {
        var userId = GetUserIdFromToken();
        if (!userId.HasValue) return;

        await Clients.Group(roomId).SendAsync("ReceiveMessage", new
        {
            senderId = userId.Value,
            roomId,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendPrivateMessage(long recipientId, string message)
    {
        var senderId = GetUserIdFromToken();
        if (!senderId.HasValue) return;

        // Send to recipient
        await Clients.Group($"user_{recipientId}").SendAsync("ReceivePrivateMessage", new
        {
            senderId = senderId.Value,
            recipientId,
            message,
            timestamp = DateTime.UtcNow
        });

        // Send confirmation to sender
        await Clients.Caller.SendAsync("MessageSent", new
        {
            senderId = senderId.Value,
            recipientId,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendTypingIndicator(string roomId, bool isTyping)
    {
        var userId = GetUserIdFromToken();
        if (!userId.HasValue) return;

        await Clients.OthersInGroup(roomId).SendAsync("UserTyping", new
        {
            userId = userId.Value,
            roomId,
            isTyping,
            timestamp = DateTime.UtcNow
        });
    }

    public static string? GetConnectionId(long userId)
    {
        return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
    }

    public static bool IsUserOnline(long userId)
    {
        return _userConnections.ContainsKey(userId);
    }

    public static IEnumerable<long> GetUsersInRoom(string roomId)
    {
        return _chatRooms.TryGetValue(roomId, out var users) ? users : Enumerable.Empty<long>();
    }

    private long? GetUserIdFromToken()
    {
        var idClaim = Context.User?.FindFirst("id")
                   ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)
                   ?? Context.User?.FindFirst("personId")
                   ?? Context.User?.FindFirst("sub");

        if (idClaim != null && long.TryParse(idClaim.Value, out long userId))
        {
            return userId;
        }

        return null;
    }
}
