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
    public void Gets_all_messages_for_club()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetClubMessages(-1).Result.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        var messagesList = result.ToList();
        messagesList.Count.ShouldBe(3); // poruke -1, -2 i -4 su za klub -1
        messagesList.ShouldContain(m => m.Id == -1 && m.AuthorId == -11);
        messagesList.ShouldContain(m => m.Id == -2 && m.AuthorId == -12);
        messagesList.ShouldContain(m => m.Id == -4 && m.AuthorId == -12);
    }

    [Fact]
    public void Gets_empty_list_for_club_without_messages()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetClubMessages(-3).Result.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        result.Count().ShouldBe(0);
    }

    [Fact]
    public void Messages_are_ordered_by_timestamp_descending()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetClubMessages(-1).Result.Result)?.Value as IEnumerable<ClubMessageDto>;

        result.ShouldNotBeNull();
        var messagesList = result.ToList();
        
        // Provera da je prva poruka novija (ima ve?i timestamp)
        if (messagesList.Count >= 2)
        {
            messagesList[0].TimestampCreated.ShouldBeGreaterThanOrEqualTo(messagesList[1].TimestampCreated);
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
