using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain.Events
{
    public class CommentDeletedEvent // : IDomainEvent
    {
        public long BlogId { get; }
        public long CommentId { get; }

        public CommentDeletedEvent(long blogId, long commentId)
        {
            BlogId = blogId;
            CommentId = commentId;
        }
    }
}