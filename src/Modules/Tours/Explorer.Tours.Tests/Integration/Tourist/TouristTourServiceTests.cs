using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristTourQueryTests : BaseToursIntegrationTest
{
    public TouristTourQueryTests(ToursTestFactory factory) : base(factory) { }

    // =====================================================
    // GET /api/tours/public
    // =====================================================

    [Fact]
    public void GetPublishedTours_returns_only_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = (ObjectResult)controller.GetPublishedTours();
        var tours = result.Value as List<TouristTourPreviewDto>;

        result.StatusCode.ShouldBe(200);
        tours.ShouldNotBeNull();
        tours.Count.ShouldBe(2);

        tours.All(t => t.Id == -13 || t.Id == -14).ShouldBeTrue();
    }

    // =====================================================
    // GET /api/tours/public/{id}
    // =====================================================

    [Fact]
    public void GetPublishedTour_returns_200_for_published_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = (ObjectResult)controller.GetPublishedTour(-13);
        var dto = result.Value as TouristTourDetailsDto;

        result.StatusCode.ShouldBe(200);
        dto.ShouldNotBeNull();
        dto.Id.ShouldBe(-13);
        dto.Name.ShouldBe("Test tour for preview 1");
    }

    [Fact]
    public void GetPublishedTour_returns_404_for_draft_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = controller.GetPublishedTour(-10);

        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void GetPublishedTour_returns_404_for_non_existing_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = controller.GetPublishedTour(999);

        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    // =====================================================
    // Helper
    // =====================================================

    private static TouristTourController CreateController(IServiceScope scope)
    {
        return new TouristTourController(
            scope.ServiceProvider.GetRequiredService<ITouristTourService>()
        )
        {
            ControllerContext = BuildContext("-1") // turist context
        };
    }
}
