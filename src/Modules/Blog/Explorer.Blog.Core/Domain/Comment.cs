using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Blog.Core.Domain
{
    // Comment is an entity in BlogPost agregate
    public class Comment : Entity
    {
        public long PersonId { get; private set; }
        public DateTime CreationTime { get; private set; }
        public string Text { get; private set; }
        public DateTime? LastEditTime { get; private set; }

        public Comment() { }

        public Comment(long personId, DateTime creationTime, string text)
        {
            Validate(text);

            PersonId = personId;
            CreationTime = creationTime;
            Text = text;
            LastEditTime = null;
        }

        private void Validate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Comment text cannot be empty.", nameof(text));
            }
        }

        public void UpdateText(string newText)
        {
            Validate(newText);
            Text = newText;
            LastEditTime = DateTime.UtcNow;
        }

        public bool CanBeModified()
        {
            const int modificationLimitMinutes = 15;
            return (DateTime.UtcNow - CreationTime).TotalMinutes <= modificationLimitMinutes;
        }
    }
}