using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public;

public interface IBlogPostService
{
    BlogPostDto Create(CreateBlogPostDto request);
    BlogPostDto Update(long id, UpdateBlogPostDto request);
    List<BlogPostDto> GetForAuthor(long authorId);
    void Delete(long id);
}
