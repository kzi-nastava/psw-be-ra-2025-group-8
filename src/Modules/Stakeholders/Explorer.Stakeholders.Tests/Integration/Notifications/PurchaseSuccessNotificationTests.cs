using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.Notifications;

[Collection("Sequential")]
public class PurchaseSuccessNotificationTests : BaseStakeholdersIntegrationTest
{
    public PurchaseSuccessNotificationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void NotifyTourPurchased_creates_PurchaseSuccess_notification()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // 1) Register tourist
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var account = new AccountRegistrationDto
        {
            Username = $"notif_purchase_{suffix}@test.com",
            Email = $"notif_purchase_{suffix}@test.com",
            Password = "test123",
            Name = "Notif",
            Surname = "Purchase"
        };

        var authController = new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());

        var tokens = ((ObjectResult)authController.RegisterTourist(account).Result).Value as AuthenticationTokensDto;
        tokens.ShouldNotBeNull();
        var userId = tokens!.Id;


        // 2) Trigger purchase notification via adapter
        var purchaseNotif = scope.ServiceProvider.GetRequiredService<IPurchaseNotificationService>();
        var tourId = 999;

        purchaseNotif.NotifyTourPurchased(userId, tourId);

        // 3) Assert notification exists in DB
        dbContext.ChangeTracker.Clear();

        var notification = dbContext.Notifications
            .SingleOrDefault(n =>
                n.UserId == userId &&
                n.Type == NotificationType.PurchaseSuccess &&
                n.RelatedEntityId == tourId);

        notification.ShouldNotBeNull();
        notification!.IsRead.ShouldBeFalse();
        notification.Title.ShouldBe("Purchase successful");
        notification.RelatedEntityType.ShouldBe("Tour");
    }
}
