using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Blog.Core.Domain.Events;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public long AuthorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public BlogStatus Status { get; private set; }
        public List<BlogImage> Images { get; private set; }
        public List<Comment> Comments { get; private set; }

        private readonly List<object> _domainEvents = new List<object>();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected BlogPost() { }

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            Status = BlogStatus.Draft;
            Images = images?.ToList() ?? new List<BlogImage>();
            Comments = new List<Comment>();
            Validate();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private void AddDomainEvent(object domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void UpdateDraft(string title, string description, IEnumerable<BlogImage>? images)
        {
            if (Status != BlogStatus.Draft)
                throw new InvalidOperationException("Only draft blogs can have title and images updated.");

            Title = title;
            Description = description;
            Images = images?.ToList() ?? new List<BlogImage>();
            LastModifiedAt = DateTime.UtcNow;
            Validate();
        }

        public void UpdatePublished(string description)
        {
            if (Status != BlogStatus.Published)
                throw new InvalidOperationException("Only published blogs can be updated this way.");

            Description = description;
            LastModifiedAt = DateTime.UtcNow;
            ValidateDescription();
        }

        public void Publish()
        {
            if (Status != BlogStatus.Draft)
                throw new InvalidOperationException("Only draft blogs can be published.");

            Status = BlogStatus.Published;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != BlogStatus.Published)
                throw new InvalidOperationException("Only published blogs can be archived.");

            Status = BlogStatus.Archived;
            LastModifiedAt = DateTime.UtcNow;
        }

        public Comment AddComment(long personId, string text)
        {
            // moze se kreirati samo u stanju objavljen
            if (Status != BlogStatus.Published)
            {
                throw new InvalidOperationException("Comments can only be added to a Published blog.");
            }

            var newComment = new Comment(
                personId,
                DateTime.UtcNow,
                text
            );

            Comments.Add(newComment);

            AddDomainEvent(new CommentCreatedEvent(this.Id, newComment.Id));

            return newComment;
        }

        public Comment UpdateComment(long commentId, long personId, string newText)
        {
            throw new NotImplementedException();
        }

        public void DeleteComment(long commentId, long personId)
        {
            throw new NotImplementedException();
        }

        private void Validate()
        {
            if (AuthorId == 0) throw new ArgumentException("Invalid AuthorId");
            if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Invalid Title");
            ValidateDescription();
        }

        private void ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        }
    }
}