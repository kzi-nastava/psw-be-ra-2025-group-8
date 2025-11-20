using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Controllers.Administrator;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class RatingQueryTests : BaseStakeholdersIntegrationTest
{
    public RatingQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    //GET ALL TEST
    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAdministratorController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<RatingDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(4);
        result.TotalCount.ShouldBe(4);
    }

    //GET TEST
    [Fact]
    public void Retrieves_one()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as RatingDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Grade.ShouldBe(5);
        result.Comment.ShouldBe("Odlična ocena za platformu!");
    }

    private static Explorer.API.Controllers.Administrator.AdministratorRatingController CreateAdministratorController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Administrator.AdministratorRatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            // Simulira Admina (UserID 1)
            ControllerContext = BuildContext("1")
        };
    }

    private static Explorer.API.Controllers.Tourist.RatingController CreateTouristController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Tourist.RatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            // Simulira Autora (UserID 2)
            ControllerContext = BuildContext("2")
        };
    }
}