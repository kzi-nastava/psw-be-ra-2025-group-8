using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
 public TourCommandTests(ToursTestFactory factory) : base(factory) { }

 [Fact]
 public void Creates_tour_draft()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
 var newEntity = new TourDto
 {
 Name = "Nova test tura",
 Description = "Opis nove ture",
 Difficulty =2,
 Tags = new List<string> { "tagA", "tagB" }
 };
 var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourDto;
 result.ShouldNotBeNull();
 result.Id.ShouldNotBe(0);
 result.Name.ShouldBe(newEntity.Name);
 result.Status.ShouldBe("Draft");
 result.Price.ShouldBe(0);
 result.AuthorId.ShouldBe(-1);
 var stored = dbContext.Tours.FirstOrDefault(t => t.Name == newEntity.Name);
 stored.ShouldNotBeNull();
 stored!.AuthorId.ShouldBe(-1);
 stored.Status.ShouldBe(Core.Domain.TourStatus.Draft);
 }

 [Fact]
 public void Updates_tour_of_author()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
 var updated = new TourDto
 {
 Id = -10,
 Name = "Izmenjena Test Tour1",
 Description = "Izmenjen opis",
 Difficulty =4,
 Tags = new List<string> { "tagX" },
 Status = "Draft",
 Price =0,
 AuthorId = -1
 };
 var result = ((ObjectResult)controller.Update(updated).Result)?.Value as TourDto;
 result.ShouldNotBeNull();
 result.Id.ShouldBe(-10);
 result.Name.ShouldBe(updated.Name);
 result.Description.ShouldBe(updated.Description);
 var stored = dbContext.Tours.FirstOrDefault(t => t.Id == -10);
 stored.ShouldNotBeNull();
 stored!.Name.ShouldBe(updated.Name);
 stored.Description.ShouldBe(updated.Description);
 }

 [Fact]
 public void Update_fails_for_other_authors_tour()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var updated = new TourDto
 {
 Id = -12,
 Name = "Hack attempt",
 Description = "Neuspesna izmena",
 Difficulty =1,
 Tags = new List<string>(),
 Status = "Draft",
 Price =0,
 AuthorId = -1
 };
 var result = controller.Update(updated);
 result.Result.ShouldBeOfType<ForbidResult>();
 }

 [Fact]
 public void Deletes_draft_tour_of_author()
 {
 using var scope = Factory.Services.CreateScope();
 var controller = CreateController(scope, "-1");
 var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
 var response = (OkResult)controller.Delete(-11);
 response.StatusCode.ShouldBe(200);
 var stored = dbContext.Tours.FirstOrDefault(t => t.Id == -11);
 stored.ShouldBeNull();
 }

 private static TourController CreateController(IServiceScope scope, string authorId)
 {
 return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
 {
 ControllerContext = BuildContext(authorId)
 };
 }
}
