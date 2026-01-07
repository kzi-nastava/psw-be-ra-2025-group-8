using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Explorer.API.Controllers;
using Explorer.API.Controllers.Administrator;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.Notifications;

[Collection("Sequential")]
public class WalletTopUpNotificationTests : BaseStakeholdersIntegrationTest
{
    public WalletTopUpNotificationTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void DepositCoins_creates_WalletTopUp_notification()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // 1) Register tourist (unique username)
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var account = new AccountRegistrationDto
        {
            Username = $"notif_topup_{suffix}@test.com",
            Email = $"notif_topup_{suffix}@test.com",
            Password = "test123",
            Name = "Notif",
            Surname = "Topup"
        };

        var authController = new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());

        var tokens = ((ObjectResult)authController.RegisterTourist(account).Result).Value as AuthenticationTokensDto;
        tokens.ShouldNotBeNull();
        var userId = tokens!.Id;

        // 2) Admin deposits coins
        var adminWalletController = new AdminWalletController(scope.ServiceProvider.GetRequiredService<IWalletService>());
        var depositDto = new DepositCoinsDto { UserId = userId, Amount = 123 };

        var depositAction = adminWalletController.DepositCoins(depositDto);
        (depositAction.Result as OkObjectResult).ShouldNotBeNull();

        // 3) Assert notification exists in DB
        dbContext.ChangeTracker.Clear();

        var notification = dbContext.Notifications
            .SingleOrDefault(n => n.UserId == userId && n.Type == NotificationType.WalletTopUp);

        notification.ShouldNotBeNull();
        notification!.IsRead.ShouldBeFalse();
        notification.Title.ShouldBe("Wallet top-up");
        notification.Content.ShouldContain("credited");
        notification.RelatedEntityType.ShouldBe("Wallet");
    }
}
