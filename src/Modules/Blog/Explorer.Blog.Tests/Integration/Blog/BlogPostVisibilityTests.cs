using System.Collections.Generic;
using System.Security.Claims;
using Explorer.API.Controllers;
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
public class BlogPostVisibilityTests : BaseBlogIntegrationTest
{
    public BlogPostVisibilityTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Tourist_sees_only_published_and_archived_blogs()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, userId: -23, personId: -23);

        var result = ((ObjectResult)controller.GetVisibleBlogs().Result).Value as List<BlogPostDto>;

        result.ShouldNotBeNull();
        result.ShouldAllBe(b => b.Status == 1 || b.Status == 2); // Published or Archived
        result.ShouldNotContain(b => b.Status == 0); // No drafts from other authors
    }

    [Fact]
    public void Author_sees_own_drafts_plus_published_and_archived()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.GetVisibleBlogs().Result).Value as List<BlogPostDto>;

        result.ShouldNotBeNull();
        
        // Debug: print what we got
        Console.WriteLine($"Total blogs returned: {result.Count}");
        foreach (var blog in result)
        {
            Console.WriteLine($"Blog ID: {blog.Id}, AuthorId: {blog.AuthorId}, Status: {blog.Status}, Title: {blog.Title}");
        }
        
        result.ShouldContain(b => b.Status == 0 && b.AuthorId == -21); // Own drafts
        result.ShouldContain(b => b.Status == 1); // Published
        result.ShouldContain(b => b.Status == 2); // Archived
    }

    [Fact]
    public void Gets_only_own_blog_posts()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.GetMyBlogPosts().Result).Value as List<BlogPostDto>;

        result.ShouldNotBeNull();
        result.ShouldAllBe(p => p.AuthorId == -21);
    }

    private static BlogPostController CreateTouristController(IServiceScope scope, long userId, long personId)
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
                    new Claim(ClaimTypes.Role, "tourist")
                }, "Test"))
            }
        };

        return controller;
    }
}
