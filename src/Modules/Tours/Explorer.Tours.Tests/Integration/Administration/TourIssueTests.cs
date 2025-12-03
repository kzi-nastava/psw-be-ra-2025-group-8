using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration;

[Collection("Sequential")]
public class TourIssueTests : BaseToursIntegrationTest
{
    public TourIssueTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Author_responds_to_issue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsAuthor(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        var request = new ReportProblemController.AuthorResponseRequest
        {
            Response = "Hvala na prijavi! Radimo na rešenju problema."
        };

        // Act
        var result = ((ObjectResult)controller.AuthorRespond(-1, request).Result)?.Value as ReportProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(-1);
        result.AuthorId.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-1); // AuthorId iz BuildContext je -1
        result.AuthorResponse.ShouldBe("Hvala na prijavi! Radimo na rešenju problema.");
        result.AuthorResponseTime.ShouldNotBeNull();
        result.AuthorResponseTime.Value.ShouldBeGreaterThan(DateTime.MinValue);
        result.AuthorResponseTime.Value.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity!.AuthorId.ShouldBe(-1);
        storedEntity.AuthorResponse.ShouldBe("Hvala na prijavi! Radimo na rešenju problema.");
        storedEntity.AuthorResponseTime.ShouldNotBeNull();
    }

    [Fact]
    public void Author_respond_fails_with_empty_response()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsAuthor(scope);
        
        var request = new ReportProblemController.AuthorResponseRequest
        {
            Response = "" // Invalid - empty
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.AuthorRespond(-1, request));
    }

    [Fact]
    public void Tourist_marks_issue_as_resolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "3"); // TouristId 3 for report -3
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        var request = new ReportProblemController.TouristResolutionRequest
        {
            IsResolved = true,
            Comment = "Problem je rešen. Hvala na brzom odgovoru!"
        };

        // Act
        var result = ((ObjectResult)controller.MarkResolved(-3, request).Result)?.Value as ReportProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(-3);
        result.IsResolved.ShouldNotBeNull();
        result.IsResolved.Value.ShouldBeTrue();
        result.TouristResolutionComment.ShouldBe("Problem je rešen. Hvala na brzom odgovoru!");
        result.TouristResolutionTime.ShouldNotBeNull();
        result.TouristResolutionTime.Value.ShouldBeGreaterThan(DateTime.MinValue);
        result.TouristResolutionTime.Value.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == -3);
        storedEntity.ShouldNotBeNull();
        storedEntity!.IsResolved.ShouldNotBeNull();
        storedEntity.IsResolved.Value.ShouldBeTrue();
        storedEntity.TouristResolutionComment.ShouldBe("Problem je rešen. Hvala na brzom odgovoru!");
    }

    [Fact]
    public void Tourist_marks_issue_as_unresolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1"); // TouristId 1 for report -4
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        var request = new ReportProblemController.TouristResolutionRequest
        {
            IsResolved = false,
            Comment = "Problem i dalje postoji. Molim vas za dodatne informacije."
        };

        // Act
        var result = ((ObjectResult)controller.MarkResolved(-4, request).Result)?.Value as ReportProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(-4);
        result.IsResolved.ShouldNotBeNull();
        result.IsResolved.Value.ShouldBeFalse();
        result.TouristResolutionComment.ShouldBe("Problem i dalje postoji. Molim vas za dodatne informacije.");

        // Assert - Database
        var storedEntity = dbContext.ReportProblem.FirstOrDefault(i => i.Id == -4);
        storedEntity.ShouldNotBeNull();
        storedEntity!.IsResolved.ShouldNotBeNull();
        storedEntity.IsResolved.Value.ShouldBeFalse();
    }

    [Fact]
    public void Adds_message_to_issue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1"); // Tourist 1 for report -1
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        var request = new ReportProblemController.AddMessageRequest
        {
            Content = "Ovo je test poruka u okviru prijave problema."
        };

        // Act
        var result = ((ObjectResult)controller.AddMessage(-1, request).Result)?.Value as IssueMessageDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);
        result.ReportProblemId.ShouldBe(-1);
        result.AuthorId.ShouldBe(1); // Iz BuildContext (tourist 1)
        result.Content.ShouldBe("Ovo je test poruka u okviru prijave problema.");
        result.CreatedAt.ShouldBeGreaterThan(DateTime.MinValue);
        result.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);

        // Assert - Database
        var storedReport = dbContext.ReportProblem.FirstOrDefault(r => r.Id == -1);
        storedReport.ShouldNotBeNull();
        storedReport!.Messages.Count.ShouldBeGreaterThan(0);
        var lastMessage = storedReport.Messages.LastOrDefault();
        lastMessage.ShouldNotBeNull();
        lastMessage!.Content.ShouldBe("Ovo je test poruka u okviru prijave problema.");
        lastMessage.AuthorId.ShouldBe(1);
    }

    [Fact]
    public void Add_message_fails_with_empty_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1");
        
        var request = new ReportProblemController.AddMessageRequest
        {
            Content = "" // Invalid - empty
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.AddMessage(-1, request));
    }

    [Fact]
    public void Retrieves_all_messages_for_issue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        // Dodaj nekoliko poruka prvo
        var request1 = new ReportProblemController.AddMessageRequest { Content = "Prva poruka" };
        var request2 = new ReportProblemController.AddMessageRequest { Content = "Druga poruka" };
        var request3 = new ReportProblemController.AddMessageRequest { Content = "Tre?a poruka" };
        
        controller.AddMessage(-1, request1);
        controller.AddMessage(-1, request2);
        controller.AddMessage(-1, request3);

        // Act
        var result = ((ObjectResult)controller.GetMessages(-1).Result)?.Value as List<IssueMessageDto>;

        // Assert
        result.ShouldNotBeNull();
        result!.Count.ShouldBeGreaterThanOrEqualTo(3);
        
        // Proveri da li postoje dodate poruke
        result.Any(m => m.Content == "Prva poruka").ShouldBeTrue();
        result.Any(m => m.Content == "Druga poruka").ShouldBeTrue();
        result.Any(m => m.Content == "Tre?a poruka").ShouldBeTrue();
        
        // Sve poruke treba da budu za isti ReportProblemId
        result.All(m => m.ReportProblemId == -1).ShouldBeTrue();
    }

    [Fact]
    public void Get_messages_returns_empty_list_for_issue_without_messages()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1"); // Tourist 1 for report -4
        
        // Act - koristi problem koji nema poruke
        var result = ((ObjectResult)controller.GetMessages(-4).Result)?.Value as List<IssueMessageDto>;

        // Assert
        result.ShouldNotBeNull();
        result!.Count.ShouldBe(0);
    }

    [Fact]
    public void Get_messages_fails_for_nonexistent_issue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1");

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.GetMessages(-9999));
    }

    [Fact]
    public void GetById_returns_issue_with_all_details()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1"); // Tourist 1 for report -1

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as ReportProblemDto;

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(-1);
        result.TourId.ShouldBe(-1); // Changed from 1 to -1
        result.TouristId.ShouldBe(1);
        result.Category.ShouldBe(0); // Technical
        result.Priority.ShouldBe(2); // High
        result.Description.ShouldNotBeNullOrEmpty();
        result.ReportTime.ShouldBeGreaterThan(DateTime.MinValue);
    }

    [Fact]
    public void GetById_fails_for_nonexistent_issue()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateControllerAsTourist(scope, "1");

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.GetById(-9999));
    }

    // Helper methods for creating controllers with different user contexts
    private static ReportProblemController CreateControllerAsTourist(IServiceScope scope, string touristId)
    {
        return new ReportProblemController(
            scope.ServiceProvider.GetRequiredService<IReportProblemService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Author.ITourService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }

    private static ReportProblemController CreateControllerAsAuthor(IServiceScope scope)
    {
        // Author ID -1 owns Tour ID -1 (from test data)
        return new ReportProblemController(
            scope.ServiceProvider.GetRequiredService<IReportProblemService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Author.ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    private static ReportProblemController CreateController(IServiceScope scope)
    {
        return new ReportProblemController(
            scope.ServiceProvider.GetRequiredService<IReportProblemService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Author.ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
