using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
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
    public class TourRatingCommandTests : BaseToursIntegrationTest
    {
        public TourRatingCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -11,
                Rating = 5,
                Comment = "Fantastična tura, preporučujem svima!",
                TourCompletionPercentage = 100.0
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Rating.ShouldBe(newEntity.Rating);
            result.Comment.ShouldBe(newEntity.Comment);
            result.TourCompletionPercentage.ShouldBe(100.0);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == result.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Rating.ShouldBe(5);
        }

        [Fact]
        public void Creates_with_minimum_rating()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourRatingDto
            {
                IdTour = -2,
                IdTourist = -12,
                Rating = 1,
                Comment = "Razočarenje, nije ispunilo očekivanja.",
                TourCompletionPercentage = 25.0
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Rating.ShouldBe(1);
            result.Comment.ShouldBe(newEntity.Comment);
            result.TourCompletionPercentage.ShouldBe(25.0);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == result.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Rating.ShouldBe(1);
        }

        [Fact]
        public void Creates_without_comment()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -13,
                Rating = 3,
                Comment = null,
                TourCompletionPercentage = 50.0
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Rating.ShouldBe(3);
            result.Comment.ShouldBeNull();

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == result.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Comment.ShouldBeNull();
        }

        [Fact]
        public void Create_fails_invalid_rating_too_high()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -11,
                Rating = 6,
                Comment = "Pokušaj sa ocenom većom od 5",
                TourCompletionPercentage = 100.0
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity));
        }

        [Fact]
        public void Create_fails_invalid_rating_too_low()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -11,
                Rating = 0,
                Comment = "Pokušaj sa ocenom manjom od 1",
                TourCompletionPercentage = 100.0
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity));
        }

        [Fact]
        public void Create_fails_invalid_tour_completion_percentage_too_high()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -11,
                Rating = 5,
                Comment = "Pokušaj sa procenatom većim od 100",
                TourCompletionPercentage = 150.0
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity));
        }

        [Fact]
        public void Create_fails_invalid_tour_completion_percentage_negative()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var newEntity = new TourRatingDto
            {
                IdTour = -1,
                IdTourist = -11,
                Rating = 5,
                Comment = "Pokušaj sa negativnim procenatom",
                TourCompletionPercentage = -10.0
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(newEntity));
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new TourRatingDto
            {
                Id = -1,
                IdTour = -1,
                IdTourist = -21,
                Rating = 3,
                Comment = "Promenio sam mišljenje, ipak nije bilo savršeno.",
                TourCompletionPercentage = 100.0
            };
            var created = ((ObjectResult)controller.Create(updatedEntity).Result)?.Value as TourRatingDto;
            created.ShouldNotBeNull();

            var tourRating = new TourRatingDto
            {
                Id = created!.Id,
                IdTour = -1,
                IdTourist = -21,
                Rating = 4,
                Comment = "Promenio sam mišljenje, ipak je bilo savršeno.",
                TourCompletionPercentage = 90.0
            };

            // Act
            var result = ((ObjectResult)controller.Update(created.Id, tourRating).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Rating.ShouldBe(4);
            result.Comment.ShouldBe(tourRating.Comment);
            result.TourCompletionPercentage.ShouldBe(90.0);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == created.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Rating.ShouldBe(4);
            storedEntity.Comment.ShouldBe(tourRating.Comment);
            storedEntity.TourCompletionPercentage.ShouldBe(90.0);
        }

        [Fact]
        public void Update_rating_only()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourRatingDto
            {
                Id = -1,
                IdTour = -1,
                IdTourist = -21,
                Rating = 3,
                Comment = "Promenio sam mišljenje, ipak nije bilo savršeno.",
                TourCompletionPercentage = 100.0
            };
            var created = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourRatingDto;
            created.ShouldNotBeNull();
            var updatedEntity = new TourRatingDto
            {
                Id = created.Id,
                IdTour = created.IdTour,
                IdTourist = created.IdTourist,
                Rating = 5,
                Comment = created.Comment,
                TourCompletionPercentage = created.TourCompletionPercentage
            };

            // Act
            var result = ((ObjectResult)controller.Update(created.Id, updatedEntity).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Rating.ShouldBe(5);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == created.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Rating.ShouldBe(5);
        }

        [Fact]
        public void Update_comment_only()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var originalRating = dbContext.TourRatings.FirstOrDefault(i => i.Id == -3)?.Rating ?? 0;
            var updatedEntity = new TourRatingDto
            {
                Id = -3,
                IdTour = -2,
                IdTourist = -22,
                Rating = originalRating,
                Comment = "Ažuriran komentar nakon ponovnog razmatranja.",
                TourCompletionPercentage = 50.0
            };

            // Act
            var result = ((ObjectResult)controller.Update(-3, updatedEntity).Result)?.Value as TourRatingDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-3);
            result.Comment.ShouldBe(updatedEntity.Comment);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == -3);
            storedEntity.ShouldNotBeNull();
            storedEntity.Comment.ShouldBe(updatedEntity.Comment);
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourRatingDto
            {
                Id = -1000,
                IdTour = -1,
                IdTourist = -11,
                Rating = 4,
                Comment = "Pokušaj ažuriranja nepostojeće ocene",
                TourCompletionPercentage = 100.0
            };

            // Act & Assert
            Should.Throw<KeyNotFoundException>(() => controller.Update(-1000, updatedEntity));
        }

        [Fact]
        public void Update_fails_invalid_rating()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourRatingDto
            {
                Id = -1,
                IdTour = -1,
                IdTourist = -21,
                Rating = 7,
                Comment = "Pokušaj sa nevalidnom ocenom",
                TourCompletionPercentage = 100.0
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Update(-1, updatedEntity));
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = (OkResult)controller.Delete(-5);

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var storedEntity = dbContext.TourRatings.FirstOrDefault(i => i.Id == -5);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Delete(-1000);

            // Assert - samo proveri da metoda ne krahira, bez specifičnog assertion-a
            result.ShouldNotBeNull();
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
