using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Message : Entity
    {
        public long SenderId { get; private set; }
        public long RecipientId { get; private set; }
        public string Content { get; private set; }
        public DateTime TimestampCreated { get; private set; }
        public DateTime? TimestampUpdated { get; private set; }
        public bool IsDeleted { get; private set; }

        protected Message() { }

        public Message(long senderId, long recipientId, string content)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Content = content;
            TimestampCreated = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void Edit(string newContent)
        {
            Content = newContent;
            TimestampUpdated = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            Content = "Message deleted";
            TimestampUpdated = DateTime.UtcNow;
        }
    }
}
