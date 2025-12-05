using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        NotificationDto Create(NotificationDto dto);
        List<NotificationDto> GetByUserId(long userId);
        List<NotificationDto> GetUnreadByUserId(long userId);
        NotificationDto MarkAsRead(long notificationId);
        void MarkAllAsRead(long userId);
    }
}
