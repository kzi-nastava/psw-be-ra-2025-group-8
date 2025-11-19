using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Tests;

public class PreferenceTagsTests : IClassFixture<StakeholdersTestFactory>
{
    private readonly StakeholdersTestFactory _factory;

    public PreferenceTagsTests(StakeholdersTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void GetTagsForPerson_Returns_existing_tags()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPreferenceTagsService>();

        var tags = svc.GetTagsForPerson(-21).ToList();
        tags.Count.ShouldBeGreaterThan(0);
        tags.Any(t => t.Tag == "mountain").ShouldBeTrue();
    }

    [Fact]
    public void AddTagForPerson_Adds_new_tag_or_returns_existing_and_links()
    {
        using var scope = _factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<IPreferenceTagsService>();
        var ctx = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

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
        var ctx = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var pref = ctx.TouristPreferences.Single(tp => tp.PersonId == -21);
        // ensure we have an existing tag -201 linked (-301)
        var existingLink = ctx.PreferenceTags.SingleOrDefault(pt => pt.TouristPreferencesId == pref.Id && pt.TagsId == -201);
        existingLink.ShouldNotBeNull();

        svc.RemoveTagFromPerson(-21, -201);

        var after = ctx.PreferenceTags.SingleOrDefault(pt => pt.TouristPreferencesId == pref.Id && pt.TagsId == -201);
        after.ShouldBeNull();
    }
}

