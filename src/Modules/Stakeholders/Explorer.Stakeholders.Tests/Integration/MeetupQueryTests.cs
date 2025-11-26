using System;
using System.Collections.Generic;
using System.Linq;               // <- zbog ToList i All
using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.BuildingBlocks.Tests;

namespace Explorer.Stakeholders.Tests;

[Collection("Sequential")]
public class MeetupQueryTests : BaseStakeholdersIntegrationTest
{
    public MeetupQueryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_meetup_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, personId: "-21");

        const int seedId = -301;
        const string seedName = "Seed meetup 1";

        // Act
        var actionResult = controller.Get(seedId);              
        var objResult = actionResult.Result as ObjectResult;     

        // Assert
        objResult.ShouldNotBeNull();
        objResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var meetup = objResult.Value as MeetupDto;
        meetup.ShouldNotBeNull();
        meetup!.Id.ShouldBe(seedId);
        meetup.Name.ShouldBe(seedName);
    }

    [Fact]
    public void Retrieves_meetups_by_creator()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, personId: "-21");

        const long creatorId = -21;

        // Act
        var actionResult = controller.GetByCreator(creatorId);     
        var objResult = actionResult.Result as ObjectResult;      

        // Assert
        objResult.ShouldNotBeNull();
        objResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var meetups = objResult.Value as IEnumerable<MeetupDto>;
        meetups.ShouldNotBeNull();

        var list = meetups!.ToList();
        list.Count.ShouldBeGreaterThan(0);
        list.All(m => m.CreatorId == creatorId).ShouldBeTrue();
    }

    private static MeetupController CreateController(IServiceScope scope, string personId)
    {
        var service = scope.ServiceProvider.GetRequiredService<IMeetupService>();
        var controller = new MeetupController(service);
        controller.ControllerContext = BuildContext(personId);
        return controller;
    }
}
