using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Tourist;
using System.Collections.Generic; // Za KeyNotFoundException

namespace Explorer.Stakeholders.Tests.Rating;

[Collection("Sequential")]
public class RatingCommandTests : BaseStakeholdersIntegrationTest
{
    public RatingCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    // Ključni ID-jevi iz SQL skripte:
    private const long AUTHOR_USER_ID_CREATE = -23;         // Turista 3: Nema ocenu, koristi se za CREATE
    private const long AUTHOR_USER_ID_UPDATE = -22;         // Turista 2: Ima ocenu -4 (Grade 1), koristi se za UPDATE
    private const long AUTHOR_USER_ID_DELETE = -21;         // Turista 1: Ima ocenu -3 (Grade 3), koristi se za DELETE
    private const long USER_WITHOUT_RATING_ID = -100;       // Admin: Nema ocenu

    // ID ocene koja se koristi za brisanje
    private const int RATING_ID_TO_DELETE = -3;


    // --- CREATE TEST ---
    [Fact]
    public void Creates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_USER_ID_CREATE); // Logujemo se kao -23
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new RatingNoIdDto
        {
            Grade = 4,
            Comment = "Kreirano u testu uspešno."
        };

        // Act
        var actionResult = controller.Create(newEntity);
        var result = ((ObjectResult)actionResult.Result)?.Value as RatingDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Grade.ShouldBe(newEntity.Grade);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.CreationDate.ShouldBeGreaterThan(DateTime.MinValue);
    }


    // --- UPDATE TEST ---
    [Fact]
    public void Updates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao AUTHOR_USER_ID_UPDATE (-22), koji ima rating ID -4
        var controller = CreateController(scope, AUTHOR_USER_ID_UPDATE);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var updatedEntity = new RatingNoIdDto
        {
            Grade = 3,
            Comment = "Izmenjeno u testu."
        };

        var oldRating = dbContext.Ratings.FirstOrDefault(i => i.Id == -4);
        oldRating.ShouldNotBeNull();
        var oldCreationDate = oldRating.CreationDate;

        // Act
        var actionResult = controller.UpdateByCurrentUser(updatedEntity);
        var result = ((ObjectResult)actionResult.Result)?.Value as RatingDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-4);
        result.Grade.ShouldBe(updatedEntity.Grade);
        result.Comment.ShouldBe(updatedEntity.Comment);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == -4);
        storedEntity.ShouldNotBeNull();
        storedEntity.Comment.ShouldBe(updatedEntity.Comment);
        storedEntity.CreationDate.ShouldBeGreaterThan(oldCreationDate);
    }

    // UPDATE TEST (Neuspeh: Korisnik koji pokušava da ažurira nema kreiranu ocenu)
    [Fact]
    public void Update_fails_no_existing_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao -100 (korisnik bez ocene)
        var controller = CreateController(scope, USER_WITHOUT_RATING_ID);

        var updatedEntity = new RatingNoIdDto { Grade = 5, Comment = "..." };

        // Act & Assert
        // Servis baca KeyNotFoundException ako ocena ne postoji za datog korisnika
        Should.Throw<KeyNotFoundException>(() => controller.UpdateByCurrentUser(updatedEntity));
    }


    // --- DELETE TEST ---
    [Fact]
    public void Deletes_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao UserID = -21 (koji ima ocenu ID -3)
        var controller = CreateController(scope, AUTHOR_USER_ID_DELETE);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var actionResult = controller.DeleteByCurrentUser();
        var result = actionResult as OkResult;

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == RATING_ID_TO_DELETE);
        storedEntity.ShouldBeNull();
    }

    // DELETE TEST (Neuspeh: Korisnik koji pokušava da briše nema kreiranu ocenu)
    [Fact]
    public void Delete_fails_no_existing_rating()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao UserID = -100 (Admin) koji NEMA rating
        var controller = CreateController(scope, USER_WITHOUT_RATING_ID);

        // Act & Assert
        // Servis vraća Ok() jer ne baca izuzetak za nepostojeću ocenu
        var result = controller.DeleteByCurrentUser() as OkResult;
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
    }

    private static RatingController CreateController(IServiceScope scope, long userId)
    {
        return new RatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}