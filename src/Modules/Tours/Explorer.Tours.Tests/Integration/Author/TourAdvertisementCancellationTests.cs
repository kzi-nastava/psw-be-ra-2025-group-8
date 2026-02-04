using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourAdvertisementCancellationTests : BaseToursIntegrationTest
{
    public TourAdvertisementCancellationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void CancelAdvertisement_calculates_refund_and_deactivates_ad()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Controllers
        var authorController = CreateAuthorController(scope, "-12");
        var touristController = CreateTouristController(scope, "-1");

        // Arrange
        const long tourId = -9001;
        const int authorId = -12;

        Cleanup(db, tourId);

        SeedPublishedTour(db, tourId, price: 77777m, difficulty: 2, authorId: authorId);

        // Reklama: U=200 coins, trajanje ukupno 10 dana, preostalo 5 dana => ratio=0.5
        // Setup fee S=20% => 40
        // Refund base (U-S)=160
        // P = 160 * 0.5 = 80
        var now = DateTime.UtcNow;
        var purchasedAt = now.AddDays(-5);
        var endsAt = now.AddDays(5);


        SeedAdvertisement(db, tourId, authorId, TourAdvertisementTier.Premium,
            coinsSpent: 200,
            purchasedAtUtc: purchasedAt,
            endsAtUtc: endsAt);

        db.ChangeTracker.Clear();

        // Sanity: pre cancel mora biti advertised u listi
        var before = touristController.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 77777,
            maxPrice: 77777
        );

        var okBefore = before.ShouldBeOfType<OkObjectResult>();
        var listBefore = okBefore.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();
        listBefore.Count.ShouldBe(1);
        listBefore[0].IsAdvertised.ShouldBeTrue();

        // Act
        var cancelResult = authorController.CancelAdvertisement(tourId);

        // Assert response
        var ok = cancelResult.Result.ShouldBeOfType<OkObjectResult>();
        var dto = ok.Value.ShouldBeAssignableTo<CancelTourAdvertisementResultDto>();

        dto.TourId.ShouldBe(tourId);
        var expectedRefund = ExpectedRefund(U: 200, purchasedAt, endsAt, dto.CancelledAtUtc);
        dto.RefundedAdventureCoins.ShouldBe(expectedRefund);
        dto.CancelledAtUtc.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));

        // Assert: ad is not active anymore (EndsAt set to now)
        var ad = db.TourAdvertisements.Single(a => a.TourId == tourId);
        ad.EndsAtUtc.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddSeconds(2));

        // Assert: tourist listing no longer marks it as advertised
        var after = touristController.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 77777,
            maxPrice: 77777
        );

        var okAfter = after.ShouldBeOfType<OkObjectResult>();
        var listAfter = okAfter.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();
        listAfter.Count.ShouldBe(1);
        listAfter[0].IsAdvertised.ShouldBeFalse();
        listAfter[0].AdvertisementTier.ShouldBeNull();
        listAfter[0].AdvertisementEndsAtUtc.ShouldBeNull();
    }

    // ---------- helpers ----------

    private static TourController CreateAuthorController(IServiceScope scope, string personId)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<IBundleService>()
        )
        {
            ControllerContext = BuildContext(personId)
        };
    }



    private static TouristTourController CreateTouristController(IServiceScope scope, string personId)
    {
        return new TouristTourController(scope.ServiceProvider.GetRequiredService<ITouristTourService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static int ExpectedRefund(int U, DateTime purchasedAtUtc, DateTime endsAtUtc, DateTime nowUtc)
    {
        // S = 20% od U
        var S = (int)Math.Floor(U * 0.20);
        var refundableBase = U - S;

        var total = endsAtUtc - purchasedAtUtc;
        var remaining = endsAtUtc - nowUtc;

        if (refundableBase <= 0) return 0;
        if (total.TotalSeconds <= 0) return 0;
        if (remaining.TotalSeconds <= 0) return 0;

        var ratio = remaining.TotalSeconds / total.TotalSeconds;
        var P = (int)Math.Floor(refundableBase * ratio);

        if (P < 0) return 0;
        if (P > U) return U;

        return P;
    }


    private static void SeedPublishedTour(ToursContext db, long id, decimal price, int difficulty, int authorId)
    {
        var tour = new Tour(
            id: id,
            name: $"Cancel Test Tour {id}",
            description: "Tour for cancel/refund tests",
            difficulty: difficulty,
            status: TourStatus.Published,
            price: price,
            authorId: authorId
        );

        db.Tours.Add(tour);
        db.SaveChanges();
    }

    private static void SeedAdvertisement(
        ToursContext db,
        long tourId,
        int authorId,
        TourAdvertisementTier tier,
        int coinsSpent,
        DateTime purchasedAtUtc,
        DateTime endsAtUtc)
    {
        db.TourAdvertisements.Add(new TourAdvertisement(
            tourId: tourId,
            authorId: authorId,
            tier: tier,
            coinsSpent: coinsSpent,
            purchasedAtUtc: purchasedAtUtc,
            endsAtUtc: endsAtUtc
        ));

        db.SaveChanges();
    }

    private static void Cleanup(ToursContext db, params long[] tourIds)
    {
        var ads = db.TourAdvertisements.Where(a => tourIds.Contains(a.TourId)).ToList();
        if (ads.Count > 0)
        {
            db.TourAdvertisements.RemoveRange(ads);
            db.SaveChanges();
        }

        var tours = db.Tours.Where(t => tourIds.Contains(t.Id)).ToList();
        if (tours.Count > 0)
        {
            db.Tours.RemoveRange(tours);
            db.SaveChanges();
        }
    }
}
