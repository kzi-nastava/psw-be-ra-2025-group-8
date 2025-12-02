using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/blog-posts")]
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
        var touristId = User.PersonId();
        var result = _blogPostService.GetForAuthor(touristId);
        return Ok(result);
    }

    [HttpGet("visible")]
    public ActionResult<List<BlogPostDto>> GetVisibleBlogs()
    {
        var touristId = User.PersonId();
        var result = _blogPostService.GetVisibleBlogs(touristId);
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
    public ActionResult<BlogPostDto> UpdateDraft(long id, [FromBody] UpdateBlogPostDto dto)
    {
        var touristId = User.PersonId();
        var updated = _blogPostService.UpdateDraft(id, dto, touristId);
        return Ok(updated);
    }

    [HttpPut("{id:long}/description")]
    public ActionResult<BlogPostDto> UpdatePublished(long id, [FromBody] UpdatePublishedBlogPostDto dto)
    {
        var touristId = User.PersonId();
        var updated = _blogPostService.UpdatePublished(id, dto, touristId);
        return Ok(updated);
    }

    [HttpPut("{id:long}/publish")]
    public ActionResult<BlogPostDto> Publish(long id)
    {
        var touristId = User.PersonId();
        var published = _blogPostService.Publish(id, touristId);
        return Ok(published);
    }

    [HttpPut("{id:long}/archive")]
    public ActionResult<BlogPostDto> Archive(long id)
    {
        var touristId = User.PersonId();
        var archived = _blogPostService.Archive(id, touristId);
        return Ok(archived);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _blogPostService.Delete(id);
        return Ok();
    }
}
