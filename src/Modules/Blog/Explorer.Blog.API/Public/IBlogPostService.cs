using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IBlogPostService
{
    BlogPostDto Create(CreateBlogPostDto request);
    BlogPostDto UpdateDraft(long id, UpdateBlogPostDto request, long authorId);
    BlogPostDto UpdatePublished(long id, UpdatePublishedBlogPostDto request, long authorId);
    BlogPostDto Publish(long id, long authorId);
    BlogPostDto Archive(long id, long authorId);
    List<BlogPostDto> GetForAuthor(long authorId);
    List<BlogPostDto> GetVisibleBlogs(long? userId);
    void Delete(long id);
}
