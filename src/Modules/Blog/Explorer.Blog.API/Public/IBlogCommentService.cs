using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IBlogCommentService
{
    CommentDto Create(long blogId, long personId, CommentCreationDto commentData);
    List<CommentDto> GetCommentsForBlog(long blogId);
}