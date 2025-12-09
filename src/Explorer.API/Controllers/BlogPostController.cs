using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers;

[ApiController]
[Route("api/blog-posts")]
public class BlogPostController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    /// <summary>
    /// Get all visible blogs (public endpoint - no auth required)
    /// Tourists/Authors see their own drafts + all published/archived
    /// Anonymous users see only published/archived
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<List<BlogPostDto>> GetVisibleBlogs()
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.PersonId() : (long?)null;
        var result = _blogPostService.GetVisibleBlogs(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get a single blog post by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public ActionResult<BlogPostDto> GetById(long id)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.PersonId() : (long?)null;
        var result = _blogPostService.GetById(id, userId);
        return Ok(result);
    }

    /// <summary>
    /// Get only my blog posts (requires authentication)
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<List<BlogPostDto>> GetMyBlogPosts()
    {
        var authorId = User.PersonId();
        var result = _blogPostService.GetForAuthor(authorId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new blog post (requires authentication)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> Create([FromBody] CreateBlogPostDto dto)
    {
        dto.AuthorId = User.PersonId();
        var created = _blogPostService.Create(dto);
        return Ok(created);
    }

    /// <summary>
    /// Update draft blog post (title, description, images)
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> UpdateDraft(long id, [FromBody] UpdateBlogPostDto dto)
    {
        var authorId = User.PersonId();
        var updated = _blogPostService.UpdateDraft(id, dto, authorId);
        return Ok(updated);
    }

    /// <summary>
    /// Update published blog post (description only)
    /// </summary>
    [HttpPut("{id:long}/description")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> UpdatePublished(long id, [FromBody] UpdatePublishedBlogPostDto dto)
    {
        var authorId = User.PersonId();
        var updated = _blogPostService.UpdatePublished(id, dto, authorId);
        return Ok(updated);
    }

    /// <summary>
    /// Publish a draft blog post
    /// </summary>
    [HttpPut("{id:long}/publish")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> Publish(long id)
    {
        var authorId = User.PersonId();
        var published = _blogPostService.Publish(id, authorId);
        return Ok(published);
    }

    /// <summary>
    /// Archive a published blog post
    /// </summary>
    [HttpPut("{id:long}/archive")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> Archive(long id)
    {
        var authorId = User.PersonId();
        var archived = _blogPostService.Archive(id, authorId);
        return Ok(archived);
    }

    /// <summary>
    /// Delete a blog post
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult Delete(long id)
    {
        _blogPostService.Delete(id);
        return Ok();
    }

    /// <summary>
    /// Add upvote to a published blog post
    /// </summary>
    [HttpPost("{id:long}/upvote")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> AddUpvote(long id)
    {
        var personId = User.PersonId();
        var result = _blogPostService.AddUpvote(id, personId);
        return Ok(result);
    }

    /// <summary>
    /// Add downvote to a published blog post
    /// </summary>
    [HttpPost("{id:long}/downvote")]
    [Authorize(Policy = "personPolicy")]
    public ActionResult<BlogPostDto> AddDownvote(long id)
    {
        var personId = User.PersonId();
        var result = _blogPostService.AddDownvote(id, personId);
        return Ok(result);
    }
}
