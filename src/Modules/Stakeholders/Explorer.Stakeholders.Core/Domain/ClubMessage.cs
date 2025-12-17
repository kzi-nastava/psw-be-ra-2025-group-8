using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMessage : Entity
    {
        public long ClubId { get; private set; }
        public long AuthorId { get; private set; }
        public string Content { get; private set; }
        public DateTime TimestampCreated { get; private set; }
        public DateTime? TimestampUpdated { get; private set; }

        protected ClubMessage() { }

        public ClubMessage(long clubId, long authorId, string content)
        {
            ClubId = clubId;
            AuthorId = authorId;
            Content = content;
            TimestampCreated = DateTime.UtcNow;
            Validate();
        }

        public void Update(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Content cannot be empty");
            
            Content = newContent;
            TimestampUpdated = DateTime.UtcNow;
        }

        private void Validate()
        {
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
            if (AuthorId == 0) throw new ArgumentException("Invalid AuthorId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Content cannot be empty");
        }
    }
}
