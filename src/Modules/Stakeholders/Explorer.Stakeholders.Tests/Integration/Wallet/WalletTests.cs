using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Administrator;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Explorer.Stakeholders.Tests.Integration.Wallet;

[Collection("Sequential")]
public class WalletTests : BaseStakeholdersIntegrationTest
{
    public WalletTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Wallet_created_automatically_on_tourist_registration()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var authController = CreateAuthenticationController(scope);
        var account = new AccountRegistrationDto
        {
            Username = "wallettest1@test.com",
            Email = "wallettest1@test.com",
            Password = "test123",
            Name = "Wallet",
            Surname = "Test"
        };

        // Act
        var authenticationResponse = ((ObjectResult)authController.RegisterTourist(account).Result).Value as AuthenticationTokensDto;

        // Assert - User and Wallet created
        dbContext.ChangeTracker.Clear();
        var storedUser = dbContext.Users.FirstOrDefault(u => u.Username == account.Username);
        storedUser.ShouldNotBeNull();
        
        var storedWallet = dbContext.Wallets.FirstOrDefault(w => w.UserId == storedUser.Id);
        storedWallet.ShouldNotBeNull();
        storedWallet.AdventureCoins.ShouldBe(0);
    }

    [Fact]
    public void Tourist_can_view_own_wallet_balance()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Create a test tourist with wallet
        var user = new User("wallettest2@test.com", "password", UserRole.Tourist, true);
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        
        var person = new Person(user.Id, "Test", "User", "wallettest2@test.com");
        dbContext.People.Add(person);
        dbContext.SaveChanges();
        
        var wallet = new Core.Domain.Wallet(user.Id);
        dbContext.Wallets.Add(wallet);
        dbContext.SaveChanges();
        
        dbContext.ChangeTracker.Clear();
        
        var walletController = CreateWalletController(scope, user.Id);

        // Act
        var result = walletController.GetMyBalance();

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var walletDto = okResult.Value.ShouldBeOfType<WalletDto>();
        walletDto.UserId.ShouldBe(user.Id);
        walletDto.AdventureCoins.ShouldBe(0);
    }

    [Fact]
    public void Admin_can_deposit_coins_to_tourist_wallet()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Create a test tourist with wallet
        var user = new User("wallettest3@test.com", "password", UserRole.Tourist, true);
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        
        var person = new Person(user.Id, "Test", "User", "wallettest3@test.com");
        dbContext.People.Add(person);
        dbContext.SaveChanges();
        
        var wallet = new Core.Domain.Wallet(user.Id);
        dbContext.Wallets.Add(wallet);
        dbContext.SaveChanges();
        
        dbContext.ChangeTracker.Clear();
        
        var adminWalletController = CreateAdminWalletController(scope);
        var depositDto = new DepositCoinsDto
        {
            UserId = user.Id,
            Amount = 500
        };

        // Act
        var result = adminWalletController.DepositCoins(depositDto);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var walletDto = okResult.Value.ShouldBeOfType<WalletDto>();
        walletDto.UserId.ShouldBe(user.Id);
        walletDto.AdventureCoins.ShouldBe(500);
        
        // Verify in database
        dbContext.ChangeTracker.Clear();
        var updatedWallet = dbContext.Wallets.FirstOrDefault(w => w.UserId == user.Id);
        updatedWallet.ShouldNotBeNull();
        updatedWallet.AdventureCoins.ShouldBe(500);
    }

    [Fact]
    public void Admin_can_view_any_user_wallet()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Create a test tourist with wallet
        var user = new User("wallettest4@test.com", "password", UserRole.Tourist, true);
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        
        var person = new Person(user.Id, "Test", "User", "wallettest4@test.com");
        dbContext.People.Add(person);
        dbContext.SaveChanges();
        
        var wallet = new Core.Domain.Wallet(user.Id);
        dbContext.Wallets.Add(wallet);
        dbContext.SaveChanges();
        
        dbContext.ChangeTracker.Clear();
        
        var adminWalletController = CreateAdminWalletController(scope);

        // Act
        var result = adminWalletController.GetWalletByUserId(user.Id);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var walletDto = okResult.Value.ShouldBeOfType<WalletDto>();
        walletDto.UserId.ShouldBe(user.Id);
        walletDto.AdventureCoins.ShouldBe(0);
    }

    [Fact]
    public void Deposit_fails_with_invalid_amount()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Create a test tourist with wallet
        var user = new User("wallettest5@test.com", "password", UserRole.Tourist, true);
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        
        var person = new Person(user.Id, "Test", "User", "wallettest5@test.com");
        dbContext.People.Add(person);
        dbContext.SaveChanges();
        
        var wallet = new Core.Domain.Wallet(user.Id);
        dbContext.Wallets.Add(wallet);
        dbContext.SaveChanges();
        
        dbContext.ChangeTracker.Clear();
        
        var adminWalletController = CreateAdminWalletController(scope);
        var depositDto = new DepositCoinsDto
        {
            UserId = user.Id,
            Amount = -100 // Invalid negative amount
        };

        // Act
        var result = adminWalletController.DepositCoins(depositDto);

        // Assert
        result.Result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Deposit_fails_for_nonexistent_user()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var adminWalletController = CreateAdminWalletController(scope);
        var depositDto = new DepositCoinsDto
        {
            UserId = 99999, // Non-existent user
            Amount = 100
        };

        // Act
        var result = adminWalletController.DepositCoins(depositDto);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Multiple_deposits_accumulate_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Create a test tourist with wallet
        var user = new User("wallettest6@test.com", "password", UserRole.Tourist, true);
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        
        var person = new Person(user.Id, "Test", "User", "wallettest6@test.com");
        dbContext.People.Add(person);
        dbContext.SaveChanges();
        
        var wallet = new Core.Domain.Wallet(user.Id);
        dbContext.Wallets.Add(wallet);
        dbContext.SaveChanges();
        
        dbContext.ChangeTracker.Clear();
        
        var adminWalletController = CreateAdminWalletController(scope);

        // Act - First deposit
        var deposit1 = new DepositCoinsDto { UserId = user.Id, Amount = 100 };
        adminWalletController.DepositCoins(deposit1);
        
        // Act - Second deposit
        var deposit2 = new DepositCoinsDto { UserId = user.Id, Amount = 250 };
        var result = adminWalletController.DepositCoins(deposit2);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var walletDto = okResult.Value.ShouldBeOfType<WalletDto>();
        walletDto.AdventureCoins.ShouldBe(350); // 100 + 250
        
        // Verify in database
        dbContext.ChangeTracker.Clear();
        var finalWallet = dbContext.Wallets.FirstOrDefault(w => w.UserId == user.Id);
        finalWallet.ShouldNotBeNull();
        finalWallet.AdventureCoins.ShouldBe(350);
    }

    private static AuthenticationController CreateAuthenticationController(IServiceScope scope)
    {
        return new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());
    }

    private static WalletController CreateWalletController(IServiceScope scope, long userId)
    {
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var controller = new WalletController(walletService);
        
        // Mock the User claims for authorization
        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString()),
            new Claim("role", "tourist")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        
        return controller;
    }

    private static AdminWalletController CreateAdminWalletController(IServiceScope scope)
    {
        return new AdminWalletController(scope.ServiceProvider.GetRequiredService<IWalletService>());
    }
}
