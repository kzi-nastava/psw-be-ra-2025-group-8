using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Explorer.Blog.Tests.Integration.Blog;

[Collection("Sequential")]
public class BlogCommentCommandTests : BaseBlogIntegrationTest
{
    private const long PublishedBlogId = -2;
    private const long SecurePublishedBlogId = -4;

    private const long AUTHOR_ID = -21;      // author of comment -2
    private const long TOURIST_2_ID = -22;   // author of comment -1
    private const long TOURIST_3_ID = -23;   // author of comment -3

    public BlogCommentCommandTests(BlogTestFactory factory) : base(factory) { }

    private static CommentCreationDto GetCommentUpdateDto(string text)
    {
        return new CommentCreationDto
        {
            Text = text
        };
    }

    private long CreateFreshComment(IServiceScope scope, long blogId, long personId)
    {
        var controller = CreateCommentController(scope, userId: -100, personId: personId);
        var newCommentData = new CommentCreationDto
        {
            Text = "Privremeni komentar za testiranje izmene/brisanja."
        };

        var actionResult = controller.Create(blogId, newCommentData).Result;
        var createdResult = actionResult.ShouldBeOfType<CreatedAtActionResult>();
        var result = createdResult.Value as CommentDto;

        return result.Id;
    }

    [Fact]
    public void Update_succeeds_within_time_limit()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = CreateFreshComment(scope, SecurePublishedBlogId, AUTHOR_ID);
        var controller = CreateCommentController(scope, userId: -100, personId: AUTHOR_ID);
        var updatedText = "Tekst je uspešno ažuriran!";

        // Act
        var result = controller.Update(SecurePublishedBlogId, commentId, GetCommentUpdateDto(updatedText)).Result;

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var updatedComment = okResult.Value.ShouldBeOfType<CommentDto>();

        updatedComment.Text.ShouldBe(updatedText);
        updatedComment.LastEditTime.ShouldNotBeNull();
        updatedComment.LastEditTime.Value.ShouldBeGreaterThan(updatedComment.CreationTime);
    }

    [Fact]
    public void Update_fails_unauthorized_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = CreateFreshComment(scope, SecurePublishedBlogId, AUTHOR_ID);

        var controller = CreateCommentController(scope, userId: -100, personId: TOURIST_2_ID);

        // Act
        var result = controller.Update(SecurePublishedBlogId, commentId, GetCommentUpdateDto("Attempted hack")).Result;

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(403); // 403 Forbidden (not author)
    }

    [Fact]
    public void Update_fails_time_elapsed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = -1;

        var controller = CreateCommentController(scope, userId: -100, personId: TOURIST_2_ID);

        // Act
        var result = controller.Update(PublishedBlogId, commentId, GetCommentUpdateDto("Too late!")).Result;

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(403); // 403 Forbidden (time threshold passed)
    }


    [Fact]
    public void Delete_succeeds_within_time_limit()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = CreateFreshComment(scope, SecurePublishedBlogId, TOURIST_2_ID);
        var controller = CreateCommentController(scope, userId: -100, personId: TOURIST_2_ID);

        // Act
        var result = controller.Delete(SecurePublishedBlogId, commentId);

        // Assert
        result.ShouldBeOfType<NoContentResult>(); // 204 No Content

        // Second delete to test validity of first one
        var deleteResult = controller.Delete(SecurePublishedBlogId, commentId);
        deleteResult.ShouldBeOfType<NotFoundObjectResult>(); // 404 Not Found
    }

    [Fact]
    public void Delete_fails_unauthorized_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = CreateFreshComment(scope, SecurePublishedBlogId, AUTHOR_ID);

        var controller = CreateCommentController(scope, userId: -100, personId: TOURIST_2_ID);

        // Act
        var result = controller.Delete(SecurePublishedBlogId, commentId);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(403); // 403 Forbidden (not author)
    }

    [Fact]
    public void Delete_fails_time_elapsed()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var commentId = -2;

        var controller = CreateCommentController(scope, userId: -100, personId: AUTHOR_ID);

        // Act
        var result = controller.Delete(PublishedBlogId, commentId);

        // Assert
        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(403); // 403 Forbidden (time threshold passed)
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