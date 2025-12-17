using Explorer.API.Controllers.Tourist.Profile;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Profile;

[Collection("Sequential")]
public class FollowerCommandTests : BaseStakeholdersIntegrationTest
{
    public FollowerCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    // Test user IDs from c-stakeholders.sql
    private const long USER_TOURIST1_ID = -21; // Tourist 1
    private const long USER_TOURIST2_ID = -22; // Tourist 2
    private const long USER_TOURIST3_ID = -23; // Tourist 3
    private const long USER_ADMIN_ID = -100;   // Admin

    [Fact]
    public void Follow_user_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ADMIN_ID); // Admin follows Tourist1
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = ((OkObjectResult)controller.Follow(USER_TOURIST1_ID).Result)?.Value as FollowerDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.UserId.ShouldBe(USER_ADMIN_ID);
        result.FollowedAt.ShouldBeGreaterThan(DateTime.MinValue);

        // Assert - Database
        var stored = dbContext.Followers.FirstOrDefault(f => f.UserId == USER_ADMIN_ID && f.FollowingUserId == USER_TOURIST1_ID);
        stored.ShouldNotBeNull();
    }

    [Fact]
    public void Follow_fails_when_already_following()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST3_ID); // Tourist3 follows Tourist1 FIRST
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Create follow relationship
        controller.Follow(USER_TOURIST1_ID);

        // Act - Try to follow again
        var actionResult = controller.Follow(USER_TOURIST1_ID).Result;
        var result = actionResult as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Unfollow_user_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST2_ID); // Tourist2 unfollows Tourist1
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Create follow relationship first
        controller.Follow(USER_TOURIST1_ID);

        // Act - Unfollow
        var result = controller.Unfollow(USER_TOURIST1_ID) as OkObjectResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var stored = dbContext.Followers.FirstOrDefault(f => f.UserId == USER_TOURIST2_ID && f.FollowingUserId == USER_TOURIST1_ID);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Unfollow_fails_when_not_following()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ADMIN_ID); // Admin doesn't follow Tourist3

        // Act
        var result = controller.Unfollow(USER_TOURIST3_ID) as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

  [Fact]
    public void Send_message_to_followers_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST1_ID); // Tourist1 sends message
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Create follower relationship: Tourist2 follows Tourist1
        var followerController = CreateController(scope, USER_TOURIST2_ID);
        followerController.Follow(USER_TOURIST1_ID);

        var messageDto = new SendFollowerMessageDto
        {
            Content = "Test message to my followers!",
            AttachmentType = "Tour",
            AttachmentResourceId = -10
        };

        // Act
        var result = controller.SendMessageToFollowers(messageDto) as OkObjectResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database (Message created)
        var message = dbContext.FollowerMessages
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefault(m => m.SenderId == USER_TOURIST1_ID);
        message.ShouldNotBeNull();
        message.Content.ShouldBe(messageDto.Content);

        // Assert - Database (Notification created for follower)
        var notification = dbContext.Notifications
                    .OrderByDescending(n => n.Id)
                    .FirstOrDefault(n => n.UserId == USER_TOURIST2_ID && n.Type == NotificationType.FollowerMessage);
        notification.ShouldNotBeNull();
    }

    [Fact]
    public void Send_message_fails_with_content_exceeding_280_characters()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST2_ID);

        var messageDto = new SendFollowerMessageDto
        {
            Content = new string('a', 281) // 281 characters
        };

        // Act
        var result = controller.SendMessageToFollowers(messageDto) as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Send_message_fails_with_invalid_attachment_type()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST2_ID);

        var messageDto = new SendFollowerMessageDto
        {
            Content = "Test message",
            AttachmentType = "InvalidType",
            AttachmentResourceId = 10
        };

        // Act
        var result = controller.SendMessageToFollowers(messageDto) as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    private static FollowerController CreateController(IServiceScope scope, long userId)
    {
        return new FollowerController(scope.ServiceProvider.GetRequiredService<IFollowerService>())
        {
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}
