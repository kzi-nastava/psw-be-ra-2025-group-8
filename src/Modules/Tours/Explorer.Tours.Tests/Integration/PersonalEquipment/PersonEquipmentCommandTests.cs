using Explorer.API.Controllers.Tourist.Equipment;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.PersonalEquipment;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.PersonalEquipment;

[Collection("Sequential")]
public class PersonEquipmentCommandTests : BaseToursIntegrationTest
{
    public PersonEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    private static PersonEquipmentController CreateController(IServiceScope scope)
    {
        return new PersonEquipmentController(
            scope.ServiceProvider.GetRequiredService<IPersonEquipmentService>()
        )
        {
            ControllerContext = BuildContext("1")  // JWT personId = 1
        };
    }

    [Fact]
    public void Retrieves_equipment_for_person()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        db.PersonEquipment.RemoveRange(db.PersonEquipment);
        db.SaveChanges();

        db.PersonEquipment.Add(new PersonEquipment
        {
            PersonId = 1,
            EquipmentId = -1,
            AssignedAt = DateTime.UtcNow
        });

        db.PersonEquipment.Add(new PersonEquipment
        {
            PersonId = 1,
            EquipmentId = -2,
            AssignedAt = DateTime.UtcNow
        });

        db.SaveChanges();

        // Act
        var actionResult = controller.GetForPerson(1, 10);
        var objectResult = actionResult as ObjectResult;
        var result = objectResult?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(3);

        var owned = result.Results.Where(e => e.IsOwned).ToList();
        owned.Count.ShouldBe(2);

        owned.Any(e => e.EquipmentId == -1).ShouldBeTrue();
        owned.Any(e => e.EquipmentId == -2).ShouldBeTrue();
        owned.Any(e => e.EquipmentId == -3).ShouldBeFalse();
    }

    [Fact]
    public void Assigns_equipment_to_person()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        long equipmentId = -3;

        // Act
        var result = (OkResult)controller.Assign(equipmentId);

        // Assert - response
        result.StatusCode.ShouldBe(200);

        // Assert - database
        var entry = db.PersonEquipment
            .FirstOrDefault(pe => pe.PersonId == 1 && pe.EquipmentId == -3);

        entry.ShouldNotBeNull();
    }

    [Fact]
    public void Unassigns_equipment_from_person()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        long removeId = -1;

        // Act
        var result = (OkResult)controller.Unassign(removeId);

        // Assert
        result.StatusCode.ShouldBe(200);

        // Assert - database
        db.PersonEquipment
          .FirstOrDefault(pe => pe.PersonId == 1 && pe.EquipmentId == removeId)
          .ShouldBeNull();
    }

    [Fact]
    public void Assign_does_not_duplicate()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        long equipmentId = -2;

        // Act
        controller.Assign(equipmentId);
        controller.Assign(equipmentId);   // duplicate attempt

        // Assert
        var all = db.PersonEquipment
            .Where(pe => pe.PersonId == 1 && pe.EquipmentId == equipmentId)
            .ToList();

        all.Count.ShouldBe(1); // must stay unique
    }

    [Fact]
    public void Unassign_does_nothing_when_user_does_not_own_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        long notOwnedId = -3; // user 1 does not own -3 in seed

        // Act
        var result = (OkResult)controller.Unassign(notOwnedId);

        // Assert response
        result.StatusCode.ShouldBe(200);

        // Assert DB
        var stillThere = db.PersonEquipment
            .FirstOrDefault(pe => pe.PersonId == 1 && pe.EquipmentId == notOwnedId);

        stillThere.ShouldBeNull(); // not present before or after → OK
    }

    [Fact]
    public void Assign_fails_when_equipment_does_not_exist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        long invalidEquipmentId = -9999; // does not exist in Equipment table

        // Act & Assert
        Should.Throw<Exception>(() => controller.Assign(invalidEquipmentId));
    }

    [Fact]
    public void Pagination_page_1_returns_first_two_items()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetForPerson(1, 2);
        var objectResult = actionResult as ObjectResult;
        var result = objectResult?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(2);  // pageSize=2 → first 2
    }

    [Fact]
    public void Pagination_page_2_returns_last_item()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetForPerson(2, 2);
        var ot = actionResult as ObjectResult;
        var result = ot?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(1); // only 1 left (3 total)
    }

    [Fact]
    public void Pagination_page_3_returns_empty_list()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetForPerson(3, 2);
        var ot = actionResult as ObjectResult;
        var result = ot?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(0);
    }

    [Fact]
    public void Assign_updates_IsOwned_flag_in_GetForPerson()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        controller.Assign(-3);

        var actionResult = controller.GetForPerson(1, 10);
        var obj = actionResult as ObjectResult;
        var result = obj?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        var lamp = result.Results.First(e => e.EquipmentId == -3);
        lamp.IsOwned.ShouldBeTrue();
    }

    [Fact]
    public void Unassign_updates_IsOwned_flag_in_GetForPerson()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        controller.Unassign(-1);

        var actionResult = controller.GetForPerson(1, 10);
        var obj = actionResult as ObjectResult;
        var result = obj?.Value as PagedResult<EquipmentForPersonDto>;

        // Assert
        var voda = result.Results.First(e => e.EquipmentId == -1);
        voda.IsOwned.ShouldBeFalse();
    }

}
