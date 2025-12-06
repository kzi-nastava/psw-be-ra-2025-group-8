using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public long AuthorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public List<BlogImage> Images { get; private set; }
        public List<BlogVote> Votes { get; private set; }
        public int Score { get; private set; }
        public bool IsClosed { get; private set; }

        protected BlogPost() { }

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            Images = images?.ToList() ?? new List<BlogImage>();

            Votes = new List<BlogVote>();
            Score = 0;
            IsClosed = false;

            Validate();
        }

        public void Edit(string title, string description, IEnumerable<BlogImage>? images)
        {
            if (IsClosed) throw new InvalidOperationException("Closed blog post cannot be edited.");

            Title = title;
            Description = description;
            Images = images?.ToList() ?? new List<BlogImage>();
            Validate();
        }

        public void AddOrUpdateVote(long userId, int value)
        {
            if (IsClosed) throw new InvalidOperationException("Closed blog post cannot be voted on.");
            if (value != 1 && value != -1) throw new ArgumentException("Vote value must be +1 or -1.");

            var existing = Votes.FirstOrDefault(v => v.UserId == userId);

            if (existing is null)
            {
                Votes.Add(new BlogVote(userId, value));
            }
            else
            {
                existing.ChangeValue(value);
            }

            RecalculateScore();
        }

        public BlogStatus GetStatus(int commentCount)
        {
            if (Score < -10) return BlogStatus.Closed;
            if (Score > 500 && commentCount > 30) return BlogStatus.Famous;
            if (Score > 100 || commentCount > 10) return BlogStatus.Active;
            return BlogStatus.Regular;
        }

        private void RecalculateScore()
        {
            Score = Votes.Sum(v => v.Value);

            if (Score < -10)
            {
                IsClosed = true;
            }
        }

        private void Validate()
        {
            if (AuthorId <= 0) throw new ArgumentException("Invalid AuthorId");
            if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Invalid Title");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        }
    }
}
