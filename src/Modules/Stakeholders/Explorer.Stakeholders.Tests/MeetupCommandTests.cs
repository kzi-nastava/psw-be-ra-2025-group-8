using System;
using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests;

[Collection("Sequential")]
public class MeetupCommandTests : BaseStakeholdersIntegrationTest
{
    public MeetupCommandTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Create_meetup_persists_to_database()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        var controller = CreateController(scope, personId: "-21");   // turist1
        var context = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var dto = new MeetupDto
        {
            // Id se ignoriše kod kreiranja
            Name = "Test meetup",
            Description = "Test description",
            ScheduledAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Latitude = 45.25,
            Longitude = 19.83,
            CreatorId = -21     // ovo je UserId turiste iz test podataka
        };

        // Act
        var result = controller.Create(dto).Result as ObjectResult;

        // Assert - HTTP odgovor
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var created = result.Value as MeetupDto;
        created.ShouldNotBeNull();
        created.Id.ShouldBeGreaterThan(0);
        created.Name.ShouldBe(dto.Name);

        // Assert - upisan u bazu
        var fromDb = context.Meetups
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == created.Id);

        fromDb.ShouldNotBeNull();
        fromDb!.Name.ShouldBe(dto.Name);
        fromDb.CreatorId.ShouldBe(dto.CreatorId);
    }

    private static MeetupController CreateController(IServiceScope scope, string personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IMeetupService>();
        var controller = new MeetupController(service);

        // ovo dolazi iz BaseWebIntegrationTest
        controller.ControllerContext = BuildContext(personId);

        return controller;
    }
}
