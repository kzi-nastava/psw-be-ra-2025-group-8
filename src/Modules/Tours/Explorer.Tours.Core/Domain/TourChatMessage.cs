using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourChatMessage : Entity
    {
        public long TourChatRoomId { get; private set; }
        public long SenderId { get; private set; }
        public string Content { get; private set; }
        public DateTime SentAt { get; private set; }
        public DateTime? EditedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        protected TourChatMessage() { }

        public TourChatMessage(long tourChatRoomId, long senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty", nameof(content));

            TourChatRoomId = tourChatRoomId;
            SenderId = senderId;
            Content = content;
            SentAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void Edit(string newContent, long requestedByUserId)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot edit deleted message");

            if (requestedByUserId != SenderId)
                throw new UnauthorizedAccessException("Only the sender can edit the message");

            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Message content cannot be empty", nameof(newContent));

            Content = newContent;
            EditedAt = DateTime.UtcNow;
        }

        public void Delete(long requestedByUserId)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Message is already deleted");

            if (requestedByUserId != SenderId)
                throw new UnauthorizedAccessException("Only the sender can delete the message");

            IsDeleted = true;
            Content = "[Message deleted]";
            EditedAt = DateTime.UtcNow;
        }
    }
}
