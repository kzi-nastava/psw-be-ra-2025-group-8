using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionKeyPointTests : BaseToursIntegrationTest
{
    public TourExecutionKeyPointTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void CheckKeyPoint_TouristNearNextKeyPoint_ReturnsTrue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // TourExecution -1 ima dostignuta 2 keypointa, sledeći je OrderNum=3
        // Keypoint 3 je na poziciji: Lat=45.2396, Lon=19.8227
        var request = new CheckKeyPointRequestDto
        {
            TourExecutionId = -1,
            Latitude = 45.2398,  // Samo 22m od keypoint-a 3 (bilo je 45.2400)
            Longitude = 19.8228  // Bilo je 19.8230
        };

        // Act
        var result = ((ObjectResult)controller.CheckKeyPoint(request).Result)?.Value as CheckKeyPointResponseDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.KeyPointReached.ShouldBeTrue();
        result.KeyPointOrder.ShouldBe(3);
        result.ReachedAt.ShouldNotBeNull();

        // Assert - Database
        var reachedKeyPoint = dbContext.KeyPointsReached
            .FirstOrDefault(kpr => kpr.TourExecutionId == -1 && kpr.KeyPointOrder == 3);
        reachedKeyPoint.ShouldNotBeNull();
        reachedKeyPoint.ReachedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));

        // Assert - LastActivity updated
        var tourExecution = dbContext.TourExecutions.Find(-1L);
        tourExecution.LastActivity.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void CheckKeyPoint_TouristFarFromNextKeyPoint_ReturnsFalse()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // TourExecution -3 ima dostignut 1 keypoint, sledeći je OrderNum=2
        // Keypoint 2 je na poziciji: Lat=45.2551, Lon=19.8451
        var request = new CheckKeyPointRequestDto
        {
            TourExecutionId = -3,
            Latitude = 45.2671,  // Daleko od keypoint-a 2 (preko 1km)
            Longitude = 19.8335
        };

        // Act
        var result = ((ObjectResult)controller.CheckKeyPoint(request).Result)?.Value as CheckKeyPointResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.KeyPointReached.ShouldBeFalse();
        result.KeyPointOrder.ShouldBeNull();
        result.ReachedAt.ShouldBeNull();
    }

    [Fact]
    public void CheckKeyPoint_AllKeyPointsAlreadyReached_ReturnsFalse()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // TourExecution -2 ima dostignuta sva 3 keypointa
        var request = new CheckKeyPointRequestDto
        {
            TourExecutionId = -2,
            Latitude = 44.8176,
            Longitude = 20.4633
        };

        // Act
        var result = ((ObjectResult)controller.CheckKeyPoint(request).Result)?.Value as CheckKeyPointResponseDto;

        // Assert
        result.ShouldNotBeNull();
        result.KeyPointReached.ShouldBeFalse();
        result.KeyPointOrder.ShouldBeNull();
    }

    [Fact]
    public void CheckKeyPoint_UpdatesLastActivity()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var tourExecutionBefore = dbContext.TourExecutions.Find(-1L);
        var lastActivityBefore = tourExecutionBefore.LastActivity;

        // Wait a bit to ensure timestamp difference
        System.Threading.Thread.Sleep(100);

        var request = new CheckKeyPointRequestDto
        {
            TourExecutionId = -1,
            Latitude = 45.2671,
            Longitude = 19.8335
        };

        // Act
        controller.CheckKeyPoint(request);

        // Assert
        dbContext.Entry(tourExecutionBefore).Reload();
        tourExecutionBefore.LastActivity.ShouldBeGreaterThan(lastActivityBefore);
    }

    [Fact]
    public void GetReachedKeyPoints_ReturnsAllReachedKeyPoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // TourExecution -1 ima najmanje 2 dostignuta keypointa (može biti više zbog drugih testova)
        long tourExecutionId = -1;

        // Act
        var result = ((ObjectResult)controller.GetReachedKeyPoints(tourExecutionId).Result)?.Value as List<KeyPointReachedDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);  // Najmanje 2 (seed data)
        result.Any(kpr => kpr.KeyPointOrder == 1).ShouldBeTrue();
        result.Any(kpr => kpr.KeyPointOrder == 2).ShouldBeTrue();
    }

    [Fact]
    public void GetReachedKeyPoints_NoKeyPointsReached_ReturnsEmptyList()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // TourExecution -4 nema dostignutih keypointa
        long tourExecutionId = -4;

        // Act
        var result = ((ObjectResult)controller.GetReachedKeyPoints(tourExecutionId).Result)?.Value as List<KeyPointReachedDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void CheckKeyPoint_NonExistentTourExecution_ThrowsException()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var request = new CheckKeyPointRequestDto
        {
            TourExecutionId = -9999,
            Latitude = 45.0,
            Longitude = 20.0
        };

        // Act & Assert
        var result = controller.CheckKeyPoint(request).Result as NotFoundObjectResult;
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    private static TourExecutionController CreateController(IServiceScope scope)
    {
        return new TourExecutionController(
         scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext("1")  // Tourist ID = 1
        };
    }
}
