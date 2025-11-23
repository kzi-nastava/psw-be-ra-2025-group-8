using Explorer.API.Controllers.User;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubsQueryTests : BaseStakeholdersIntegrationTest
{
    public ClubsQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_single()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as ClubDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
    }

    [Fact]
    public void Retrieves_all()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetAll(0, 0).Result)?.Value as PagedResult<ClubDto>;


        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(3);
    }

    //ja sam pravio svoj CreateController, jer u bazi imam specificne userId-jeve (-11, -22, -33) koje koristim u testovima
    //mogu promeniti ako treba
    private static ClubsController CreateController(IServiceScope scope, string userId = "-11")
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}
