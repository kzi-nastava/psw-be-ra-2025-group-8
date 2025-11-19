using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Tourist.Profile;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Explorer.Stakeholders.Tests.Integration.Profile;

[Collection("Sequential")]
public class ProfileTests : BaseStakeholdersIntegrationTest
{
    public ProfileTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_profile_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, userId: -21, personId: -21);

        var result = ((ObjectResult)controller.GetProfile().Result).Value as PersonDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-21);
        result.Name.ShouldNotBeNullOrEmpty();
        result.Surname.ShouldNotBeNullOrEmpty();
        result.Email.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Updates_profile_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, userId: -21, personId: -21);

        var updateDto = new UpdatePersonDto
        {
            Name = "Updated",
            Surname = "Profile",
            Email = "updated@example.com",
            ProfilePicture = "https://example.com/profile.jpg",
            Bio = "This is my updated bio",
            Motto = "Live, laugh, love"
        };

        var result = ((ObjectResult)controller.UpdateProfile(updateDto).Result).Value as PersonDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated");
        result.Surname.ShouldBe("Profile");
        result.Email.ShouldBe("updated@example.com");
        result.ProfilePicture.ShouldBe("https://example.com/profile.jpg");
        result.Bio.ShouldBe("This is my updated bio");
        result.Motto.ShouldBe("Live, laugh, love");

        // Database assertions
        db.ChangeTracker.Clear();
        var storedPerson = db.People.FirstOrDefault(p => p.Id == -21);
        storedPerson.ShouldNotBeNull();
        storedPerson.Name.ShouldBe("Updated");
        storedPerson.Surname.ShouldBe("Profile");
        storedPerson.Email.ShouldBe("updated@example.com");
        storedPerson.ProfilePicture.ShouldBe("https://example.com/profile.jpg");
        storedPerson.Bio.ShouldBe("This is my updated bio");
        storedPerson.Motto.ShouldBe("Live, laugh, love");
    }

    [Fact]
    public void Updates_profile_with_null_optional_fields()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope, userId: -22, personId: -22);

        var updateDto = new UpdatePersonDto
        {
            Name = "Simple",
            Surname = "User",
            Email = "simple@example.com",
            ProfilePicture = null,
            Bio = null,
            Motto = null
        };

        var result = ((ObjectResult)controller.UpdateProfile(updateDto).Result).Value as PersonDto;

        result.ShouldNotBeNull();
        result.ProfilePicture.ShouldBeNull();
        result.Bio.ShouldBeNull();
        result.Motto.ShouldBeNull();

        // Database assertions
        db.ChangeTracker.Clear();
        var storedPerson = db.People.FirstOrDefault(p => p.Id == -22);
        storedPerson.ShouldNotBeNull();
        storedPerson.ProfilePicture.ShouldBeNull();
        storedPerson.Bio.ShouldBeNull();
        storedPerson.Motto.ShouldBeNull();
    }

    private static ProfileController CreateController(IServiceScope scope, long userId, long personId)
    {
        var controller = new ProfileController(
            scope.ServiceProvider.GetRequiredService<IPersonService>()
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("id", userId.ToString()),
                    new Claim("personId", personId.ToString()),
                    new Claim(ClaimTypes.Role, "tourist")
                }))
            }
        };

        return controller;
    }
}
