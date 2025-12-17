using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain.Events
{
    public class CommentCreatedEvent  //: IDomainEvent
    {
        public long BlogId { get; }
        public long CommentId { get; }

        public CommentCreatedEvent(long blogId, long commentId)
        {
            BlogId = blogId;
            CommentId = commentId;
        }
    }
}