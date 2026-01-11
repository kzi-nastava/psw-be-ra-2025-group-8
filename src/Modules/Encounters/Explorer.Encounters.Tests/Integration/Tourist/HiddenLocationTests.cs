using Explorer.API.Controllers.Tourist;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Encounters.Tests.Integration.Tourist;

[Collection("Sequential")]
public class HiddenLocationTests : BaseEncountersIntegrationTest
{
    public HiddenLocationTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_timer_when_tourist_is_in_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "1"); // PersonId = 1

        // Postavljamo zahtev: Korisnik je na lokaciji (-3 izazov iz tvog SQL-a je u Nisu: 43.3209, 21.8958)
        var request = new CheckEncounterRequestDto
        {
            PersonId = 1,
            EncounterId = -3,
            Latitude = 43.3209,
            Longitude = 21.8958
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert response
        result.ShouldNotBeNull();
        result.StartTimeInRange.ShouldNotBeNull();

        // Assert database
        db.ChangeTracker.Clear();
        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        participation.ShouldNotBeNull();
        participation.StartTimeInRange.ShouldNotBeNull();
    }

    [Fact]
    public void Completes_encounter_after_30_seconds_in_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "2"); // PersonId = 2

        // Manuelno "varamo" bazu: postavljamo da je korisnik vec u opsegu 31 sekundu
        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 2);
        if (participation != null)
        {
            participation.UpdateStartTimeInRange(DateTime.UtcNow.AddSeconds(-31));
            db.EncounterParticipations.Update(participation);
            db.SaveChanges();
        }

        var request = new CheckEncounterRequestDto
        {
            PersonId = 2,
            EncounterId = -3,
            Latitude = 43.3209,
            Longitude = 21.8958
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Completed");
        result.XPAwarded.ShouldNotBeNull();

        // Db Assert
        db.ChangeTracker.Clear();
        var storedParticipation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 2);
        storedParticipation.Status.ToString().ShouldBe("Completed");
    }

    [Fact]
    public void Resets_timer_if_tourist_leaves_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "1");

        // Prvo postavimo da tajmer vec radi
        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        participation.UpdateStartTimeInRange(DateTime.UtcNow);
        db.SaveChanges();

        // Korisnik salje koordinate koje su daleko (npr. Beograd umesto Nisa)
        var request = new CheckEncounterRequestDto
        {
            PersonId = 1,
            EncounterId = -3,
            Latitude = 44.7866,
            Longitude = 20.4489
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert
        result.StartTimeInRange.ShouldBeNull();

        // Db Assert
        db.ChangeTracker.Clear();
        var storedParticipation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        storedParticipation.StartTimeInRange.ShouldBeNull();
    }

    [Fact]
    public void Does_not_start_timer_if_out_of_range()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "1");

        // Korisnik je blizu (npr. na oko 10-15 metara), ali ne unutar granice od 5m
        // Niš koordinate su 43.3209, 21.8958. Ovo je malo pomereno:
        var request = new CheckEncounterRequestDto
        {
            PersonId = 1,
            EncounterId = -3,
            Latitude = 43.3212,
            Longitude = 21.8961
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert
        result.ShouldNotBeNull();
        result.StartTimeInRange.ShouldBeNull();

        // Db Assert
        db.ChangeTracker.Clear();
        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        participation.StartTimeInRange.ShouldBeNull();
    }

    [Fact]
    public void Does_not_complete_if_range_time_is_less_than_30_seconds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "1");

        // Manuelno postavljamo da je korisnik ušao u zonu pre samo 10 sekundi
        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        participation.UpdateStartTimeInRange(DateTime.UtcNow.AddSeconds(-10));
        db.EncounterParticipations.Update(participation);
        db.SaveChanges();

        var request = new CheckEncounterRequestDto
        {
            PersonId = 1,
            EncounterId = -3,
            Latitude = 43.3209,
            Longitude = 21.8958
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Active"); // Još uvek treba da bude aktivan
        result.CompletedAt.ShouldBeNull();

        // Db Assert
        db.ChangeTracker.Clear();
        var storedParticipation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 1);
        storedParticipation.Status.ToString().ShouldBe("Active");
    }

    [Fact]
    public void Returns_already_completed_status_if_called_again()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        var controller = CreateController(scope, "2");

        var participation = db.EncounterParticipations.FirstOrDefault(p => p.EncounterId == -3 && p.PersonId == 2);
        participation.ShouldNotBeNull();

        if (participation.Status.ToString() != "Completed")
        {
            participation.Complete(120);
            db.EncounterParticipations.Update(participation);
            db.SaveChanges();
        }

        var request = new CheckEncounterRequestDto
        {
            PersonId = 2,
            EncounterId = -3,
            Latitude = 43.3209,
            Longitude = 21.8958
        };

        // Act
        var result = ((ObjectResult)controller.CheckLocation(-3, request).Result).Value as EncounterParticipationDto;

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Completed");
    }

    private static EncounterParticipationController CreateController(IServiceScope scope, string personId)
    {
        var controller = new EncounterParticipationController(
            scope.ServiceProvider.GetRequiredService<IEncounterService>(),
            scope.ServiceProvider.GetRequiredService<IEncounterParticipationService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("id", personId),
                    new Claim(ClaimTypes.Role, "tourist")
                }))
            }
        };

        return controller;
    }
}