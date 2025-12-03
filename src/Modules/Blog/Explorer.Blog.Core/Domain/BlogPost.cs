using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public long AuthorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public List<BlogImage> Images { get; private set; }

        protected BlogPost() { }

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            Images = images?.ToList() ?? new List<BlogImage>();
            Validate();
        }

        public void Edit(string title, string description, IEnumerable<BlogImage>? images)
        {
            Title = title;
            Description = description;
            Images = images?.ToList() ?? new List<BlogImage>();
            Validate();
        }

        private void Validate()
        {
            if (AuthorId <= 0) throw new ArgumentException("Invalid AuthorId");
            if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Invalid Title");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        }
    }
}