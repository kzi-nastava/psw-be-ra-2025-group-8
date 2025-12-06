using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Blog.Core.Domain
{
    public class BlogVote : Entity
    {
        public long UserId { get; private set; }
        public int Value { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public long BlogPostId { get; private set; }

        protected BlogVote() { }

        public BlogVote(long userId, int value)
        {
            SetValue(value);
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public void ChangeValue(int value)
        {
            SetValue(value);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetValue(int value)
        {
            if (value != 1 && value != -1)
                throw new ArgumentException("Vote value must be +1 or -1.");

            Value = value;
        }
    }
}
