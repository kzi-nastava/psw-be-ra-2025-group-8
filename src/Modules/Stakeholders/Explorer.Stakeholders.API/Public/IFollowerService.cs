using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IFollowerService
{
    // Follow/Unfollow
    FollowerDto Follow(long userId, long followingUserId);
    void Unfollow(long userId, long followingUserId);
    
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
