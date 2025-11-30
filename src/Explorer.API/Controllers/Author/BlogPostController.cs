using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/blog-posts")]
[ApiController]
public class BlogPostController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public ActionResult<List<BlogPostDto>> GetMyBlogPosts()
    {
        var authorId = User.PersonId();
        var result = _blogPostService.GetForAuthor(authorId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<BlogPostDto> Create([FromBody] CreateBlogPostDto dto)
    {
        dto.AuthorId = User.PersonId();
        var created = _blogPostService.Create(dto);
        return Ok(created);
    }

    [HttpPut("{id:long}")]
    public ActionResult<BlogPostDto> Update(long id, [FromBody] UpdateBlogPostDto dto)
    {
        var updated = _blogPostService.Update(id, dto);
        return Ok(updated);
    }
}
