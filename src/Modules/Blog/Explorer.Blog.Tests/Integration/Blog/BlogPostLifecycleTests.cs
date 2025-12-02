using System.Collections.Generic;
using System.Security.Claims;
using Explorer.API.Controllers.Author;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.Integration.Blog;

[Collection("Sequential")]
public class BlogPostLifecycleTests : BaseBlogIntegrationTest
{
    public BlogPostLifecycleTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_blog_in_draft_status()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var newBlog = new CreateBlogPostDto
        {
            AuthorId = -21,
            Title = "New Draft Blog",
            Description = "Draft description",
            Images = new List<BlogImageDto>()
        };

        var result = ((ObjectResult)controller.Create(newBlog).Result).Value as BlogPostDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(0); // Draft
        result.Title.ShouldBe(newBlog.Title);
        result.Description.ShouldBe(newBlog.Description);
    }

    [Fact]
    public void Updates_draft_blog_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var updateDto = new UpdateBlogPostDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Images = new List<BlogImageDto> { new BlogImageDto { Url = "http://new.jpg", Order = 0 } }
        };

        var result = ((ObjectResult)controller.UpdateDraft(-1, updateDto).Result).Value as BlogPostDto;

        result.ShouldNotBeNull();
        result.Title.ShouldBe("Updated Title");
        result.Description.ShouldBe("Updated Description");
        result.LastModifiedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Publishes_draft_blog_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.Publish(-1).Result).Value as BlogPostDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Published
        result.LastModifiedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Updates_published_blog_description_only()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var updateDto = new UpdatePublishedBlogPostDto
        {
            Description = "Updated published description"
        };

        var result = ((ObjectResult)controller.UpdatePublished(-4, updateDto).Result).Value as BlogPostDto;

        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated published description");
        result.LastModifiedAt.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Still published
    }

    [Fact]
    public void Archives_published_blog_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.Archive(-2).Result).Value as BlogPostDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(2); // Archived
        result.LastModifiedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Cannot_update_draft_for_published_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        var updateDto = new UpdateBlogPostDto
        {
            Title = "Should Fail",
            Description = "Should Fail",
            Images = new List<BlogImageDto>()
        };

        Should.Throw<InvalidOperationException>(() => controller.UpdateDraft(-2, updateDto));
    }

    [Fact]
    public void Cannot_publish_already_published_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        Should.Throw<InvalidOperationException>(() => controller.Publish(-2));
    }

    [Fact]
    public void Cannot_archive_draft_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        Should.Throw<InvalidOperationException>(() => controller.Archive(-1));
    }

    [Fact]
    public void Cannot_archive_already_archived_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -21, personId: -21);

        Should.Throw<InvalidOperationException>(() => controller.Archive(-3));
    }

    [Fact]
    public void Cannot_update_another_authors_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, userId: -22, personId: -22);

        var updateDto = new UpdateBlogPostDto
        {
            Title = "Should Fail",
            Description = "Should Fail",
            Images = new List<BlogImageDto>()
        };

        Should.Throw<UnauthorizedAccessException>(() => controller.UpdateDraft(-1, updateDto));
    }

    private static BlogPostController CreateAuthorController(IServiceScope scope, long userId, long personId)
    {
        var controller = new BlogPostController(
            scope.ServiceProvider.GetRequiredService<IBlogPostService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("personId", personId.ToString()),
                    new Claim(ClaimTypes.Role, "author")
                }))
            }
        };

        return controller;
    }
}
