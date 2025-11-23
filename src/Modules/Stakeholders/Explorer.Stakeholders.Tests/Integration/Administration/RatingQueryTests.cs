using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Administrator;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Tests.Rating;

[Collection("Sequential")]
public class RatingQueryTests : BaseStakeholdersIntegrationTest
{
    public RatingQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    // Ključni ID-jevi iz SQL skripte:
    private const long USER_WITH_RATING_ID = -22;       // Ima ocenu -4 (Grade 1)
    private const long USER_WITHOUT_RATING_ID = -100;  // Admin bez ocene
    private const int TOTAL_RATING_COUNT = 4;

    // --- TESTOVI ZA ADMIN QUERY METODE ---

    // GET ALL TEST: Dohvatanje svih ocena (Admin)
    [Fact]
    public void Retrieves_all_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdministratorController(scope);

        // Act
        var actionResult = controller.GetAll(0, 0);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<RatingDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(TOTAL_RATING_COUNT);
        result.TotalCount.ShouldBe(TOTAL_RATING_COUNT);
    }


    // --- TESTOVI ZA TURIST QUERY METODE ---

    // GET BY CURRENT USER TEST (Uspeh: Korisnik ima ocenu)
    [Fact]
    public void Retrieves_rating_for_current_user_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao korisnik -22 (koji ima ocenu -4, Grade 1)
        var controller = CreateTouristController(scope, USER_WITH_RATING_ID);

        // Act
        var actionResult = controller.GetByCurrentUser();
        var result = ((ObjectResult)actionResult.Result)?.Value as RatingDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-4);
        // ISPRAVLJENO PREMA SQL: Očekujemo ocenu 1
        result.Grade.ShouldBe(1);
    }

    // GET BY CURRENT USER TEST (Neuspeh: Korisnik nema ocenu)
    [Fact]
    public void Retrieves_rating_for_current_user_fails_not_found()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao korisnik -100 (Admin) koji NEMA ocenu
        var controller = CreateTouristController(scope, USER_WITHOUT_RATING_ID);

        // Act
        var actionResult = controller.GetByCurrentUser();

        // Assert
        // Očekujemo 404 Not Found iz kontrolera
        ((StatusCodeResult)actionResult.Result).StatusCode.ShouldBe(404);
    }

    private static AdministratorRatingController CreateAdministratorController(IServiceScope scope)
    {
        return new AdministratorRatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            // Simulira Admina (UserID -100)
            ControllerContext = BuildContext("-100")
        };
    }

    private static RatingController CreateTouristController(IServiceScope scope, long userId)
    {
        return new RatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            // Simulira korisnika sa prosleđenim ID-jem
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}