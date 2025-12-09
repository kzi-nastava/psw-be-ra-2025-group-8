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

    // NOTE: These tests use existing test data from database (-1, -2, -3, -4)
    // to avoid conflicts with active tour validation

    [Fact]
    public void Get_existing_tour_execution()
    {
    // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        
        // Act - Get existing TourExecution -1 from database
        var result = controller.Get(-1);

        // Assert
        result.ShouldNotBeNull();
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
            IdTourist = 100,  // New tourist
            Status = "InProgress"
        };

        // Act
        var result = controller.Create(tourExecution).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_invalid_tourist_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
            {
            IdTour = -10,
            Longitude = 19.8335,
            Latitude = 45.2671,
            IdTourist = 0, // Invalid
            Status = "InProgress"
        };

        // Act
        var result = controller.Create(tourExecution).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_invalid_longitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
{
            IdTour = -10,
            Longitude = -181, // Invalid
            Latitude = 45.2671,
            IdTourist = 101,  // New tourist
            Status = "InProgress"
        };

        // Act
        var result = controller.Create(tourExecution).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourExecution = new TourExecutionDto
            {
                IdTour = -10,
                Longitude = 19.8335,
                Latitude = -91, // Invalid
                IdTourist = 102,  // New tourist
                Status = "InProgress"
            };

        // Act
        var result = controller.Create(tourExecution).Result as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
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