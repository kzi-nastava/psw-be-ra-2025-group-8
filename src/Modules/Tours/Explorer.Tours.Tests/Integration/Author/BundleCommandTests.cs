using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class BundleCommandTests : BaseToursIntegrationTest
{
    public BundleCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_bundle_draft_for_author_with_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new CreateBundleDto
        {
            Name = "Test bundle create",
            Price = 120.50m,
            TourIds = new List<long> { -13, -14 } // PUBLISHED + owned by author -12 (c-tours.sql)
        };

        var action = controller.Create(dto);
        var ok = action.Result as OkObjectResult;
        var result = ok?.Value as BundleDto;

        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);
        result.AuthorId.ShouldBe(-12);
        result.Name.ShouldBe(dto.Name);
        result.Price.ShouldBe(dto.Price);
        result.Status.ShouldBe("Draft");

        var stored = dbContext.Bundles
            .Include(b => b.BundleTours)
            .FirstOrDefault(b => b.Id == result.Id);
        stored.ShouldNotBeNull();
        stored!.AuthorId.ShouldBe(-12);
        stored.Status.ShouldBe(BundleStatus.Draft);
        stored.BundleTours.Count.ShouldBe(2);
        stored.BundleTours.Select(bt => bt.TourId).ShouldBe(new[] { -13L, -14L }, ignoreOrder: true);
    }

    [Fact]
    public void Updates_bundle_only_while_draft()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((controller.Create(new CreateBundleDto
        {
            Name = "Draft bundle to update",
            Price = 99m,
            TourIds = new List<long> { -13, -14 }
        }).Result as OkObjectResult)!.Value as BundleDto)!
            ?? throw new Exception("Bundle create failed in test setup.");

        var updateDto = new UpdateBundleDto
        {
            Name = "Draft bundle updated",
            Price = 150m,
            TourIds = new List<long> { -13, -14 }
        };
        var action = controller.Update(created.Id, updateDto);
        var ok = action.Result as OkObjectResult;
        var result = ok?.Value as BundleDto;

        result.ShouldNotBeNull();
        result!.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(updateDto.Name);
        result.Price.ShouldBe(updateDto.Price);
        result.Status.ShouldBe("Draft");

        var stored = dbContext.Bundles.First(b => b.Id == created.Id);
        stored.Name.ShouldBe(updateDto.Name);
        stored.Price.ShouldBe(updateDto.Price);
    }

    [Fact]
    public void Publish_sets_status_and_timestamp()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((controller.Create(new CreateBundleDto
        {
            Name = "Bundle to publish",
            Price = 10m,
            TourIds = new List<long> { -13, -14 }
        }).Result as OkObjectResult)!.Value as BundleDto)!;

        var action = controller.Publish(created.Id);
        var ok = action.Result as OkObjectResult;
        var result = ok?.Value as BundleDto;

        result.ShouldNotBeNull();
        result!.Status.ShouldBe("Published");
        result.PublishedAt.ShouldNotBeNull();

        var stored = dbContext.Bundles.First(b => b.Id == created.Id);
        stored.Status.ShouldBe(BundleStatus.Published);
        stored.PublishedAt.ShouldNotBeNull();
        stored.ArchivedAt.ShouldBeNull();
    }

    [Fact]
    public void Archive_sets_status_and_timestamp()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((controller.Create(new CreateBundleDto
        {
            Name = "Bundle to archive",
            Price = 10m,
            TourIds = new List<long> { -13, -14 }
        }).Result as OkObjectResult)!.Value as BundleDto)!;

        controller.Publish(created.Id);

        var action = controller.Archive(created.Id);
        var ok = action.Result as OkObjectResult;
        var result = ok?.Value as BundleDto;

        result.ShouldNotBeNull();
        result!.Status.ShouldBe("Archived");
        result.ArchivedAt.ShouldNotBeNull();

        var stored = dbContext.Bundles.First(b => b.Id == created.Id);
        stored.Status.ShouldBe(BundleStatus.Archived);
        stored.ArchivedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Create_fails_if_tours_are_not_published()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        var dto = new CreateBundleDto
        {
            Name = "Invalid bundle",
            Price = 10m,
            TourIds = new List<long> { -10, -11 } // Draft tours in c-tours.sql
        };

        Should.Throw<InvalidOperationException>(() => controller.Create(dto));
    }

    [Fact]
    public void Create_fails_if_author_tries_to_add_someone_elses_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        var dto = new CreateBundleDto
        {
            Name = "Invalid bundle - foreign tours",
            Price = 10m,
            TourIds = new List<long> { -13, -14 } // owned by author -12
        };

        Should.Throw<ForbiddenException>(() => controller.Create(dto));
    }

    [Fact]
    public void Delete_removes_draft_bundle()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = ((controller.Create(new CreateBundleDto
        {
            Name = "Bundle to delete",
            Price = 10m,
            TourIds = new List<long> { -13, -14 }
        }).Result as OkObjectResult)!.Value as BundleDto)!;

        var response = controller.Delete(created.Id) as OkResult;
        response.ShouldNotBeNull();
        response!.StatusCode.ShouldBe(200);

        dbContext.Bundles.FirstOrDefault(b => b.Id == created.Id).ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_for_published_bundle()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");

        var created = ((controller.Create(new CreateBundleDto
        {
            Name = "Published bundle delete attempt",
            Price = 10m,
            TourIds = new List<long> { -13, -14 }
        }).Result as OkObjectResult)!.Value as BundleDto)!;

        controller.Publish(created.Id);

        Should.Throw<InvalidOperationException>(() => controller.Delete(created.Id));
    }

    private static BundleController CreateController(IServiceScope scope, string authorId)
    {
        return new BundleController(scope.ServiceProvider.GetRequiredService<IBundleService>())
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}

