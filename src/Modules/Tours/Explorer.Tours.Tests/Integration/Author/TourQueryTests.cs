using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourQueryTests : BaseToursIntegrationTest
{
 public TourQueryTests(ToursTestFactory factory) : base(factory) { }

 [Fact]
 public void Retrieves_all_paged()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var result = ((ObjectResult)controller.GetAll(0,0).Result)?.Value as PagedResult<TourDto>;
 result.ShouldNotBeNull();
 result.TotalCount.ShouldBeGreaterThan(0); // seeded tours
 result.Results.Any(r => r.Id == -10).ShouldBeTrue();
 }

 [Fact]
 public void Retrieves_my_tours_only()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var result = ((ObjectResult)controller.GetMyTours().Result)?.Value as List<TourDto>;
 result.ShouldNotBeNull();
 result.Count.ShouldBeGreaterThan(0);
 result.All(r => r.AuthorId == -1).ShouldBeTrue();
 result.Any(r => r.Id == -12).ShouldBeFalse(); // belongs to -2
 }

 private static TourController CreateController(IServiceScope scope, string authorId)
 {
 return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
 {
 ControllerContext = BuildContext(authorId)
 };
 }
}
