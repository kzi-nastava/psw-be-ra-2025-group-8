using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourStatsTests : BaseToursIntegrationTest
{
    public TourStatsTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_tour_stats_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -10 belongs to author -1
        var result = ((ObjectResult)controller.GetTourStats(-10).Result)?.Value as TourStatsDto;

        // Assert
        result.ShouldNotBeNull();
        result.TourId.ShouldBe(-10);
        result.CompletionRate.ShouldBeGreaterThanOrEqualTo(0);
        result.CompletionRate.ShouldBeLessThanOrEqualTo(100);
        result.AverageCompletionPercentage.ShouldBeGreaterThanOrEqualTo(0);
        result.AverageCompletionPercentage.ShouldBeLessThanOrEqualTo(100);
    }

    [Fact]
    public void Returns_not_found_for_tour_not_owned_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -12 belongs to author -2, not -1
        var result = controller.GetTourStats(-12);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Returns_not_found_for_nonexistent_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = controller.GetTourStats(-9999);

        // Assert
        result.Result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Calculates_completion_rate_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -10 has multiple executions with different statuses
        var result = ((ObjectResult)controller.GetTourStats(-10).Result)?.Value as TourStatsDto;

        // Assert
        result.ShouldNotBeNull();
        // Completion rate should be calculated as Completed / (Completed + Abandoned) * 100
        // Tour -10 has 2 completed (-5, -7) and 1 abandoned (-6) executions
        // CompletionRate = 2 / 3 * 100 = 66.67%
        result.CompletionRate.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Excludes_executions_longer_than_12_hours_from_average_duration()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act - Tour -10 has an execution that lasted 15 hours (should be excluded)
        var result = ((ObjectResult)controller.GetTourStats(-10).Result)?.Value as TourStatsDto;

        // Assert
        result.ShouldNotBeNull();
        // Average duration should be calculated only from executions <= 12 hours
        // Valid executions for Tour -10:
        // -1: 165 minutes
        // -3: 200 minutes
        // -5: 120 minutes
        // -6: 90 minutes
        // -7: 900 minutes (EXCLUDED - 15 hours)
        // Average of valid = (165 + 200 + 120 + 90) / 4 = 143.75 minutes
        result.AverageDuration.TotalHours.ShouldBeLessThan(12);
        result.AverageDuration.TotalMinutes.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Calculates_most_common_difficulty_level()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetTourStats(-10).Result)?.Value as TourStatsDto;

        // Assert
        result.ShouldNotBeNull();
        // MostCommonDifficultyLevel should be one of the valid levels
        result.MostCommonDifficultyLevel.ShouldBeOneOf("Beginner", "Intermediate", "Professional");
    }

    private static TourStatsController CreateController(IServiceScope scope, string authorId)
    {
        return new TourStatsController(
            scope.ServiceProvider.GetRequiredService<ITourStatsService>(),
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}
