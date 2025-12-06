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
public class BlogPostQueryTests : BaseBlogIntegrationTest
{
    public BlogPostQueryTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_author_blog_posts_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();
        var controller = CreateController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.GetMyBlogPosts().Result).Value as List<BlogPostDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldContain(p => p.Id == -1);
        result.ShouldAllBe(p => p.AuthorId == -21);
    }

    private static BlogPostController CreateController(IServiceScope scope, long userId, long personId)
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
