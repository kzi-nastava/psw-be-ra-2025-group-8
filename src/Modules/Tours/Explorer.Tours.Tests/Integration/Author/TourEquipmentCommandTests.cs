using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourEquipmentCommandTests : BaseToursIntegrationTest
{
    public TourEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    private static TourEquipmentController CreateController(IServiceScope scope)
    {
        return new TourEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            // authorId = -1 → owner of tour -10
            ControllerContext = BuildContext("-1")
        };
    }
    [Fact]
    public void GetEquipment_empty_at_start()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // očisti sve veze za turu -10 da bi test bio nezavisan
        db.TourEquipment.RemoveRange(
            db.TourEquipment.Where(te => te.TourId == -10)
        );
        db.SaveChanges();

        var controller = CreateController(scope);

        var actionResult = controller.GetEquipment(-10);
        var objectResult = actionResult.Result as ObjectResult;
        var list = objectResult?.Value as List<TourEquipmentDto>;

        list.ShouldNotBeNull();
        list.Count.ShouldBe(0);
    }

    // -------------------------------------------------------
    // AddEquipment adds a new entry
    // -------------------------------------------------------
    [Fact]
    public void AddEquipment_adds_entry_to_db()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope);

        // Act
        controller.AddEquipment(-10, -1);

        // Assert DB
        var exists = db.TourEquipment
            .FirstOrDefault(te => te.TourId == -10 && te.EquipmentId == -1);

        exists.ShouldNotBeNull();
    }

    // -------------------------------------------------------
    // RemoveEquipment removes assigned equipment
    // -------------------------------------------------------
    [Fact]
    public void RemoveEquipment_removes_entry()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var controller = CreateController(scope);

        // Ensure present
        controller.AddEquipment(-10, -3);

        // Act
        controller.RemoveEquipment(-10, -3);

        // Assert DB
        var exists = db.TourEquipment
            .FirstOrDefault(te => te.TourId == -10 && te.EquipmentId == -3);

        exists.ShouldBeNull();
    }

    // -------------------------------------------------------
    // Removing unassigned equipment does NOT fail
    // -------------------------------------------------------
    [Fact]
    public void RemoveEquipment_nonexistent_does_not_fail()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = controller.RemoveEquipment(-10, -3);
        var objectResult = actionResult.Result as ObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(200);
    }

    // -------------------------------------------------------
    // Cannot add equipment to someone else's tour
    // -------------------------------------------------------
    [Fact]
    public void AddEquipment_fails_for_other_author()
    {
        using var scope = Factory.Services.CreateScope();

        var controller = new TourEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-2") // -2 is NOT owner of tour -10
        };

        Should.Throw<UnauthorizedAccessException>(() =>
            controller.AddEquipment(-10, -1));
    }

    // -------------------------------------------------------
    // Cannot remove equipment from someone else's tour
    // -------------------------------------------------------
    [Fact]
    public void RemoveEquipment_fails_for_other_author()
    {
        using var scope = Factory.Services.CreateScope();

        var controller = new TourEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-2")
        };

        Should.Throw<UnauthorizedAccessException>(() =>
            controller.RemoveEquipment(-10, -1));
    }

    // -------------------------------------------------------
    // AddEquipment throws if tour does not exist
    // -------------------------------------------------------
    [Fact]
    public void AddEquipment_fails_for_missing_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<KeyNotFoundException>(() =>
            controller.AddEquipment(-9999, -1));
    }

    // -------------------------------------------------------
    // AddEquipment throws if equipment does not exist
    // -------------------------------------------------------
    [Fact]
    public void AddEquipment_fails_for_missing_equipment()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<Exception>(() =>
            controller.AddEquipment(-10, -9999));
    }

    // -------------------------------------------------------
    // GetEquipment returns correct list after add/remove
    // -------------------------------------------------------
    [Fact]
    public void GetEquipment_after_add_remove()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // kreni od čistog stanja za turu -10
        db.TourEquipment.RemoveRange(
            db.TourEquipment.Where(te => te.TourId == -10)
        );
        db.SaveChanges();

        var controller = CreateController(scope);

        controller.AddEquipment(-10, -1);
        controller.AddEquipment(-10, -2);

        controller.RemoveEquipment(-10, -1);

        var actionResult = controller.GetEquipment(-10);
        var objectResult = actionResult.Result as ObjectResult;
        var list = objectResult?.Value as List<TourEquipmentDto>;

        list.ShouldNotBeNull();
        list.Count.ShouldBe(1);
        list[0].EquipmentId.ShouldBe(-2);
    }
}
