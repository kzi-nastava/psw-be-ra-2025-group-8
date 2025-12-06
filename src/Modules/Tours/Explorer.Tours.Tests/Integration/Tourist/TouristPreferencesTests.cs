using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Tests;
using Explorer.Tours.API.Public.Tourist;

public class TouristPreferencesTests : IClassFixture<ToursTestFactory>
{
    private readonly ToursTestFactory _factory;

    public TouristPreferencesTests(ToursTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Get_Returns_own_preferences()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITouristPreferencesService>();

        var dto = svc.Get(-21); // person -21 iz testnog data

        dto.ShouldNotBeNull();
        dto.PersonId.ShouldBe(-21);
        dto.Difficulty.ShouldBe("Beginner");
    }

    [Fact]
    public void Update_Changes_difficulty_and_persists()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITouristPreferencesService>();
        var ctx = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updateDto = new UpdateTouristPreferencesDto { Difficulty = "Professional" };
        var updated = svc.Update(-21, updateDto);

        updated.ShouldNotBeNull();
        updated.Difficulty.ShouldBe("Professional");

        // assert directly against DB
        var fromDb = ctx.TouristPreferences.Single(tp => tp.PersonId == -21);
        fromDb.Difficulty.ShouldBe(DifficultyLevel.Professional);
    }
}
