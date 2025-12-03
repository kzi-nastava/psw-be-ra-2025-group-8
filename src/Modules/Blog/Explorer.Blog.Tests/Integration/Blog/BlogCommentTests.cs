using System;
using System.Collections.Generic;
using System.Security.Claims;
using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Blog.Tests.Integration.Blog;

[Collection("Sequential")]
public class BlogCommentTests : BaseBlogIntegrationTest
{
    private const long PublishedBlogId = -2;
    private const long DraftBlogId = -1;
    private const long TouristPersonId = -22;

    public BlogCommentTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_comments_for_published_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);

        // Act
        var actionResult = controller.GetCommentsForBlog(PublishedBlogId);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<CommentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Text.ShouldBe("Hvala, drago mi je!");
        result[1].Text.ShouldBe("Super post, jako mi se sviđa!");
    }

    [Fact]
    public void Gets_empty_list_for_blog_without_comments()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);

        // Act
        var actionResult = controller.GetCommentsForBlog(DraftBlogId);
        var okResult = actionResult.Result.ShouldBeOfType<OkObjectResult>();
        var result = okResult.Value as List<CommentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void Gets_not_found_for_non_existent_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);

        // Act
        var actionResult = controller.GetCommentsForBlog(9999);

        // Assert
        actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    private const long SecurePublishedBlogId = -4;

    [Fact]
    public void Creates_comment_successfully()//if fail drop test database
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);
        var newCommentData = new CommentCreationDto
        {
            Text = "Novi komentar iz testa."
        };

        // Act
        var actionResult = controller.Create(SecurePublishedBlogId, newCommentData).Result;

        // Assert
        var createdResult = actionResult.ShouldBeOfType<CreatedAtActionResult>();
        var result = createdResult.Value as CommentDto;

        result.ShouldNotBeNull();
        result.Text.ShouldBe(newCommentData.Text);
        result.PersonId.ShouldBe(TouristPersonId);
        result.Id.ShouldBeGreaterThan(0);

        var blogService = scope.ServiceProvider.GetRequiredService<IBlogCommentService>();
        var comments = blogService.GetCommentsForBlog(SecurePublishedBlogId);
        comments.Count.ShouldBe(1);
    }

    private const long ArchivedBlogId = -3;

    [Fact]
    public void Cannot_create_comment_on_archived_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);
        var newCommentData = new CommentCreationDto { Text = "Neuspeli komentar" };

        // Act
        var actionResult = controller.Create(ArchivedBlogId, newCommentData).Result;

        // Assert
        var objectResult = actionResult.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(403);
    }

    [Fact]
    public void Cannot_create_comment_with_empty_text()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateCommentController(scope, userId: -102, personId: TouristPersonId);
        var newCommentData = new CommentCreationDto { Text = "" };

        // Act
        var actionResult = controller.Create(SecurePublishedBlogId, newCommentData).Result;

        // Assert
        actionResult.ShouldBeOfType<BadRequestObjectResult>();
    }


    private static BlogCommentController CreateCommentController(IServiceScope scope, long userId, long personId)
    {
        var controller = new BlogCommentController(
            scope.ServiceProvider.GetRequiredService<IBlogCommentService>()
        );

        // simulating logged in user
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("personId", personId.ToString()),
                    new Claim(ClaimTypes.Role, "tourist") // role is irrelevant(tourist or author)
                }))
            }
        };

        return controller;
    }
}