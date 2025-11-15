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
            Password = "pass",
            Email = "newadmin@test.com",
            Name = "New",
            Surname = "Admin",
            // ISPRAVKA ULOGE: Promenjeno iz "Admin" u "Administrator"
            Role = "Administrator"
        };

        // ISPRAVLJENO: Dodato .Result zbog ActionResult<T>
        var result = ((ObjectResult)controller.CreateAccount(dto).Result).Value as AccountDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Role.ShouldBe("Administrator"); // Provera je ažurirana

        // Database assertions
        db.ChangeTracker.Clear();
        var stored = db.Users.FirstOrDefault(u => u.Username == dto.Username);
        stored.ShouldNotBeNull();
        stored.Role.ToString().ShouldBe("Administrator"); // Provera je ažurirana
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
            Password = "pass",
            Email = "authorNew@test.com",
            Name = "Auth",
            Surname = "Or",
            Role = "Author"
        };

        // ISPRAVLJENO: Dodato .Result zbog ActionResult<T>
        var result = ((ObjectResult)controller.CreateAccount(dto).Result).Value as AccountDto;

        result.ShouldNotBeNull();
        result.Role.ShouldBe("Author");

        db.ChangeTracker.Clear();
        // ISPRAVLJENO: Koristi se Username jer klasa User nema Email svojstvo
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

        // ISPRAVKA: Dodavanje .Result da bi se rešio Task<ActionResult<T>>
        var actionResult = controller.GetAll().Result;
        var result = ((ObjectResult)actionResult).Value as IEnumerable<AccountOverviewDto>;

        result.ShouldNotBeNull();
        // Očekuje se > 0 rezultata nakon ispravne inicijalizacije baze
        result.Count().ShouldBeGreaterThan(0);

        foreach (var item in result)
        {
            item.ShouldNotBeNull();
            item.Username.ShouldNotBeNull();
            // Uklanjanje nepostojećeg svojstva Password
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

        // ISPRAVKA: Prosleđivanje ActiveStateDto umesto bool
        controller.SetActiveState(-101, new AdminUsersController.ActiveStateDto { IsActive = false });

        db.ChangeTracker.Clear();
        var user = db.Users.First(u => u.Id == -101); // Korisnik -101 mora da postoji
        user.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Unblocks_existing_user()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        // ISPRAVKA: Prosleđivanje ActiveStateDto umesto bool
        controller.SetActiveState(-102, new AdminUsersController.ActiveStateDto { IsActive = true });

        db.ChangeTracker.Clear();
        var user = db.Users.First(u => u.Id == -102); // Korisnik -102 mora da postoji
        user.IsActive.ShouldBeTrue();
    }

    private static AdminUsersController CreateController(IServiceScope scope)
    {
        return new AdminUsersController(
            scope.ServiceProvider.GetRequiredService<IAuthenticationService>()
        );
    }
}