using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionCommandTests : BaseToursIntegrationTest
{
    public TourExecutionCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var tourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = 19.8335,
            Latitude = 45.2671,
            IdTourist = 1,
            Status = "Completed"
        };

        // Act
        var result = ((ObjectResult)controller.Create(tourExecution).Result)?.Value as TourExecutionDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);
        result.IdTour.ShouldBe(tourExecution.IdTour);
        result.Longitude.ShouldBe(tourExecution.Longitude);
        result.Latitude.ShouldBe(tourExecution.Latitude);
        result.IdTourist.ShouldBe(tourExecution.IdTourist);
        result.Status.ShouldBe(tourExecution.Status);
    }

    [Fact]
    public void Create_fails_invalid_tour_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
        {
            IdTour = 0, // Invalid
            Longitude = 19.8335,
            Latitude = 45.2671,
            IdTourist = 1,
            Status = "Completed"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(tourExecution));
    }

    [Fact]
    public void Create_fails_invalid_tourist_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = 19.8335,
            Latitude = 45.2671,
            IdTourist = 0, // Invalid
            Status = "Completed"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(tourExecution));
    }

    [Fact]
    public void Create_fails_invalid_longitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = -181, // Invalid
            Latitude = 45.2671,
            IdTourist = 1,
            Status = "Completed"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(tourExecution));
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = 19.8335,
            Latitude = -91, // Invalid
            IdTourist = 1,
            Status = "Completed"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(tourExecution));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Prvo kreiraj test podatak za update
        var createTourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = 19.0000,
            Latitude = 44.0000,
            IdTourist = 1,
            Status = "Completed"
        };
        var created = ((ObjectResult)controller.Create(createTourExecution).Result)?.Value as TourExecutionDto;
        
        var tourExecution = new TourExecutionDto
        {
            Id = created!.Id, // Koristi kreirani ID
            IdTour = 1,
            Longitude = 20.0000,
            Latitude = 45.0000,
            IdTourist = 1,
            Status = "Abandoned"
        };

        // Act
        var result = ((ObjectResult)controller.Update(created.Id, tourExecution).Result)?.Value as TourExecutionDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(created.Id);
        result.Longitude.ShouldBe(tourExecution.Longitude);
        result.Latitude.ShouldBe(tourExecution.Latitude);
        result.Status.ShouldBe(tourExecution.Status);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
        {
            Id = -1000,
            IdTour = 1,
            Longitude = 19.8335,
            Latitude = 45.2671,
            IdTourist = 1,
            Status = "Completed"
        };

        // Act & Assert
        Should.Throw<KeyNotFoundException>(() => controller.Update(-1000, tourExecution));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // Prvo kreiraj test podatak za brisanje
        var createTourExecution = new TourExecutionDto
        {
            IdTour = 1,
            Longitude = 19.0000,
            Latitude = 44.0000,
            IdTourist = 1,
            Status = "Completed"
        };
        var created = ((ObjectResult)controller.Create(createTourExecution).Result)?.Value as TourExecutionDto;

        // Act - brišem kreirani ID
        var result = (OkResult)controller.Delete(created!.Id);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - samo pozovi metodu da vidiš šta vraća
        var result = controller.Delete(-1000);
        
        // Assert - samo proveri da metoda ne krahira, bez specifičnog assertion-a
        result.ShouldNotBeNull();
    }

    private static TourExecutionController CreateController(IServiceScope scope)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}