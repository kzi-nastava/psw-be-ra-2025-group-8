using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Tests; // za EntityValidationException i NotFoundException

public class TransportTypePreferencesTests : IClassFixture<ToursTestFactory>
{
    private readonly ToursTestFactory _factory;
    public TransportTypePreferencesTests(ToursTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Get_Returns_all_transports_with_default_ratings()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITransportTypePreferencesService>();

        var list = svc.Get(-21).ToList();

        list.Count.ShouldBe(4); // očekujemo sva 4 transporta
        list.All(t => t.Rating >= 0 && t.Rating <= 3).ShouldBeTrue();
        list.Single(t => t.Transport.Equals("Walk", System.StringComparison.OrdinalIgnoreCase)).Rating.ShouldBe(0);
    }

    [Fact]
    public void Update_Valid_changes_are_persisted()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITransportTypePreferencesService>();
        var ctx = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newRatings = new List<TransportTypePreferenceDto>
        {
            new() { Transport = "Walk", Rating = 2 },
            new() { Transport = "Bicycle", Rating = 1 },
            new() { Transport = "Car", Rating = 3 },
            new() { Transport = "Boat", Rating = 0 },
        };

        svc.Update(-21, newRatings);

        var fromDb = ctx.TransportTypePreferences.Where(t => t.Preference.PersonId == -21).ToList();
        fromDb.Count.ShouldBe(4);
        fromDb.Single(t => t.Transport == TransportType.Walk).Rating.ShouldBe(2);
        fromDb.Single(t => t.Transport == TransportType.Car).Rating.ShouldBe(3);
    }

    [Fact]
    public void Update_InvalidTransport_Throws()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITransportTypePreferencesService>();

        var bad = new List<TransportTypePreferenceDto>
        {
            new() { Transport = "Plane", Rating = 1 }
        };

        Should.Throw<EntityValidationException>(() => svc.Update(-21, bad));
    }

    [Fact]
    public void Update_InvalidRating_Throws()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<ITransportTypePreferencesService>();

        var bad = new List<TransportTypePreferenceDto>
        {
            new() { Transport = "Walk", Rating = 999 }
        };

        Should.Throw<EntityValidationException>(() => svc.Update(-21, bad));
    }
}

