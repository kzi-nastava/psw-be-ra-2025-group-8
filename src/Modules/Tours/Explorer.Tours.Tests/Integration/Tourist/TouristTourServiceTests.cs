using Explorer.API.Controllers.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var tour = db.Tours.Find(-10L);
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTours();
            var objectResult = actionResult as ObjectResult;
            var result = objectResult?.Value as List<object>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }

        [Fact]
        public void GetPublishedTour_ReturnsDetailsWithFirstKeyPoint()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish Test Tour1 (-10)
            var tour = db.Tours.Find(-10L);
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult?.Value;

            // Assert
            result.ShouldNotBeNull();

            dynamic dto = result!;
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
            var tour = db.Tours.Find(-10L);
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult?.Value;

            // Assert
            result.ShouldNotBeNull();
            dynamic dto = result!;
            ((IEnumerable<string>)dto.Tags).ShouldContain("mountain");
            ((IEnumerable<string>)dto.Tags).ShouldContain("food");
        }

        [Fact]
        public void GetPublishedTour_ReturnsAverageRating()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish Test Tour1 (-10)
            var tour = db.Tours.Find(-10L);
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult?.Value;

            // Assert
            result.ShouldNotBeNull();
            dynamic dto = result!;
            dto.AverageRating.ShouldBeGreaterThan(0); // jer postoje rating zapisi u seed-u
        }

        [Fact]
        public void GetPublishedTour_ReturnsReviewsWithAuthorName()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Publish Test Tour1 (-10)
            var tour = db.Tours.Find(-10L);
            tour.Status = Core.Domain.TourStatus.Published;
            db.SaveChanges();

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var objectResult = actionResult as ObjectResult;
            var result = objectResult?.Value;

            // Assert
            result.ShouldNotBeNull();
            dynamic dto = result!;
            dto.Reviews.Count.ShouldBeGreaterThan(0);
            dto.Reviews[0].AuthorName.ShouldNotBeNull(); // jer vučemo Person.Name + Surname
        }

        [Fact]
        public void GetPublishedTour_NotPublished_ReturnsNotFound()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var actionResult = controller.GetPublishedTour(-10);
            var result = actionResult as NotFoundObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
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
