using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubsCommandTests : BaseStakeholdersIntegrationTest
{
    public ClubsCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new CreateClubDto
        {
            Name = "Test Klub",
            Description = "Opis test kluba",
            ImageUrls = new List<string> { "slika1.jpg" }
        };

        // Act
        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as ClubDto;

        // Assert – Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(dto.Name);

        // Assert – DB
        var storedEntity = db.Clubs.FirstOrDefault(c => c.Name == "Test Klub").ShouldNotBeNull();
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateClubDto
        {
            Description = "Nema imena"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(dto));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new ClubDto
        {
            Name = "Promenjeni Klub",
            Description = "Novi opis",
            ImageUrls = new List<string> { "x.jpg", "y.jpg" },
            OwnerId = -22
        };

        // Act
        var result = ((ObjectResult)controller.Update(-2, dto).Result)?.Value as ClubDto;

        // Assert – Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-2);
        result.Name.ShouldBe(dto.Name);
        result.Description.ShouldBe(dto.Description);

        // Assert – DB
        var stored = db.Clubs.FirstOrDefault(c => c.Id == -2);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(dto.Name);
    }

    [Fact]
    public void Update_fails_unauthorized()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new ClubDto
        {
            Name = "Nesto",
            Description = "Nesto",
            OwnerId = -33
        };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() =>
            controller.Update(-3, dto));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-13");
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert
        result.StatusCode.ShouldBe(200);

        db.Clubs.FirstOrDefault(c => c.Id == -3).ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_unauthorized()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22");

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Delete(-1));
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