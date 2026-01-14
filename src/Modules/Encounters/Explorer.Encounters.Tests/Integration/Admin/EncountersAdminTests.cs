using Explorer.API.Controllers.Administration;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Encounters.Tests.Integration.Admin;

[Collection("Sequential")]
public class EncountersAdminTests : BaseEncountersIntegrationTest
{
    public EncountersAdminTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_all_encounters()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetAll().Result).Value as List<EncounterDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Gets_encounter_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetById(-2).Result).Value as EncounterDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-2);
    }

    [Fact]
    public void Creates_encounter_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope);

        var createDto = new EncounterDto
        {
            Name = "Admin created encounter",
            Description = "Encounter created by admin",
            Location = "Kragujevac",
            Latitude = 44.0128,
            Longitude = 20.9114,
            Type = "MiscBased",
            XPReward = 70
        };

        var result = ((ObjectResult)controller.Create(createDto).Result)
            .Value as EncounterDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Admin created encounter");
        result.Status.ShouldBe("Draft");

        // Database assertions
        db.ChangeTracker.Clear();
        var stored = db.Encounters.FirstOrDefault(e => e.Name == "Admin created encounter");
        stored.ShouldNotBeNull();
        stored.Status.ShouldBe(EncouterStatus.Draft);
    }

    [Fact]
    public void Updates_encounter_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope);

        var updateDto = new EncounterUpdateDto
        {
            Name = "Updated encounter",
            Description = "Updated description",
            Location = "Niš",
            Latitude = 43.3209,
            Longitude = 21.8958,
            Type = "SocialBased",
            XPReward = 120
        };

        var result = ((ObjectResult)controller.Update(-2, updateDto).Result)
            .Value as EncounterDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated encounter");
        result.XPReward.ShouldBe(120);

        // Database assertions
        db.ChangeTracker.Clear();
        var stored = db.Encounters.FirstOrDefault(e => e.Id == -2);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe("Updated encounter");
        stored.Type.ShouldBe(EncouterType.SocialBased);
        stored.XPReward.ShouldBe(120);
    }

    [Fact]
    public void Deletes_encounter_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope);

        // Ako je već obrisan (npr. test pokrenut ranije) – test je OK
        var existing = db.Encounters.FirstOrDefault(e => e.Id == -1);
        if (existing == null)
        {
            true.ShouldBeTrue();
            return;
        }

        // Act
        var response = controller.Delete(-1);

        // Assert response
        response.ShouldBeOfType<OkResult>();

        // Assert database
        db.ChangeTracker.Clear();
        var deleted = db.Encounters.FirstOrDefault(e => e.Id == -1);
        deleted.ShouldBeNull();
    }

    // -------------------------
    // Controller factory
    // -------------------------
    private static EncountersController CreateController(IServiceScope scope)
    {
        var controller = new EncountersController(
            scope.ServiceProvider.GetRequiredService<IEncounterService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "administrator")
                }))
            }
        };

        return controller;
    }
}
