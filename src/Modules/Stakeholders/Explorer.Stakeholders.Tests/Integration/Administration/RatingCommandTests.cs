using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Stakeholders.Tests.Integration;

[Collection("Sequential")]
public class RatingCommandTests : BaseStakeholdersIntegrationTest
{
    public RatingCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    private const long AUTHOR_USER_ID = -13;
    private const long AUTHOR_USER_ID_2 = -22;

    //CREATE TEST
    [Fact]
    public void Creates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_USER_ID);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new RatingDto
        {
            Grade = 5,
            Comment = "Kreirano u testu."
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as RatingDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Grade.ShouldBe(newEntity.Grade);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.UserId.ShouldBe(AUTHOR_USER_ID);
    }

    //UPDATE TEST
    [Fact]
    public void Updates_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_USER_ID_2);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var updatedEntity = new RatingDto
        {
            Id = -4, // ID ocene koju menjamo (pripada UserID -22)
            Grade = 5,
            Comment = "Izmenjeno u testu."
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as RatingDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-4);
        result.Comment.ShouldBe(updatedEntity.Comment);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == -4);
        storedEntity.ShouldNotBeNull();
        storedEntity.Comment.ShouldBe("Izmenjeno u testu.");
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, AUTHOR_USER_ID);
        var updatedEntity = new RatingDto
        {
            Id = -1000, // Nepostojeći ID
            Grade = 5
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Update_fails_not_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao UserID = -1 (Admin), a želimo da menjamo ocenu -4 koja pripada UserID = -22 (Autor)
        var controller = CreateController(scope, -1);
        var updatedEntity = new RatingDto
        {
            Id = -4, // Vlasnik je ID -22
            Grade = 5
        };

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Update(updatedEntity));
    }

    //DELETE TEST
    [Fact]
    public void Deletes_rating_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao UserID = -21
        var controller = CreateController(scope, -21);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = (OkResult)controller.Delete(-3); // ID ocene koju brišemo

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Ratings.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_not_owner()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        // Logujemo se kao UserID = -21 (Admin)
        var controller = CreateController(scope, -21);

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.Delete(-2));
    }

    private static Explorer.API.Controllers.Tourist.RatingController CreateController(IServiceScope scope, long userId)
    {
        return new Explorer.API.Controllers.Tourist.RatingController(
            scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            ControllerContext = BuildContext(userId.ToString())
        };
    }
}