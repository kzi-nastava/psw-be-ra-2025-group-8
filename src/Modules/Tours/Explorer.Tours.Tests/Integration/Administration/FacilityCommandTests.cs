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
public class FacilityCommandTests : BaseToursIntegrationTest
{
    public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
     using var scope = Factory.Services.CreateScope();
     var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new FacilityDto
     {
     Name = "Viewpoint Tower",
  Latitude = 45.2550,
      Longitude = 19.8450,
    Category = "Other"
        };

        // Act
 var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as FacilityDto;

     // Assert - Response
   result.ShouldNotBeNull();
 result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Latitude.ShouldBe(newEntity.Latitude);
    result.Longitude.ShouldBe(newEntity.Longitude);
        
    // Assert - Database
var storedEntity = dbContext.Facilities.FirstOrDefault(i => i.Id == result.Id);
        storedEntity.ShouldNotBeNull();
  storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.Latitude.ShouldBe(newEntity.Latitude);
   storedEntity.Longitude.ShouldBe(newEntity.Longitude);
    }

    [Fact]
    public void Create_fails_invalid_name()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
      var controller = CreateController(scope);
     var updatedEntity = new FacilityDto
        {
     Name = "",
            Latitude = 45.0,
  Longitude = 19.0,
  Category = "WC"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Create_fails_invalid_latitude()
    {
        // Arrange
  using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
        Name = "Test Facility",
     Latitude = 95.0, // Invalid latitude
   Longitude = 19.0,
            Category = "WC"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
public void Create_fails_invalid_longitude()
    {
 // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new FacilityDto
        {
            Name = "Test Facility",
  Latitude = 45.0,
            Longitude = 190.0, // Invalid longitude
            Category = "WC"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
  using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new FacilityDto
{
    Id = -1,
         Name = "Updated WC Central Park",
         Latitude = 45.2680,
     Longitude = 19.8340,
     Category = "WC"
        };

        // Act
      var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as FacilityDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedEntity.Name);
     result.Latitude.ShouldBe(updatedEntity.Latitude);
        result.Longitude.ShouldBe(updatedEntity.Longitude);

        // Assert - Database
        var storedEntity = dbContext.Facilities.FirstOrDefault(i => i.Name == "Updated WC Central Park");
        storedEntity.ShouldNotBeNull();
    storedEntity.Latitude.ShouldBe(updatedEntity.Latitude);
        var oldEntity = dbContext.Facilities.FirstOrDefault(i => i.Name == "Public WC Central Park");
        oldEntity.ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
      using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
    var updatedEntity = new FacilityDto
  {
            Id = -1000,
   Name = "Test",
            Latitude = 45.0,
      Longitude = 19.0,
            Category = "WC"
 };

// Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
  var result = (OkResult)controller.Delete(-4);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedEntity = dbContext.Facilities.FirstOrDefault(i => i.Id == -4);
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
    
    private static FacilityController CreateController(IServiceScope scope)
    {
        return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
        {
       ControllerContext = BuildContext("-1")
        };
    }
}
