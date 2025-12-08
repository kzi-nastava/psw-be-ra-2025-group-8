using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain.Events
{
    public class CommentUpdatedEvent // : IDomainEvent
    {
        public long BlogId { get; }
        public long CommentId { get; }

        public CommentUpdatedEvent(long blogId, long commentId)
        {
            BlogId = blogId;
            CommentId = commentId;
        }
    }
}