using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourRecommendationTests : BaseToursIntegrationTest
{
    public TourRecommendationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_recommendations_for_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-1);
        result.GeneratedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }

    [Fact]
    public void Returns_empty_recommendations_for_author_with_no_tours()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999"); // Non-existent author

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-999);
        result.Recommendations.ShouldBeEmpty();
    }

    [Fact]
    public void Recommendations_have_valid_structure()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();

        foreach (var recommendation in result.Recommendations)
        {
            recommendation.TourId.ShouldNotBe(0);
            recommendation.TourName.ShouldNotBeNullOrEmpty();
            recommendation.Message.ShouldNotBeNullOrEmpty();
            recommendation.Sentiment.ShouldBeOneOf(RecommendationSentiment.Positive, RecommendationSentiment.Negative);
            recommendation.Category.ShouldBeOneOf(RecommendationCategory.General, RecommendationCategory.Economic);
        }
    }

    [Fact]
    public void Recommendations_are_for_authors_tours_only()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        var authorTourIds = tourService.GetByAuthor(-1).Select(t => t.Id).ToList();

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();

        foreach (var recommendation in result.Recommendations)
        {
            authorTourIds.ShouldContain(recommendation.TourId);
        }
    }

    [Fact]
    public void Positive_recommendations_have_correct_sentiment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();

        var positiveRecommendations = result.Recommendations
            .Where(r => r.Sentiment == RecommendationSentiment.Positive)
            .ToList();

        foreach (var recommendation in positiveRecommendations)
        {
            // Positive recommendations should have encouraging messages
            recommendation.Message.ShouldNotContain("predugačka");
            recommendation.Message.ShouldNotContain("skraćivanje");
        }
    }

    [Fact]
    public void Negative_recommendations_have_correct_sentiment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        // Act
        var result = ((ObjectResult)controller.GetMyRecommendations().Result)?.Value as AuthorRecommendationsDto;

        // Assert
        result.ShouldNotBeNull();

        var negativeRecommendations = result.Recommendations
            .Where(r => r.Sentiment == RecommendationSentiment.Negative)
            .ToList();

        foreach (var recommendation in negativeRecommendations)
        {
            // Negative recommendations should contain improvement suggestions
            (recommendation.Message.Contains("Razmislite") ||
             recommendation.Message.Contains("Preporučujemo") ||
             recommendation.Message.Contains("prilagođavanju")).ShouldBeTrue();
        }
    }

    private static TourRecommendationController CreateController(IServiceScope scope, string authorId)
    {
        return new TourRecommendationController(
            scope.ServiceProvider.GetRequiredService<ITourRecommendationService>()
        )
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}
