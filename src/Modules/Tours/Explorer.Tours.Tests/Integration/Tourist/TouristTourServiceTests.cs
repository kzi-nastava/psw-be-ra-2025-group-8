using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristTourControllerTests : BaseToursIntegrationTest
    {
        public TouristTourControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetPublishedTours_ReturnsOnlyPublishedTours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish tour -10 (Test Tour1)
            var tour = db.Tours.Find(-10L)!;
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTours(); // ActionResult<List<TouristTourPreviewDto>>
            var okResult = actionResult as OkObjectResult;
            okResult.ShouldNotBeNull();

            var result = okResult.Value as List<TouristTourPreviewDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Id.ShouldBe(-10);
        }

        [Fact]
        public void GetPublishedTour_ReturnsDetailsWithFirstKeyPoint()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish Test Tour1 (-10)
            var tour = db.Tours.Find(-10L)!;
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10); // ActionResult<TouristTourDetailsDto>
            var okResult = actionResult as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as TouristTourDetailsDto;

            // Assert
            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(-10);
            dto.FirstKeyPoint.ShouldNotBeNull();
            dto.FirstKeyPoint.Name.ShouldBe("Trg Slobode");
            dto.FirstKeyPoint.Latitude.ShouldBe(45.2671);
            dto.FirstKeyPoint.Longitude.ShouldBe(19.8335);
        }

        [Fact]
        public void GetPublishedTour_ReturnsDetailsWithTagsAndEquipment()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish Test Tour1 (-10)
            var tour = db.Tours.Find(-10L)!;
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var okResult = actionResult as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as TouristTourDetailsDto;

            // Assert
            dto.ShouldNotBeNull();
            dto.Tags.ShouldContain("mountain");
            dto.Tags.ShouldContain("food");
        }

        [Fact]
        public void GetPublishedTour_ReturnsAverageRating()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tour = db.Tours.Find(-10L)!;
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var okResult = actionResult as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as TouristTourDetailsDto;

            // Assert
            dto.ShouldNotBeNull();
            dto.AverageRating.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void GetPublishedTour_ReturnsReviewsWithAuthorName()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tour = db.Tours.Find(-10L)!;
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var okResult = actionResult as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as TouristTourDetailsDto;

            // Assert
            dto.ShouldNotBeNull();
            dto.Reviews.Count.ShouldBeGreaterThan(0);
            dto.Reviews[0].AuthorName.ShouldNotBeNull();
        }

        [Fact]
        public void GetPublishedTour_NotPublished_ReturnsNotFound()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act – tour -10 je DRAFT po seed-u
            var actionResult = controller.GetPublishedTour(-10);
            var notFound = actionResult as NotFoundObjectResult;

            // Assert
            notFound.ShouldNotBeNull();
            notFound.StatusCode.ShouldBe(404);
        }

        private static TouristTourController CreateController(IServiceScope scope)
        {
            return new TouristTourController(
                scope.ServiceProvider.GetRequiredService<ITouristTourService>())
            {
                ControllerContext = BuildContext("-21")  // Tourist ID = -21 (seed user)
            };
        }
    }
}
