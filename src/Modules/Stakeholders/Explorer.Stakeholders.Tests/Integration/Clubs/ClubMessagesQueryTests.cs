using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubMessagesQueryTests : BaseStakeholdersIntegrationTest
{
    public ClubMessagesQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_club_messages_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Create some test messages first
        var msg1 = new CreateClubMessageDto { ClubId = -1, Content = "Test message 1" };
        var msg2 = new CreateClubMessageDto { ClubId = -1, Content = "Test message 2" };
        controller.PostMessage(-1, msg1);
        controller.PostMessage(-1, msg2);

        var actionResult = controller.GetClubMessages(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        var messagesList = result.ToList();
        messagesList.Count.ShouldBeGreaterThanOrEqualTo(2); // At least our 2 messages
        messagesList.All(m => m.ClubId == -1).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_messages_for_empty_club()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = controller.GetClubMessages(-3);
        var result = ((ObjectResult)actionResult.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        result.Count().ShouldBe(0);
    }

    [Fact]
    public void Messages_are_ordered_by_timestamp()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        // Create test messages with different timestamps
        var msg1 = new CreateClubMessageDto { ClubId = -1, Content = "First message" };
        var msg2 = new CreateClubMessageDto { ClubId = -1, Content = "Second message" };
        var msg3 = new CreateClubMessageDto { ClubId = -1, Content = "Third message" };

        controller.PostMessage(-1, msg1);
        System.Threading.Thread.Sleep(100); // Ensure different timestamps
        controller.PostMessage(-1, msg2);
        System.Threading.Thread.Sleep(100);
        controller.PostMessage(-1, msg3);

        var actionResult = controller.GetClubMessages(-1);
        var result = ((ObjectResult)actionResult.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        var messagesList = result.ToList();
        messagesList.Count.ShouldBeGreaterThan(1);

        // Repository returns messages ordered by TimestampCreated DESCENDING (newest first)
        for (int i = 1; i < messagesList.Count; i++)
        {
            messagesList[i].TimestampCreated.ShouldBeLessThanOrEqualTo(messagesList[i - 1].TimestampCreated);
        }
    }

    private static ClubMessagesController CreateController(IServiceScope scope, string userId = "-11")
    {
        return new ClubMessagesController(scope.ServiceProvider.GetRequiredService<IClubMessageService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
