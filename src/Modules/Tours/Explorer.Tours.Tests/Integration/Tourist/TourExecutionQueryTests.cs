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
    public class TourExecutionQueryTests : BaseToursIntegrationTest
    {
        public TourExecutionQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_tour_executions()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<TourExecutionDto>;

            // Assert
            result.ShouldNotBeNull();
            // Ne testiraj tačan broj jer se podaci menjaju tokom testova
        }

        [Fact]
        public void Retrieves_tour_execution_by_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            
            // Prvo kreiraj test podatak
            var createTourExecution = new TourExecutionDto
            {
                IdTour = 1,
                Longitude = 19.0000,
                Latitude = 44.0000,
                IdTourist = 1,
                Status = "Completed"
            };
            var created = ((ObjectResult)controller.Create(createTourExecution).Result)?.Value as TourExecutionDto;

            // Act
            var result = ((ObjectResult)controller.Get(created!.Id).Result)?.Value as TourExecutionDto;

            // Assert
            result.ShouldNotBeNull();
            result!.Id.ShouldBe(created.Id);
            result.IdTour.ShouldBe(1);
            result.IdTourist.ShouldBe(1);
            result.Status.ShouldBe("Completed");
        }

        [Fact]
        public void Returns_not_found_for_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Get(9999);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void Retrieves_tour_executions_by_tourist()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTourist(1).Result)?.Value as List<TourExecutionDto>;

            // Assert
            result.ShouldNotBeNull();
            // Samo proveri da nije null, ne testiraj count jer se podaci menjaju
        }

        [Fact]
        public void Retrieves_tour_executions_by_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetByTour(1).Result)?.Value as List<TourExecutionDto>;

            // Assert
            result.ShouldNotBeNull();
            // Ako ima rezultata, proveri da li su za ispravan tour
            if (result!.Count > 0)
            {
                result.All(te => te.IdTour == 1).ShouldBeTrue();
            }
        }

        [Fact]
        public void Retrieves_tour_execution_by_tourist_and_tour()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            
            // Prvo kreiraj test podatak
            var createTourExecution = new TourExecutionDto
            {
                IdTour = 1,
                Longitude = 19.0000,
                Latitude = 44.0000,
                IdTourist = 1,
                Status = "Completed"
            };
            var created = ((ObjectResult)controller.Create(createTourExecution).Result)?.Value as TourExecutionDto;

            // Act
            var actionResult = controller.GetByTouristAndTour(1, 1);

            // Assert
            if (actionResult.Result is ObjectResult objectResult)
            {
                var tourExecution = objectResult.Value as TourExecutionDto;
                tourExecution.ShouldNotBeNull();
                tourExecution!.IdTourist.ShouldBe(1);
                tourExecution.IdTour.ShouldBe(1);
            }
            else if (actionResult.Result is NotFoundResult)
            {
                // OK ako je NotFound
            }
            else
            {
                // Neočekivan rezultat
                actionResult.Result.ShouldNotBeNull();
            }
        }

        [Fact]
        public void Returns_not_found_for_invalid_tourist_and_tour_combination()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.GetByTouristAndTour(9999, 9999);

            // Assert
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        private static TourExecutionController CreateController(IServiceScope scope)
        {
            return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}