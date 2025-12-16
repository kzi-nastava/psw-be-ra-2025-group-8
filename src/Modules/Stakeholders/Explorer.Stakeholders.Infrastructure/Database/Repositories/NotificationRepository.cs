using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly StakeholdersContext _context;

        public NotificationRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public Notification Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return notification;
        }

        public Notification Get(long id)
        {
            return _context.Notifications.FirstOrDefault(n => n.Id == id);
        }

        public List<Notification> GetByUserId(long userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public List<Notification> GetUnreadByUserId(long userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public Notification Update(Notification notification)
        {
            _context.Notifications.Update(notification);
            _context.SaveChanges();
            return notification;
        }
    }
}
