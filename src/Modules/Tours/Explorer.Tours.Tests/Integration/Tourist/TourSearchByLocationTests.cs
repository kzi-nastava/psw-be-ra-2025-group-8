using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourSearchByLocationTests : BaseToursIntegrationTest
{
    public TourSearchByLocationTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void SearchByLocation_returns_tours_within_range()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Pretpostavljam da postoji tura sa keypoint-om u blizini ove lokacije
        var searchDto = new TourSearchByLocationDto
        {
            Latitude = 45.0,
            Longitude = 19.0,
            DistanceInKilometers = 100.0
        };

        var result = (ObjectResult)controller.SearchToursByLocation(searchDto);
        var tours = result.Value as List<TouristTourPreviewDto>;

        result.StatusCode.ShouldBe(200);
        tours.ShouldNotBeNull();
    }

    [Fact]
    public void SearchByLocation_returns_bad_request_for_negative_distance()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var searchDto = new TourSearchByLocationDto
        {
            Latitude = 45.0,
            Longitude = 19.0,
            DistanceInKilometers = -10.0
        };

        var result = controller.SearchToursByLocation(searchDto);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void SearchByLocation_returns_bad_request_for_null_dto()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = controller.SearchToursByLocation(null);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void SearchByLocation_returns_empty_list_when_no_tours_in_range()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Lokacija daleko od svih tura
        var searchDto = new TourSearchByLocationDto
        {
            Latitude = -80.0,
            Longitude = -170.0,
            DistanceInKilometers = 10.0
        };

        var result = (ObjectResult)controller.SearchToursByLocation(searchDto);
        var tours = result.Value as List<TouristTourPreviewDto>;

        result.StatusCode.ShouldBe(200);
        tours.ShouldNotBeNull();
        tours.ShouldBeEmpty();
    }

    // =====================================================
    // Helper
    // =====================================================

    private static TouristTourController CreateController(IServiceScope scope)
    {
        return new TouristTourController(scope.ServiceProvider.GetRequiredService<ITouristTourService>())
        {
            ControllerContext = BuildContext("-1") // turist context
        };
    }
}
