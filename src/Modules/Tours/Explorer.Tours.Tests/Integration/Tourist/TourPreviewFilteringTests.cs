using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPreviewFilteringTests : BaseToursIntegrationTest
{
    public TourPreviewFilteringTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Price_filter_min_max_returns_only_tours_in_range()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope, "-1");

        // unique price band for this test
        SeedPublishedTour(db, -5001, price: 10m, difficulty: 1, authorId: -1);
        SeedPublishedTour(db, -5002, price: 110m, difficulty: 1, authorId: -1);
        SeedPublishedTour(db, -5003, price: 210m, difficulty: 1, authorId: -1);

        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 100,
            maxPrice: 200
        );

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Select(t => t.Id).ShouldContain(-5002);
        tours.Select(t => t.Id).ShouldNotContain(-5001);
        tours.Select(t => t.Id).ShouldNotContain(-5003);
    }

    [Fact]
    public void OwnedEquipment_filter_returns_only_tours_where_all_required_equipment_is_owned()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // PersonId = 1 (iz seed-a) ima opremu -1 i -2
        var controller = CreateController(scope, "1");

        // unique price band for this test
        SeedPublishedTour(db, -5101, price: 500m, difficulty: 2, authorId: -1, equipmentIds: new long[] { -1, -2 });
        SeedPublishedTour(db, -5102, price: 500m, difficulty: 2, authorId: -1, equipmentIds: new long[] { -3 });

        var result = controller.GetPublishedTours(
            ownedEquipment: true,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 500,
            maxPrice: 500
        );

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Select(t => t.Id).ShouldContain(-5101);
        tours.Select(t => t.Id).ShouldNotContain(-5102);
    }

    [Fact]
    public void PreferenceTags_filter_returns_only_tours_matching_any_preference_tag()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // PersonId = -21 (iz seed-a) ima tagove: -201 (mountain), -202 (food)
        var controller = CreateController(scope, "-21");

        // unique price band for this test
        SeedPublishedTour(db, -5201, price: 700m, difficulty: 1, authorId: -1, tagIds: new long[] { -201 }); // match
        SeedPublishedTour(db, -5202, price: 700m, difficulty: 1, authorId: -1, tagIds: new long[] { -203 }); // no match

        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: true,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 700,
            maxPrice: 700
        );

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Select(t => t.Id).ShouldContain(-5201);
        tours.Select(t => t.Id).ShouldNotContain(-5202);
    }

    [Fact]
    public void PreferenceDifficulty_filter_returns_only_tours_allowed_by_tourist_preference()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // PersonId = -21 ima DifficultyLevel.Beginner => dozvoljene tezine: 1 i 2
        var controller = CreateController(scope, "-21");

        // unique price band for this test
        SeedPublishedTour(db, -5301, price: 900m, difficulty: 1, authorId: -1); // allowed
        SeedPublishedTour(db, -5302, price: 900m, difficulty: 2, authorId: -1); // allowed
        SeedPublishedTour(db, -5303, price: 900m, difficulty: 3, authorId: -1); // not allowed

        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: true,
            difficulties: null,
            minPrice: 900,
            maxPrice: 900
        );

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Select(t => t.Id).ShouldContain(-5301);
        tours.Select(t => t.Id).ShouldContain(-5302);
        tours.Select(t => t.Id).ShouldNotContain(-5303);
    }

    [Fact]
    public void Difficulties_filter_returns_only_tours_in_selected_difficulties()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope, "-1");

        // unique price band for this test
        SeedPublishedTour(db, -5401, price: 1100m, difficulty: 1, authorId: -1); // should match
        SeedPublishedTour(db, -5402, price: 1100m, difficulty: 3, authorId: -1); // should match
        SeedPublishedTour(db, -5403, price: 1100m, difficulty: 2, authorId: -1); // should NOT match

        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: new List<int> { 1, 3 },
            minPrice: 1100,
            maxPrice: 1100
        );

        var ok = result.ShouldBeOfType<OkObjectResult>();
        var tours = ok.Value.ShouldBeAssignableTo<List<TouristTourPreviewDto>>();

        tours.Select(t => t.Id).ShouldContain(-5401);
        tours.Select(t => t.Id).ShouldContain(-5402);
        tours.Select(t => t.Id).ShouldNotContain(-5403);
    }

    [Fact]
    public void Price_filter_returns_400_when_max_less_than_min()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        var result = controller.GetPublishedTours(
            ownedEquipment: false,
            preferenceTags: false,
            preferenceDifficulty: false,
            difficulties: null,
            minPrice: 200,
            maxPrice: 100
        );

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    // ------------------ helpers ------------------

    private static TouristTourController CreateController(IServiceScope scope, string personId)
    {
        return new TouristTourController(scope.ServiceProvider.GetRequiredService<ITouristTourService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static void SeedPublishedTour(
        ToursContext db,
        long id,
        decimal price,
        int difficulty,
        int authorId,
        long[]? tagIds = null,
        long[]? equipmentIds = null)
    {
        var tour = new Tour(
            id: id,
            name: $"Test Tour {id}",
            description: "Tour for filtering tests",
            difficulty: difficulty,
            status: TourStatus.Published,
            price: price,
            authorId: authorId
        );

        db.Tours.Add(tour);

        if (tagIds != null)
        {
            foreach (var tagId in tagIds)
            {
                db.TourTags.Add(new TourTag
                {
                    TourId = id,
                    TagsId = tagId
                });
            }
        }

        if (equipmentIds != null)
        {
            foreach (var eqId in equipmentIds)
            {
                db.TourEquipment.Add(new TourEquipment(id, eqId));
            }
        }

        db.SaveChanges();
    }
}
