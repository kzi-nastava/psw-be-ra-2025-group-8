using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Tests.TestHelpers;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class BundlePurchaseWithWalletTests : BasePaymentsIntegrationTest
{
    public BundlePurchaseWithWalletTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Purchase_bundle_successfully_with_sufficient_AC()
    {
        using var scope = Factory.Services.CreateScope();
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var bundlePurchaseService = scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>();

        long userId = -21; // turista1
        long bundleId = MockBundleInfoProvider.ExistingBundleId;

        // cleanup for repeatability
        paymentsDb.BundlePurchaseRecords.RemoveRange(
            paymentsDb.BundlePurchaseRecords.Where(r => r.TouristId == userId && r.BundleId == bundleId)
        );
        paymentsDb.PurchasedItems.RemoveRange(
            paymentsDb.PurchasedItems.Where(pi => pi.UserId == userId &&
                MockBundleInfoProvider.ExistingBundleTourIds.Contains(pi.TourId))
        );
        paymentsDb.SaveChanges();
        paymentsDb.ChangeTracker.Clear();

        stakeholdersDb.ChangeTracker.Clear();
        var walletBefore = stakeholdersDb.Wallets.FirstOrDefault(w => w.UserId == userId);
        walletBefore.ShouldNotBeNull();
        var beforeBalance = walletBefore!.AdventureCoins;

        var requiredCoins = (int)Math.Ceiling(MockBundleInfoProvider.ExistingBundlePrice);
        beforeBalance.ShouldBeGreaterThanOrEqualTo(requiredCoins);

        // Act
        var result = bundlePurchaseService.PurchasePublishedBundle(userId, bundleId);

        // Assert response
        result.ShouldNotBeNull();
        result.BundleId.ShouldBe(bundleId);
        result.TouristId.ShouldBe(userId);
        result.Price.ShouldBe(MockBundleInfoProvider.ExistingBundlePrice);
        result.AdventureCoinsSpent.ShouldBe(requiredCoins);
        result.TourIds.ShouldBe(MockBundleInfoProvider.ExistingBundleTourIds, ignoreOrder: true);

        // Assert wallet deducted
        stakeholdersDb.ChangeTracker.Clear();
        var walletAfter = stakeholdersDb.Wallets.First(w => w.UserId == userId);
        walletAfter.AdventureCoins.ShouldBe(beforeBalance - requiredCoins);

        // Assert purchased items (once per tour, price/coins are 0 for direct purchases)
        paymentsDb.ChangeTracker.Clear();
        var cart = paymentsDb.ShoppingCarts
            .Include(c => c.PurchasedItems)
            .FirstOrDefault(c => c.UserId == userId);
        cart.ShouldNotBeNull();

        foreach (var tourId in MockBundleInfoProvider.ExistingBundleTourIds)
        {
            var purchased = cart!.PurchasedItems.FirstOrDefault(pi => pi.TourId == tourId);
            purchased.ShouldNotBeNull();
            purchased!.Price.ShouldBe(0m);
            purchased.AdventureCoinsSpent.ShouldBe(0);
        }

        // Assert purchase record created
        var record = paymentsDb.BundlePurchaseRecords
            .FirstOrDefault(r => r.TouristId == userId && r.BundleId == bundleId);
        record.ShouldNotBeNull();
        record!.AdventureCoinsSpent.ShouldBe(requiredCoins);
        record.Price.ShouldBe(MockBundleInfoProvider.ExistingBundlePrice);
    }

    [Fact]
    public void Purchase_bundle_fails_with_insufficient_AC()
    {
        using var scope = Factory.Services.CreateScope();
        var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var bundlePurchaseService = scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>();

        long userId = -22; // turista2

        stakeholdersDb.ChangeTracker.Clear();
        var wallet = stakeholdersDb.Wallets.FirstOrDefault(w => w.UserId == userId);
        wallet.ShouldNotBeNull();

        // reduce wallet to 1 AC
        if (wallet!.AdventureCoins > 1)
        {
            wallet.DeductCoins(wallet.AdventureCoins - 1);
            stakeholdersDb.SaveChanges();
        }

        Should.Throw<InvalidOperationException>(() =>
            bundlePurchaseService.PurchasePublishedBundle(userId, MockBundleInfoProvider.ExistingBundleId)
        );
    }

    [Fact]
    public void Purchase_bundle_fails_when_bundle_not_found()
    {
        using var scope = Factory.Services.CreateScope();
        var bundlePurchaseService = scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>();

        Should.Throw<NotFoundException>(() =>
            bundlePurchaseService.PurchasePublishedBundle(-21, -999999)
        );
    }

    [Fact]
    public void Purchased_items_are_not_duplicated_if_bundle_is_bought_twice()
    {
        using var scope = Factory.Services.CreateScope();
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var bundlePurchaseService = scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>();

        long userId = -21;
        long bundleId = MockBundleInfoProvider.ExistingBundleId;

        paymentsDb.PurchasedItems.RemoveRange(
            paymentsDb.PurchasedItems.Where(pi => pi.UserId == userId &&
                MockBundleInfoProvider.ExistingBundleTourIds.Contains(pi.TourId))
        );
        paymentsDb.SaveChanges();
        paymentsDb.ChangeTracker.Clear();

        // Act: buy twice
        bundlePurchaseService.PurchasePublishedBundle(userId, bundleId);
        bundlePurchaseService.PurchasePublishedBundle(userId, bundleId);

        // Assert: only one purchased item per tour
        paymentsDb.ChangeTracker.Clear();
        var cart = paymentsDb.ShoppingCarts
            .Include(c => c.PurchasedItems)
            .First(c => c.UserId == userId);

        var ids = cart.PurchasedItems
            .Where(pi => MockBundleInfoProvider.ExistingBundleTourIds.Contains(pi.TourId))
            .Select(pi => pi.TourId)
            .ToList();

        ids.Distinct().Count().ShouldBe(MockBundleInfoProvider.ExistingBundleTourIds.Count);
        ids.Count.ShouldBe(MockBundleInfoProvider.ExistingBundleTourIds.Count);
    }
}
