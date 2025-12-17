using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public NotificationDto Create(NotificationDto dto)
        {
            var notification = new Notification(
                dto.UserId,
                (NotificationType)dto.Type,
                dto.Title,
                dto.Content,
                dto.RelatedEntityId,
                dto.RelatedEntityType
            );

            var created = _notificationRepository.Create(notification);
            return _mapper.Map<NotificationDto>(created);
        }

        public List<NotificationDto> GetByUserId(long userId)
        {
            var notifications = _notificationRepository.GetByUserId(userId);
            return notifications.Select(_mapper.Map<NotificationDto>).ToList();
        }

        public List<NotificationDto> GetUnreadByUserId(long userId)
        {
            var notifications = _notificationRepository.GetUnreadByUserId(userId);
            return notifications.Select(_mapper.Map<NotificationDto>).ToList();
        }

        public NotificationDto MarkAsRead(long notificationId)
        {
            var notification = _notificationRepository.Get(notificationId);
            notification.MarkAsRead();
            var updated = _notificationRepository.Update(notification);
            return _mapper.Map<NotificationDto>(updated);
        }

        public void MarkAllAsRead(long userId)
        {
            var notifications = _notificationRepository.GetUnreadByUserId(userId);
            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
                _notificationRepository.Update(notification);
            }
        }

        public List<NotificationDto> GetUnreadByUserIdAndTypes(long userId, params int[] types)
        {
            var allNotifications = _notificationRepository.GetUnreadByUserId(userId);
            var notificationTypes = types.Select(t => (NotificationType)t).ToArray();
            var filtered = allNotifications.Where(n => notificationTypes.Contains(n.Type));
            return filtered.Select(_mapper.Map<NotificationDto>).ToList();
        }
    }
}
