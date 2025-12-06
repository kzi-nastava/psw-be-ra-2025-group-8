using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit;

/// <summary>
/// Unit tests for Tour aggregate root,
/// - adding key points
/// - computing tour length
/// - publishing and archiving rules
/// </summary>
public class TourAggregateTests
{
    private static Tour CreateDraftTour()
    {
        return new Tour(
            name: "Test tour",
            description: "Opis ture",
            difficulty: 2,
            authorId: -1);
    }

    [Fact]
    public void AddKeyPoint_first_point_keeps_length_zero()
    {
        // Arrange
        var tour = CreateDraftTour();

        // Act
        tour.AddKeyPoint(
            name: "Start",
            description: "Polazna tačka",
            imageUrl: "",
            secret: "Secret 1",
            location: new GeoCoordinate(45.0, 19.0));

        // Assert
        tour.KeyPoints.Count.ShouldBe(1);
        tour.LengthInKilometers.ShouldBe(0); // still no length with only one point
        tour.KeyPoints[0].Order.ShouldBe(1);
    }

    [Fact]
    public void AddKeyPoint_second_point_recalculates_length()
    {
        // Arrange
        var tour = CreateDraftTour();

        // Act
        tour.AddKeyPoint(
            name: "Start",
            description: "Polazna tačka",
            imageUrl: "",
            secret: "Secret 1",
            location: new GeoCoordinate(45.0, 19.0));

        tour.AddKeyPoint(
            name: "End",
            description: "Krajnja tačka",
            imageUrl: "",
            secret: "Secret 2",
            location: new GeoCoordinate(45.1, 19.1));

        // Assert
        tour.KeyPoints.Count.ShouldBe(2);
        tour.KeyPoints[0].Order.ShouldBe(1);
        tour.KeyPoints[1].Order.ShouldBe(2);
        tour.LengthInKilometers.ShouldBeGreaterThan(0);
    }

    [Theory]
    [InlineData(TourStatus.Published)]
    [InlineData(TourStatus.Archived)]
    public void AddKeyPoint_fails_when_tour_not_draft(TourStatus status)
    {
        // Arrange
        var tour = CreateDraftTour();
        tour.Status = status;

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            tour.AddKeyPoint(
                name: "Some point",
                description: "Opis",
                imageUrl: "",
                secret: "Secret",
                location: new GeoCoordinate(45.0, 19.0));
        });
    }

    [Fact]
    public void Publish_fails_when_less_than_two_key_points()
    {
        // Arrange
        var tour = CreateDraftTour();
        tour.TourTags.Add(new TourTag { TourId = tour.Id, TagsId = 1 });
        tour.SetTransportTime(TransportType.Walk, 120);

        // Act & Assert: bez ijedne ključne tačke
        Should.Throw<InvalidOperationException>(() => tour.Publish());

        // Dodamo jednu ključnu tačku – i dalje ne sme da se objavi
        tour.AddKeyPoint(
            name: "Only point",
            description: "Jedina tačka",
            imageUrl: "",
            secret: "Secret",
            location: new GeoCoordinate(45.0, 19.0));

        Should.Throw<InvalidOperationException>(() => tour.Publish());
    }

    [Fact]
    public void Publish_fails_when_basic_data_missing()
    {
        // Arrange
        var tour = CreateDraftTour();
        tour.Name = ""; // invalid name

        // We're adding two key points to satisfy that condition
        tour.AddKeyPoint(
            name: "A",
            description: "",
            imageUrl: "",
            secret: "",
            location: new GeoCoordinate(45.0, 19.0));

        tour.AddKeyPoint(
            name: "B",
            description: "",
            imageUrl: "",
            secret: "",
            location: new GeoCoordinate(45.1, 19.1));

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => tour.Publish());
    }

    [Fact]
    public void Publish_fails_when_no_transport_times_defined()
    {
        // Arrange
        var tour = CreateDraftTour();
        tour.TourTags.Add(new TourTag { TourId = tour.Id, TagsId = 1 });

        tour.AddKeyPoint("Start", "Polazna tačka", "", "", new GeoCoordinate(45.0, 19.0));
        tour.AddKeyPoint("End", "Krajnja tačka", "", "", new GeoCoordinate(45.1, 19.1));

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => tour.Publish());
    }

    [Fact]
    public void Publish_succeeds_when_all_rules_satisfied()
    {
        var tour = CreateDraftTour();
        tour.TourTags.Add(new TourTag { TourId = tour.Id, TagsId = 1 });

        tour.AddKeyPoint("Start", "Polazna tačka", "", "", new GeoCoordinate(45.0, 19.0));
        tour.AddKeyPoint("End", "Krajnja tačka", "", "", new GeoCoordinate(45.1, 19.1));

        tour.SetTransportTime(TransportType.Walk, 120);

        tour.Publish();

        tour.Status.ShouldBe(TourStatus.Published);
        tour.LengthInKilometers.ShouldBeGreaterThan(0);
        tour.PublishedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Archive_sets_status_to_archived()
    {
        // Arrange
        var tour = CreateDraftTour();

        // Act
        tour.Archive();

        // Assert
        tour.Status.ShouldBe(TourStatus.Archived);
    }
}
