using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class ReportProblemCommandTests : BaseToursIntegrationTest
{
    public ReportProblemCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new ReportProblemDto
        {
            TourId = -1, // Use existing negative TourId from test data
            TouristId = 1,
            Category = 0, // Technical
            Priority = 2, // High
            Description = "Test problem description for creation"
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as ReportProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(newEntity.TourId);
        result.TouristId.ShouldBe(newEntity.TouristId);
        result.Category.ShouldBe(newEntity.Category);
        result.Priority.ShouldBe(newEntity.Priority);
        result.Description.ShouldBe(newEntity.Description);

        // Promeni assertion za ReportTime - proveri da NIJE default vrednost
        result.ReportTime.ShouldBeGreaterThan(DateTime.MinValue); // ← Promena ovde
        result.ReportTime.ShouldBeLessThanOrEqualTo(DateTime.UtcNow); // ← I dodaj ovo

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity!.TourId.ShouldBe(newEntity.TourId);
        storedEntity.Description.ShouldBe(newEntity.Description);
    }

    [Fact]
    public void Create_fails_invalid_tour_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new ReportProblemDto
        {
            TourId = 0, // Invalid
            TouristId = 1,
            Category = 0,
            Priority = 1,
            Description = "Test description"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_invalid_tourist_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new ReportProblemDto
        {
            TourId = -1, // Valid TourId
            TouristId = 0, // Invalid
            Category = 0,
            Priority = 1,
            Description = "Test description"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_empty_description()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new ReportProblemDto
        {
            TourId = -1, // Valid TourId
            TouristId = 1,
            Category = 0,
            Priority = 1,
            Description = "" // Invalid
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new ReportProblemDto
        {
            Id = -1,
            TourId = -1, // Use negative TourId
            TouristId = 1,
            Category = 1, // Safety
            Priority = 3, // Critical
            Description = "Updated problem description for testing"
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as ReportProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(-1);
        result.TourId.ShouldBe(updatedEntity.TourId);
        result.TouristId.ShouldBe(updatedEntity.TouristId);
        result.Category.ShouldBe(updatedEntity.Category);
        result.Priority.ShouldBe(updatedEntity.Priority);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity!.Category.ShouldBe((Explorer.Tours.Core.Domain.ReportCategory)updatedEntity.Category);
        storedEntity.Priority.ShouldBe((Explorer.Tours.Core.Domain.ReportPriority)updatedEntity.Priority);
        storedEntity.Description.ShouldBe(updatedEntity.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new ReportProblemDto
        {
            Id = -1000,
            TourId = -1, // Use negative TourId
            TouristId = 1,
            Category = 0,
            Priority = 1,
            Description = "Test description"
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // First create a ReportProblem to delete (to avoid dependency on test data)
        var newEntity = new ReportProblemDto
        {
            TourId = -1, // Use existing negative TourId from test data
            TouristId = 1,
            Category = 0, // Technical
            Priority = 2, // High
            Description = "Test problem to be deleted"
        };
        var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as ReportProblemDto;
        created.ShouldNotBeNull();
        var idToDelete = created!.Id;

        // Act
        var result = (OkResult)controller.Delete(idToDelete);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == idToDelete);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }
    /*
    [Fact]
    public void CloseIssueByAdmin_sets_IsClosedByAdmin()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReportProblemService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = service.CloseIssueByAdmin(-1);
        result.ShouldNotBeNull();
        result.IsClosedByAdmin.ShouldNotBeNull();
        result.IsClosedByAdmin.Value.ShouldBeTrue();

        var stored = db.ReportProblem.Find(-1L);
        stored.ShouldNotBeNull();
        stored!.IsClosedByAdmin.ShouldNotBeNull();
        stored.IsClosedByAdmin.Value.ShouldBeTrue();
    }

    [Fact]
    public void PenalizeAuthor_sets_flag_and_archives_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReportProblemService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Use a report that references a tour which exists in seeded data (e.g. report -4 -> tour -3)
        var result = service.PenalizeAuthor(-4);
        result.ShouldNotBeNull();
        result.IsAuthorPenalized.ShouldNotBeNull();
        result.IsAuthorPenalized.Value.ShouldBeTrue();

        var stored = db.ReportProblem.Find(-4L);
        stored.ShouldNotBeNull();
        stored!.IsAuthorPenalized.ShouldNotBeNull();
        stored.IsAuthorPenalized.Value.ShouldBeTrue();

        // Verify corresponding tour is archived
        var tour = db.Tours.Find((long)stored.TourId);
        tour.ShouldNotBeNull();
        tour!.Status.ShouldBe(TourStatus.Archived);
    }
    */


    private static ReportProblemController CreateController(IServiceScope scope)
    {
        return new ReportProblemController(
            scope.ServiceProvider.GetRequiredService<IReportProblemService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Author.ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
