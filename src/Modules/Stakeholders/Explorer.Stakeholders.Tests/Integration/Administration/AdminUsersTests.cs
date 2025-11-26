using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Administrator;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminUsersTests : BaseStakeholdersIntegrationTest
{
    public AdminUsersTests(StakeholdersTestFactory factory) : base(factory) { }

    // -------------------------
    // CREATE ACCOUNT TESTS
    // -------------------------
    [Fact]
    public void Creates_admin_account_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var dto = new AdminCreateAccountDto
        {
            Username = "newadmin@test.com",
            Password = "password",
            Email = "newadmin@test.com",
            Role = "Administrator"
        };

        var actionResult = controller.CreateAccount(dto).Result;
        var createdResult = actionResult.ShouldBeOfType<CreatedAtActionResult>();
        var result = createdResult.Value.ShouldBeOfType<AccountDto>();

        // Response assertions
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Role.ShouldBe("Administrator"); 

        // Database assertions
        db.ChangeTracker.Clear();
        var stored = db.Users.FirstOrDefault(u => u.Username == dto.Username);
        stored.ShouldNotBeNull();
        stored.Role.ToString().ShouldBe("Administrator"); 
    }

    [Fact]
    public void Creates_author_account_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var dto = new AdminCreateAccountDto
        {
            Username = "authorNew@test.com",
            Password = "password",
            Email = "authorNew@test.com",
            Role = "Author"
        };
        var actionResult = controller.CreateAccount(dto).Result;
        var createdResult = actionResult.ShouldBeOfType<CreatedAtActionResult>();
        var result = createdResult.Value.ShouldBeOfType<AccountDto>();

        result.ShouldNotBeNull();
        result.Role.ShouldBe("Author");

        db.ChangeTracker.Clear();
        db.Users.FirstOrDefault(u => u.Username == dto.Username).ShouldNotBeNull();
    }



    // -------------------------
    // GET ALL USERS TEST
    // -------------------------
    [Fact]
    public void Gets_all_accounts_without_passwords()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var actionResult = controller.GetAll().Result;
        var result = ((ObjectResult)actionResult).Value as IEnumerable<AccountOverviewDto>;

        result.ShouldNotBeNull();
        // expected > 0 results after database initialization
        result.Count().ShouldBeGreaterThan(0);

        foreach (var item in result)
        {
            item.ShouldNotBeNull();
            item.Username.ShouldNotBeNull();
        }
    }



    // -------------------------
    // BLOCKING USERS
    // -------------------------
    [Fact]
    public void Blocks_existing_user()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        controller.SetActiveState(-101, new AdminUsersController.ActiveStateDto { IsActive = false });

        db.ChangeTracker.Clear();
        var user = db.Users.First(u => u.Id == -101); // user -101 must exist
        user.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Unblocks_existing_user()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        controller.SetActiveState(-102, new AdminUsersController.ActiveStateDto { IsActive = true });

        db.ChangeTracker.Clear();
        var user = db.Users.First(u => u.Id == -102); // user -102 must exist
        user.IsActive.ShouldBeTrue();
    }

    private static AdminUsersController CreateController(IServiceScope scope)
    {
        return new AdminUsersController(
            scope.ServiceProvider.GetRequiredService<IAuthenticationService>()
        );
    }
}