using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourRatingQueryTests : BaseToursIntegrationTest
    {
        public TourRatingQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_tour_ratings()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(7);
            result.TotalCount.ShouldBe(7);
        }

        [Fact]
        public void Returns_not_found_for_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Get(-1000);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void Retrieves_tour_ratings_by_tourist()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTourist(-21).Result)?.Value as List<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result.All(tr => tr.IdTourist == -21).ShouldBeTrue();
        }

        [Fact]
        public void Returns_empty_list_for_tourist_without_ratings()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTourist(-100).Result)?.Value as List<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Retrieves_tour_ratings_by_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTour(-1).Result)?.Value as List<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.All(tr => tr.IdTour == -1).ShouldBeTrue();
        }

        [Fact]
        public void Returns_empty_list_for_tour_without_ratings()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTour(-100).Result)?.Value as List<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Retrieves_tour_rating_by_tourist_and_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTouristAndTour(-21, -1).Result)?.Value as TourRatingDto;

            // Assert
            result.ShouldNotBeNull();
            result.IdTourist.ShouldBe(-21);
            result.IdTour.ShouldBe(-1);
            result.Rating.ShouldBe(5);
        }

        [Fact]
        public void Returns_not_found_for_invalid_tourist_and_tour_combination()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.GetByTouristAndTour(-100, -100);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void Retrieves_ratings_with_different_completion_percentages()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            var fullCompletion = result.Results.Where(r => r.TourCompletionPercentage == 100.0).ToList();
            var partialCompletion = result.Results.Where(r => r.TourCompletionPercentage < 100.0).ToList();
            
            fullCompletion.Count.ShouldBeGreaterThan(0);
            partialCompletion.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Retrieves_ratings_with_and_without_comments()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            var withComments = result.Results.Where(r => !string.IsNullOrEmpty(r.Comment)).ToList();
            var withoutComments = result.Results.Where(r => string.IsNullOrEmpty(r.Comment)).ToList();
            
            withComments.Count.ShouldBeGreaterThan(0);
            withoutComments.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Retrieves_ratings_across_rating_range()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TourRatingDto>;

            // Assert
            result.ShouldNotBeNull();
            var allRatingsValid = result.Results.All(r => r.Rating >= 1 && r.Rating <= 5);
            allRatingsValid.ShouldBeTrue();
            
            // Proveri da postoje razli?ite ocene
            var distinctRatings = result.Results.Select(r => r.Rating).Distinct().ToList();
            distinctRatings.Count.ShouldBeGreaterThan(1);
        }

        private static TourRatingController CreateController(IServiceScope scope)
        {
            return new TourRatingController(scope.ServiceProvider.GetRequiredService<ITourRatingService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
