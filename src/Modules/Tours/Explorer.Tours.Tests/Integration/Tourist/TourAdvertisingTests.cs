using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourAdvertisingTests : BaseToursIntegrationTest
{
    public TourAdvertisingTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void GetPublishedTours_orders_advertised_first_and_by_tier()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope, "-1");

        // Arrange: 3 ture u jedinstvenom price range-u (da ne pokupimo seed)
        const long premiumTourId = -8001;
        const long basicTourId = -8002;
        const long normalTourId = -8003;

        const int authorId = -12;
        const int minPrice = 55500;
        const int maxPrice = 55502;

        Cleanup(db, premiumTourId, basicTourId, normalTourId);

        SeedPublishedTour(db, premiumTourId, price: 55500m, difficulty: 1, authorId: authorId);
        SeedPublishedTour(db, basicTourId, price: 55501m, difficulty: 1, authorId: authorId);
        SeedPublishedTour(db, normalTourId, price: 55502m, difficulty: 1, authorId: authorId);

        var now = DateTime.UtcNow;
        SeedAdvertisement(db, premiumTourId, authorId, TourAdvertisementTier.Premium,
            coinsSpent: 300, purchasedAtUtc: now.AddHours(-2), endsAtUtc: now.AddDays(7));

        SeedAdvertisement(db, basicTourId, authorId, TourAdvertisementTier.Basic,
            coinsSpent: 100, purchasedAtUtc: now.AddHours(-1), endsAtUtc: now.AddDays(7));

        // Act: koristimo postojeći filter endpoint (isti kao u TourPreviewFilteringTests)
        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: minPrice,
            maxPrice: maxPrice
        );

        // Assert
        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Count.ShouldBe(3);

        tours[0].Id.ShouldBe(premiumTourId);
        tours[0].IsAdvertised.ShouldBeTrue();
        tours[0].AdvertisementTier.ShouldBe("Premium");
        tours[0].AdvertisementEndsAtUtc.ShouldNotBeNull();

        tours[1].Id.ShouldBe(basicTourId);
        tours[1].IsAdvertised.ShouldBeTrue();
        tours[1].AdvertisementTier.ShouldBe("Basic");
        tours[1].AdvertisementEndsAtUtc.ShouldNotBeNull();

        tours[2].Id.ShouldBe(normalTourId);
        tours[2].IsAdvertised.ShouldBeFalse();
        tours[2].AdvertisementTier.ShouldBeNull();
        tours[2].AdvertisementEndsAtUtc.ShouldBeNull();
    }

    [Fact]
    public void GetPublishedTour_ignores_expired_advertisement()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope, "-1");

        const long tourId = -8101;
        const int authorId = -12;

        Cleanup(db, tourId);

        SeedPublishedTour(db, tourId, price: 66666m, difficulty: 2, authorId: authorId);

        var now = DateTime.UtcNow;
        // Reklama istekla -> mora se ignorisati
        SeedAdvertisement(db, tourId, authorId, TourAdvertisementTier.Standard,
            coinsSpent: 200, purchasedAtUtc: now.AddDays(-10), endsAtUtc: now.AddMinutes(-1));

        var result = (ObjectResult)controller.GetPublishedTour(tourId);
        result.StatusCode.ShouldBe(200);

        var dto = result.Value.ShouldBeAssignableTo<TouristTourDetailsDto>();
        dto.Id.ShouldBe(tourId);

        dto.IsAdvertised.ShouldBeFalse();
        dto.AdvertisementTier.ShouldBeNull();
        dto.AdvertisementEndsAtUtc.ShouldBeNull();
    }

    // ---------------- helpers ----------------

    private static TouristTourController CreateController(IServiceScope scope, string personId)
    {
        return new TouristTourController(scope.ServiceProvider.GetRequiredService<ITouristTourService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static void SeedPublishedTour(ToursContext db, long id, decimal price, int difficulty, int authorId)
    {
        var tour = new Tour(
            id: id,
            name: $"Adv Test Tour {id}",
            description: "Tour for advertising tests",
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
