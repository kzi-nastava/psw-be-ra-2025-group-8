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
        public BlogPopularityStatus PopularityStatus { get; private set; }
        public List<BlogImage> Images { get; private set; }
        public List<Comment> Comments { get; private set; }
        public List<Vote> Votes { get; private set; }

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
            PopularityStatus = BlogPopularityStatus.None;
            Images = images?.ToList() ?? new List<BlogImage>();
            Comments = new List<Comment>();
            Votes = new List<Vote>();
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
            if (PopularityStatus == BlogPopularityStatus.Closed)
                throw new InvalidOperationException("Closed blogs cannot be updated.");

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
            if (PopularityStatus == BlogPopularityStatus.Closed)
                throw new InvalidOperationException("Closed blogs cannot be updated.");

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
            
            // Update popularity status when blog is published
            UpdatePopularityStatus();
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
            // cannot add comments to closed blogs
            if (PopularityStatus == BlogPopularityStatus.Closed)
            {
                throw new InvalidOperationException("Comments cannot be added to a Closed blog.");
            }
            
            // can only be created in state published
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

            // Automatically update popularity status based on votes and comments
            UpdatePopularityStatus();

            return newComment;
        }

        public Comment UpdateComment(long commentId, long personId, string newText)
        {
            var commentToUpdate = Comments.FirstOrDefault(c => c.Id == commentId);

            if (commentToUpdate == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found in blog.");
            }

            if (commentToUpdate.PersonId != personId)
            {
                throw new UnauthorizedAccessException("Only the author can update this comment.");
            }

            if (!commentToUpdate.CanBeModified())
            {
                throw new UnauthorizedAccessException("Comment cannot be modified more than 15 minutes after creation.");
            }

            commentToUpdate.UpdateText(newText);

            AddDomainEvent(new CommentUpdatedEvent(this.Id, commentId));

            return commentToUpdate;
        }

        public void DeleteComment(long commentId, long personId)
        {
            var commentToDelete = Comments.FirstOrDefault(c => c.Id == commentId);

            if (commentToDelete == null)
            {
                throw new KeyNotFoundException($"Comment with ID {commentId} not found in blog.");
            }

            if (commentToDelete.PersonId != personId)
            {
                throw new UnauthorizedAccessException("Only the author can delete this comment.");
            }

            if (!commentToDelete.CanBeModified())
            {
                throw new UnauthorizedAccessException("Comment cannot be deleted more than 15 minutes after creation.");
            }

            Comments.Remove(commentToDelete);

            AddDomainEvent(new CommentDeletedEvent(this.Id, commentId));
        }

        public Vote AddVote(long personId, VoteType voteType)
        {
            // cannot vote on closed blogs
            if (PopularityStatus == BlogPopularityStatus.Closed)
            {
                throw new InvalidOperationException("Votes cannot be added to a Closed blog.");
            }
            
            // can only vote on published blogs
            if (Status != BlogStatus.Published)
            {
                throw new InvalidOperationException("Votes can only be added to a Published blog.");
            }

            // check if user already voted
            var existingVote = Votes.FirstOrDefault(v => v.PersonId == personId);
            if (existingVote != null)
            {
                // if same vote type, remove vote (toggle off)
                if (existingVote.Type == voteType)
                {
                    Votes.Remove(existingVote);
                    // Automatically update popularity status after vote removal
                    UpdatePopularityStatus();
                    return null;
                }
                // if different vote type, change vote
                existingVote.ChangeVote(voteType);
                // Automatically update popularity status after vote change
                UpdatePopularityStatus();
                return existingVote;
            }

            // add new vote
            var newVote = new Vote(personId, voteType);
            Votes.Add(newVote);
            
            // Automatically update popularity status based on votes and comments
            UpdatePopularityStatus();
            
            return newVote;
        }

        public void RemoveVote(long personId)
        {
            var vote = Votes.FirstOrDefault(v => v.PersonId == personId);
            if (vote != null)
            {
                Votes.Remove(vote);
                // Automatically update popularity status after vote removal
                UpdatePopularityStatus();
            }
        }

        public Vote? GetUserVote(long personId)
        {
            return Votes.FirstOrDefault(v => v.PersonId == personId);
        }

        public int GetUpvoteCount()
        {
            return Votes.Count(v => v.Type == VoteType.Upvote);
        }

        public int GetDownvoteCount()
        {
            return Votes.Count(v => v.Type == VoteType.Downvote);
        }

        public int GetScore()
        {
            return GetUpvoteCount() - GetDownvoteCount();
        }

        public void UpdatePopularityStatus()
        {
            // Only update popularity status for published blogs
            if (Status != BlogStatus.Published && Status != BlogStatus.Archived)
                return;

            var score = GetScore();
            var commentCount = Comments.Count;

            // Blog is closed if score < -10
            if (score < -10)
            {
                PopularityStatus = BlogPopularityStatus.Closed;
                LastModifiedAt = DateTime.UtcNow;
                return;
            }

            // Blog is famous if score > 500 AND commentCount > 30
            if (score > 500 && commentCount > 30)
            {
                PopularityStatus = BlogPopularityStatus.Famous;
                LastModifiedAt = DateTime.UtcNow;
                return;
            }

            // Blog is active if score > 100 OR commentCount > 10
            if (score > 100 || commentCount > 10)
            {
                PopularityStatus = BlogPopularityStatus.Active;
                LastModifiedAt = DateTime.UtcNow;
                return;
            }

            // Otherwise, set to None
            PopularityStatus = BlogPopularityStatus.None;
            LastModifiedAt = DateTime.UtcNow;
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