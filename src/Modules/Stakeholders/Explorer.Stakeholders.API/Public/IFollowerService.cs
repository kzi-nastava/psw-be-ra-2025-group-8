using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IFollowerService
{
    // Search users
    List<UserSearchResultDto> SearchUsers(string searchTerm, long currentUserId);
    
    // Follow/Unfollow
    FollowerDto Follow(long userId, long followingUserId);
    void Unfollow(long userId, long followingUserId);
    void RemoveFollower(long userId, long followerUserId);
    
    // Get followers/following
    List<FollowerDto> GetFollowers(long userId);
    List<FollowerDto> GetFollowing(long userId);
    
    // Send message to all followers
    void SendMessageToFollowers(long senderId, SendFollowerMessageDto messageDto);
    
    // Get notifications
    List<NotificationDto> GetNotifications(long userId);
    List<NotificationDto> GetUnreadNotifications(long userId);
    void MarkNotificationAsRead(long notificationId);
}
