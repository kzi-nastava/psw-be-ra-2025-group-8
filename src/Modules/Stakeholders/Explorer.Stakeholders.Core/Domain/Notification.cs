using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Notification : Entity
    {
        public long UserId { get; private set; }
        public NotificationType Type { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public long? RelatedEntityId { get; private set; }
        public string? RelatedEntityType { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Notification() { }

        public Notification(long userId, NotificationType type, string title, string content, long? relatedEntityId = null, string? relatedEntityType = null)
        {
            if (userId == 0) throw new ArgumentException("Invalid UserId.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty.");

            UserId = userId;
            Type = type;
            Title = title;
            Content = content;
            RelatedEntityId = relatedEntityId;
            RelatedEntityType = relatedEntityType;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }

    public enum NotificationType
    {
        IssueMessage = 0,
        General = 1
    }
}
