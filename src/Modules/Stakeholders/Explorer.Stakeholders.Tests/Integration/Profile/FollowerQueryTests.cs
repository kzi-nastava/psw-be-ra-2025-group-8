using Explorer.API.Controllers.Tourist.Profile;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Profile;

[Collection("Sequential")]
public class FollowerQueryTests : BaseStakeholdersIntegrationTest
{
    public FollowerQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    // Test user IDs from c-stakeholders.sql
    private const long USER_TOURIST1_ID = -21; // Tourist 1
    private const long USER_TOURIST2_ID = -22; // Tourist 2
    private const long USER_TOURIST3_ID = -23; // Tourist 3
    private const long USER_ADMIN_ID = -100;   // Admin

    [Fact]
    public void Get_my_followers_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST1_ID); // Tourist1 gets followers

        // Create followers: Tourist2 and Tourist3 follow Tourist1
        var follower2Controller = CreateController(scope, USER_TOURIST2_ID);
        follower2Controller.Follow(USER_TOURIST1_ID);
        var follower3Controller = CreateController(scope, USER_TOURIST3_ID);
        follower3Controller.Follow(USER_TOURIST1_ID);

        // Act
        var result = ((OkObjectResult)controller.GetMyFollowers().Result)?.Value as List<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.Any(f => f.UserId == USER_TOURIST2_ID).ShouldBeTrue();
        result.Any(f => f.UserId == USER_TOURIST3_ID).ShouldBeTrue();
    }

    [Fact]
    public void Get_my_followers_returns_empty_when_no_followers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ADMIN_ID); // Admin has no followers

        // Act
        var result = ((OkObjectResult)controller.GetMyFollowers().Result)?.Value as List<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Get_my_following_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST1_ID); // Tourist1 gets who they follow

         // Create following: Tourist1 follows Tourist2 and Tourist3
         controller.Follow(USER_TOURIST2_ID);
        controller.Follow(USER_TOURIST3_ID);

        // Act
        var result = ((OkObjectResult)controller.GetMyFollowing().Result)?.Value as List<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.Any(f => f.UserId == USER_TOURIST2_ID).ShouldBeTrue();
        result.Any(f => f.UserId == USER_TOURIST3_ID).ShouldBeTrue();
    }

    [Fact]
    public void Get_my_following_returns_empty_when_not_following_anyone()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_ADMIN_ID); // Admin doesn't follow anyone

        // Act
        var result = ((OkObjectResult)controller.GetMyFollowing().Result)?.Value as List<FollowerDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Get_notifications_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var senderController = CreateController(scope, USER_TOURIST1_ID);
        var receiverController = CreateController(scope, USER_TOURIST2_ID);

        // Tourist2 follows Tourist1
        receiverController.Follow(USER_TOURIST1_ID);

        // Tourist1 sends message
        var messageDto = new SendFollowerMessageDto { Content = "Test notification" };
        senderController.SendMessageToFollowers(messageDto);

        // Act
        var result = ((OkObjectResult)receiverController.GetNotifications().Result)?.Value as List<NotificationDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Get_unread_notifications_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var senderController = CreateController(scope, USER_TOURIST1_ID);
        var receiverController = CreateController(scope, USER_TOURIST3_ID);

        // Tourist3 follows Tourist1
        receiverController.Follow(USER_TOURIST1_ID);

        // Tourist1 sends message
        var messageDto = new SendFollowerMessageDto { Content = "Unread test" };
        senderController.SendMessageToFollowers(messageDto);

        // Act
        var result = ((OkObjectResult)receiverController.GetUnreadNotifications().Result)?.Value as List<NotificationDto>;

        // Assert
        result.ShouldNotBeNull();
        result.All(n => !n.IsRead).ShouldBeTrue();
    }

    [Fact]
    public void Mark_notification_as_read_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var senderController = CreateController(scope, USER_TOURIST1_ID);
        var receiverController = CreateController(scope, USER_ADMIN_ID);

        // Admin follows Tourist1
        receiverController.Follow(USER_TOURIST1_ID);

        // Tourist1 sends message
        var messageDto = new SendFollowerMessageDto { Content = "Mark as read test" };
        senderController.SendMessageToFollowers(messageDto);

        // Get notification ID
        var notifications = ((OkObjectResult)receiverController.GetUnreadNotifications().Result)?.Value as List<NotificationDto>;
        notifications.ShouldNotBeNull();
        notifications.Count.ShouldBeGreaterThan(0);
        var notificationId = notifications!.First().Id;

        // Act
        var result = receiverController.MarkNotificationAsRead(notificationId);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.StatusCode.ShouldBe(200);
    }

    [Fact]
    public void Mark_notification_as_read_fails_with_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, USER_TOURIST1_ID);

        // Act
        var result = controller.MarkNotificationAsRead(-9999) as NotFoundObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    private static FollowerController CreateController(IServiceScope scope, long userId)
    {
        return new FollowerController(scope.ServiceProvider.GetRequiredService<IFollowerService>())
        {
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}
