using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubMessagesCommandTests : BaseStakeholdersIntegrationTest
{
    public ClubMessagesCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Member_posts_message_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12"); // ?lan kluba -1
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new CreateClubMessageDto
        {
            ClubId = -1,
            Content = "Nova poruka na stranici kluba!"
        };

        var result = ((ObjectResult)controller.PostMessage(-1, dto).Result)?.Value as ClubMessageDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ClubId.ShouldBe(-1);
        result.AuthorId.ShouldBe(-12);
        result.Content.ShouldBe(dto.Content);

        db.ChangeTracker.Clear();
        var storedMessage = db.ClubMessages.FirstOrDefault(m => m.Id == result.Id);
        storedMessage.ShouldNotBeNull();
        storedMessage.Content.ShouldBe(dto.Content);
    }

    [Fact]
    public void Owner_posts_message_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11"); // vlasnik kluba -1
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new CreateClubMessageDto
        {
            ClubId = -1,
            Content = "Poruka od vlasnika kluba"
        };

        var result = ((ObjectResult)controller.PostMessage(-1, dto).Result)?.Value as ClubMessageDto;

        result.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-11);
    }

    [Fact]
    public void Non_member_cannot_post_message()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // nije ?lan kluba -1

        var dto = new CreateClubMessageDto
        {
            ClubId = -1,
            Content = "Poku?aj neovla??enog postavljanja poruke"
        };

        var result = controller.PostMessage(-1, dto).Result as ForbidResult;
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Author_updates_message_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11"); // autor poruke -1
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new UpdateClubMessageDto
        {
            Content = "A?urirana poruka"
        };

        var result = ((ObjectResult)controller.UpdateMessage(-1, -1, dto).Result)?.Value as ClubMessageDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Content.ShouldBe("A?urirana poruka");
        result.TimestampUpdated.ShouldNotBeNull();

        db.ChangeTracker.Clear();
        var storedMessage = db.ClubMessages.FirstOrDefault(m => m.Id == -1);
        storedMessage.ShouldNotBeNull();
        storedMessage.Content.ShouldBe("A?urirana poruka");
    }

    [Fact]
    public void Non_author_cannot_update_message()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12"); // nije autor poruke -1

        var dto = new UpdateClubMessageDto
        {
            Content = "Poku?aj neovla??enog a?uriranja"
        };

        var result = controller.UpdateMessage(-1, -1, dto).Result as ForbidResult;
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Author_deletes_own_message_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12"); // autor poruke -2
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var result = controller.DeleteMessage(-1, -2) as NoContentResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        db.ChangeTracker.Clear();
        db.ClubMessages.FirstOrDefault(m => m.Id == -2).ShouldBeNull();
    }

    [Fact]
    public void Club_owner_deletes_member_message_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11"); // vlasnik kluba -1
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var result = controller.DeleteMessage(-1, -4) as NoContentResult; // poruka autora -12

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        db.ChangeTracker.Clear();
        db.ClubMessages.FirstOrDefault(m => m.Id == -4).ShouldBeNull();
    }

    [Fact]
    public void Non_authorized_user_cannot_delete_message()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // nije ?lan kluba -1 niti vlasnik

        var result = controller.DeleteMessage(-1, -1) as ForbidResult;
        result.ShouldNotBeNull();
    }

    private static ClubMessagesController CreateController(IServiceScope scope, string userId = "-11")
    {
        return new ClubMessagesController(scope.ServiceProvider.GetRequiredService<IClubMessageService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
