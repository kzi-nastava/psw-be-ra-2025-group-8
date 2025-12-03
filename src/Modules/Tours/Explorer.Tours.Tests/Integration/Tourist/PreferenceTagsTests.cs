using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Tests;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class PreferenceTagsTests : IClassFixture<ToursTestFactory>
{
    private readonly ToursTestFactory _factory;

    public PreferenceTagsTests(ToursTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void GetTagsForPerson_Returns_existing_tags()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPreferenceTagsService>();
        var ctx = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var pref = ctx.TouristPreferences.Single(tp => tp.PersonId == -21);

        // Reset current state so test is deterministic regardless of execution order
        ctx.PreferenceTags.RemoveRange(ctx.PreferenceTags.Where(pt => pt.TouristPreferencesId == pref.Id));
        ctx.SaveChanges();

        // Ensure baseline tags exist for this person
        svc.AddTagForPerson(-21, new TagDto { Tag = "mountain" });
        svc.AddTagForPerson(-21, new TagDto { Tag = "food" });

        // Act
        var tags = svc.GetTagsForPerson(-21).ToList();

        // Assert
        tags.ShouldNotBeNull();
        tags.Count.ShouldBe(2, "Person -21 treba da ima tačno 2 tagova: mountain i food");

        // Proveri da sadrži očekivane tagove
        var tagNames = tags.Select(t => t.Tag).ToList();
        tagNames.ShouldContain("mountain", "Person -21 treba da ima tag 'mountain'");
        tagNames.ShouldContain("food", "Person -21 treba da ima tag 'food'");

        // Proveri da ne sadrži tagove koji nisu povezani sa tom osobom
        tagNames.ShouldNotContain("history", "Person -21 ne treba da ima tag 'history' (on je povezan sa person -22)");

        // Proveri da tagovi imaju postavljene ID-ove (ne 0)
        var mountainTag = tags.FirstOrDefault(t => t.Tag == "mountain");
        mountainTag.ShouldNotBeNull();
        mountainTag.Id.ShouldNotBe(0, "Tag 'mountain' treba da ima postavljen ID");

        var foodTag = tags.FirstOrDefault(t => t.Tag == "food");
        foodTag.ShouldNotBeNull();
        foodTag.Id.ShouldNotBe(0, "Tag 'food' treba da ima postavljen ID");
    }

    [Fact]
    public void AddTagForPerson_Adds_new_tag_or_returns_existing_and_links()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPreferenceTagsService>();
        var ctx = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // add brand new tag
        var newTagDto = new TagDto { Tag = "beach" };
        var result = svc.AddTagForPerson(-21, newTagDto);

        result.ShouldNotBeNull();
        result.Tag.ShouldBe("beach");

        // ensure link exists in PreferenceTags
        var pref = ctx.TouristPreferences.Single(tp => tp.PersonId == -21);
        var linked = ctx.PreferenceTags.SingleOrDefault(pt => pt.TouristPreferencesId == pref.Id && pt.TagsId == result.Id);
        linked.ShouldNotBeNull();
    }

    [Fact]
    public void RemoveTagFromPerson_Removes_link_only()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPreferenceTagsService>();
        var ctx = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // 1) Uzmi preference za osobu -21
        var pref = ctx.TouristPreferences.Single(tp => tp.PersonId == -21);

        // 2) Kreiraj tag (EF ce sam dodeliti ID)
        var tag = new Tags("mountain");
        ctx.Tags.Add(tag);
        ctx.SaveChanges();
        // sada tag.Id ima stvarnu vrednost (pozitivan broj)

        // 3) Napravi PreferenceTags vezu
        ctx.PreferenceTags.Add(new PreferenceTags
        {
            TouristPreferencesId = pref.Id,
            TagsId = tag.Id
        });
        ctx.SaveChanges();

        // 4) Uveri se da link postoji pre brisanja
        var existingLink = ctx.PreferenceTags
            .SingleOrDefault(pt => pt.TouristPreferencesId == pref.Id && pt.TagsId == tag.Id);
        existingLink.ShouldNotBeNull();

        // 5) Akcija — brisanje tag veze
        svc.RemoveTagFromPerson(-21, tag.Id);

        // 6) Proveri da je veza uklonjena
        var after = ctx.PreferenceTags
            .SingleOrDefault(pt => pt.TouristPreferencesId == pref.Id && pt.TagsId == tag.Id);
        after.ShouldBeNull();
    }

}

