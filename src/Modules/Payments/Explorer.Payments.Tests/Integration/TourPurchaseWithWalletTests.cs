using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class TourPurchaseWithWalletTests : BasePaymentsIntegrationTest
{
    public TourPurchaseWithWalletTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Purchase_tour_successfully_with_sufficient_AC()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Use existing test user: turista1 (-21) has 1000 AC from seed data
        long userId = -21;

        paymentsDb.ChangeTracker.Clear();
        stakeholdersDb.ChangeTracker.Clear();

        // Get or create cart
        try
        {
            cartService.GetCart(userId);
        }
        catch
        {
            cartService.CreateCart(userId);
        }

        // Add tour to cart (tour -511 costs 50 AC)
        try
        {
            cartService.AddItem(userId, new OrderItemDto { TourId = -511 });
        }
        catch (InvalidOperationException)
        {
            // Item already in cart, continue
        }

        // Act
        cartService.PurchaseItem(userId, -511);

        // Assert
        stakeholdersDb.ChangeTracker.Clear();
        var wallet = stakeholdersDb.Wallets.FirstOrDefault(w => w.UserId == userId);
        wallet.ShouldNotBeNull();
        wallet.AdventureCoins.ShouldBeLessThan(1000);
    }

    [Fact]
    public void Purchase_tour_fails_with_insufficient_AC()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Use test user with low balance: turista2 (-22) has 500 AC
        long userId = -22;
        
        paymentsDb.ChangeTracker.Clear();
        stakeholdersDb.ChangeTracker.Clear();

        // Manually reduce wallet balance to 1 AC for this test
        var wallet = stakeholdersDb.Wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet != null && wallet.AdventureCoins > 1)
        {
            // Deduct enough to leave only 1 AC
            wallet.DeductCoins(wallet.AdventureCoins - 1);
            stakeholdersDb.SaveChanges();
        }

        // Get or create cart
        try
        {
            cartService.GetCart(userId);
        }
        catch
        {
            cartService.CreateCart(userId);
        }

        // Add expensive tour to cart
        try
        {
            cartService.AddItem(userId, new OrderItemDto { TourId = -522 }); // 100 AC
        }
        catch (InvalidOperationException)
        {
            // Item already in cart
        }

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => cartService.PurchaseItem(userId, -522))
            .Message.ShouldContain("Insufficient Adventure Coins");
    }

    [Fact]
    public void Purchase_all_items_successfully_with_sufficient_AC()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Use turista3 (-23) with 2000 AC
        long userId = -23;

        paymentsDb.ChangeTracker.Clear();
        stakeholdersDb.ChangeTracker.Clear();

        // Get or create cart
        try
        {
            cartService.GetCart(userId);
            cartService.ClearCart(userId); // Clear any existing items
        }
        catch
        {
            cartService.CreateCart(userId);
        }

        // Add two tours
        cartService.AddItem(userId, new OrderItemDto { TourId = -511 }); // 50 AC
        cartService.AddItem(userId, new OrderItemDto { TourId = -533 }); // 70 AC

        // Act
        cartService.PurchaseAllItems(userId);

        // Assert
        stakeholdersDb.ChangeTracker.Clear();
        var wallet = stakeholdersDb.Wallets.FirstOrDefault(w => w.UserId == userId);
        wallet.ShouldNotBeNull();
        wallet.AdventureCoins.ShouldBeLessThan(2000);
    }
}

