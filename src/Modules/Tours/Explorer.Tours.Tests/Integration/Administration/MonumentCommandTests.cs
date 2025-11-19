using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class MonumentCommandTests : BaseToursIntegrationTest
{
    public MonumentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new MonumentDto
        {
            Name = "Smederevska tvrđava",
            Description = "Najveća srednjovekovna ravničarska tvrđava u Evropi, izgrađena u 15. veku.",
            YearOfConstruction = 1430,
            Status = "Active",
            Latitude = 44.6614,
            Longitude = 20.9301
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);

        // Assert - Database
        var storedEntity = dbContext.Monument.FirstOrDefault(i => i.Name == newEntity.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Name = "",
            Description = "Test",
            YearOfConstruction = 2000,
            Latitude = 44.0,
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_invalid_year()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Name = "Test Monument",
            Description = "Test",
            YearOfConstruction = 3000, // Future year
            Latitude = 44.0,
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Create_fails_invalid_coordinates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new MonumentDto
        {
            Name = "Test Monument",
            Description = "Test",
            YearOfConstruction = 2000,
            Latitude = 200, // Invalid latitude
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new MonumentDto
        {
            Id = -1,
            Name = "Beogradska tvrđava",
            Description = "Ažurirani opis istorijske tvrđave u Beogradu.",
            YearOfConstruction = 1717,
            Status = "Active",
            Latitude = 44.823319,
            Longitude = 20.450739
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as MonumentDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
        result.Description.ShouldBe(updatedEntity.Description);

        // Assert - Database
        var storedEntity = dbContext.Monument.FirstOrDefault(i => i.Name == "Beogradska tvrđava");
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        var oldEntity = dbContext.Monument.FirstOrDefault(i => i.Name == "Kalemegdanska tvrđava");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new MonumentDto
        {
            Id = -1000,
            Name = "Test",
            Description = "Test",
            YearOfConstruction = 2000,
            Latitude = 44.0,
            Longitude = 20.0
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Monument.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static MonumentController CreateController(IServiceScope scope)
    {
        return new MonumentController(scope.ServiceProvider.GetRequiredService<IMonumentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}