using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        Notification Get(long id);
        List<Notification> GetByUserId(long userId);
        List<Notification> GetUnreadByUserId(long userId);
        Notification Update(Notification notification);
    }
}
