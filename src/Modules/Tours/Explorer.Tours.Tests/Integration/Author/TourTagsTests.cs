using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class TourTagsTests : BaseToursIntegrationTest
    {
        public TourTagsTests(ToursTestFactory factory) : base(factory) { }

        private static TourTagsController CreateController(IServiceScope scope, string authorId = "-1")
        {
            return new TourTagsController(
                scope.ServiceProvider.GetRequiredService<ITourService>()
            )
            {
                // authorId = -1 → owner of tour -10
                ControllerContext = BuildContext(authorId)
            };
        }

        // -------------------------------------------------------
        // UpdateTags sa praznom listom → nema tagova u DB
        // -------------------------------------------------------
        [Fact]
        public void UpdateTags_empty_clears_all_tags()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // očisti sve tag veze za turu -10
            db.TourTags.RemoveRange(db.TourTags.Where(tt => tt.TourId == -10));
            db.SaveChanges();

            var controller = CreateController(scope);

            var result = controller.UpdateTags(-10, new UpdateTourTagsDto { Tags = new List<string>() });
            var ok = result.Result as ObjectResult;
            var dto = ok?.Value as TourDto;

            dto.ShouldNotBeNull();
            dto.Tags.ShouldNotBeNull();
            dto.Tags.Count.ShouldBe(0);

            var dbCount = db.TourTags.Count(tt => tt.TourId == -10);
            dbCount.ShouldBe(0);
        }

        // -------------------------------------------------------
        // AddTag dodaje vezu u TourTags
        // -------------------------------------------------------
        [Fact]
        public void AddTag_adds_entry_to_db()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var controller = CreateController(scope);

            // počisti postojeće veze
            db.TourTags.RemoveRange(db.TourTags.Where(tt => tt.TourId == -10));
            db.SaveChanges();

            // Act
            var result = controller.AddTag(-10, "mountain");

            // Assert DB
            var tagEntity = db.Tags.Single(t => t.Tag == "mountain");
            var exists = db.TourTags
                .FirstOrDefault(tt => tt.TourId == -10 && tt.TagsId == tagEntity.Id);

            exists.ShouldNotBeNull();

            // dodatno: DTO treba da sadrži "mountain"
            var ok = result.Result as ObjectResult;
            var dto = ok?.Value as TourDto;
            dto.ShouldNotBeNull();
            dto.Tags.ShouldContain("mountain");
        }

        // -------------------------------------------------------
        // AddTag sprečava dupliranje istog taga na turi
        // -------------------------------------------------------
        [Fact]
        public void AddTag_prevents_duplicates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            controller.AddTag(-10, "food");

            Should.Throw<InvalidOperationException>(() =>
                controller.AddTag(-10, "food"));
        }

        // -------------------------------------------------------
        // RemoveTag uklanja vezu tag–tura
        // -------------------------------------------------------
        [Fact]
        public void RemoveTag_removes_entry()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var controller = CreateController(scope);

            // kreni od čistog stanja
            db.TourTags.RemoveRange(db.TourTags.Where(tt => tt.TourId == -10));
            db.SaveChanges();

            controller.AddTag(-10, "history");

            // Act
            controller.RemoveTag(-10, "history");

            // Assert DB
            var historyTag = db.Tags.Single(t => t.Tag == "history");
            var exists = db.TourTags
                .FirstOrDefault(tt => tt.TourId == -10 && tt.TagsId == historyTag.Id);

            exists.ShouldBeNull();
        }

        // -------------------------------------------------------
        // RemoveTag za tag koji nije vezan → ne puca
        // -------------------------------------------------------
        [Fact]
        public void RemoveTag_nonexistent_does_not_fail()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var controller = CreateController(scope);

            // očisti sve veze za "history"
            var historyTag = db.Tags.SingleOrDefault(t => t.Tag == "history");
            if (historyTag != null)
            {
                db.TourTags.RemoveRange(
                    db.TourTags.Where(tt => tt.TourId == -10 && tt.TagsId == historyTag.Id));
                db.SaveChanges();
            }

            var actionResult = controller.RemoveTag(-10, "history");
            var objectResult = actionResult.Result as ObjectResult;
            objectResult.ShouldNotBeNull();
            objectResult.StatusCode.ShouldBe(200);
        }

        // -------------------------------------------------------
        // Ne možeš da menjaš tagove na tuđoj turi
        // -------------------------------------------------------
        [Fact]
        public void AddTag_fails_for_other_author()
        {
            using var scope = Factory.Services.CreateScope();

            // authorId = -2 → nije vlasnik ture -10 (AuthorId = -1)
            var controller = CreateController(scope, authorId: "-2");

            Should.Throw<UnauthorizedAccessException>(() =>
                controller.AddTag(-10, "mountain"));
        }

        [Fact]
        public void RemoveTag_fails_for_other_author()
        {
            using var scope = Factory.Services.CreateScope();

            var controller = CreateController(scope, authorId: "-2");

            Should.Throw<UnauthorizedAccessException>(() =>
                controller.RemoveTag(-10, "mountain"));
        }

        // -------------------------------------------------------
        // AddTag baca grešku ako tura ne postoji
        // -------------------------------------------------------
        [Fact]
        public void AddTag_fails_for_missing_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            Should.Throw<KeyNotFoundException>(() =>
                controller.AddTag(-9999, "mountain"));
        }

        // -------------------------------------------------------
        // UpdateTags postavlja tačno zadatu listu tagova
        // -------------------------------------------------------
        [Fact]
        public void UpdateTags_sets_exact_tags_list()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var controller = CreateController(scope);

            // start: očisti sve veze
            db.TourTags.RemoveRange(db.TourTags.Where(tt => tt.TourId == -10));
            db.SaveChanges();

            var dtoTags = new List<string> { "mountain", "food" };

            var result = controller.UpdateTags(-10, new UpdateTourTagsDto { Tags = dtoTags });
            var ok = result.Result as ObjectResult;
            var dto = ok?.Value as TourDto;

            dto.ShouldNotBeNull();
            dto.Tags.ShouldNotBeNull();
            dto.Tags.Count.ShouldBe(2);
            dto.Tags.ShouldContain("mountain");
            dto.Tags.ShouldContain("food");

            // proveri i u bazi
            var mtId = db.Tags.Single(t => t.Tag == "mountain").Id;
            var foodId = db.Tags.Single(t => t.Tag == "food").Id;

            var links = db.TourTags.Where(tt => tt.TourId == -10).ToList();
            links.Count.ShouldBe(2);
            links.Any(l => l.TagsId == mtId).ShouldBeTrue();
            links.Any(l => l.TagsId == foodId).ShouldBeTrue();
        }
    }
}
