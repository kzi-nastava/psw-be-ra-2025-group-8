using Explorer.API.Controllers.User;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubJoinRequestTests : BaseStakeholdersIntegrationTest
{
    public ClubJoinRequestTests(StakeholdersTestFactory factory) : base(factory) { }

    private void CleanupRequests(StakeholdersContext db, long clubId, long touristId)
    {
        var existing = db.ClubJoinRequests
            .Where(r => r.ClubId == clubId && r.TouristId == touristId)
            .ToList();
        db.ClubJoinRequests.RemoveRange(existing);
        db.SaveChanges();
    }

    private void RemoveMemberFromClub(StakeholdersContext db, long clubId, long touristId)
    {
        var club = db.Clubs.FirstOrDefault(c => c.Id == clubId);
        if (club != null && club.MemberIds.Contains(touristId))
        {
            club.RemoveMember(touristId);
            db.SaveChanges();
        }
    }

    [Fact]
    public void Tourist_requests_to_join_club_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21"); // turista1
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -1, -21);

        // Act
        var result = ((ObjectResult)controller.RequestToJoin(-1).Result)?.Value as ClubJoinRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ClubId.ShouldBe(-1);
        result.TouristId.ShouldBe(-21);
        result.Status.ShouldBe("Pending");

        // Assert - Database
        var storedRequest = db.ClubJoinRequests.FirstOrDefault(r => r.Id == result.Id);
        storedRequest.ShouldNotBeNull();
        storedRequest.ClubId.ShouldBe(-1);
        storedRequest.TouristId.ShouldBe(-21);
    }

    [Fact]
    public void Tourist_cannot_request_to_join_club_twice()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); // turista2
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -3, -22);

        // First request
        controller.RequestToJoin(-3);

        // Act & Assert - Second request should fail
        Should.Throw<InvalidOperationException>(() => controller.RequestToJoin(-3));
    }

    [Fact]
    public void Tourist_cannot_request_to_join_if_already_member()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11"); // autor1 je vec clan kluba -1

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => controller.RequestToJoin(-1));
    }

    [Fact]
    public void Tourist_cancels_join_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23"); // turista3
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -2, -23);

        // Create request first
        var createResult = ((ObjectResult)controller.RequestToJoin(-2).Result)?.Value as ClubJoinRequestDto;
        createResult.ShouldNotBeNull();
        var requestId = createResult.Id;

        // Act - Cancel the request
        var result = controller.CancelJoinRequest(requestId);

        // Assert - Response
        result.ShouldBeOfType<OkResult>();

        // Assert - Database (request should be deleted)
        var deletedRequest = db.ClubJoinRequests.FirstOrDefault(r => r.Id == requestId);
        deletedRequest.ShouldBeNull();
    }

    [Fact]
    public void Tourist_cannot_cancel_someone_elses_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller1 = CreateController(scope, "-21"); // turista1
        var controller2 = CreateController(scope, "-23"); // turista3
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -3, -21);

        // turista1 creates request
        var createResult = ((ObjectResult)controller1.RequestToJoin(-3).Result)?.Value as ClubJoinRequestDto;
        var requestId = createResult.Id;

        // Act & Assert - turista3 tries to cancel turista1's request
        Should.Throw<UnauthorizedAccessException>(() => controller2.CancelJoinRequest(requestId));
    }

    [Fact]
    public void Owner_accepts_join_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateController(scope, "-23"); // turista3
        var ownerController = CreateController(scope, "-11"); // autor1 (owner of club -1)
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Clean up any existing memberships and requests
        RemoveMemberFromClub(db, -1, -23);
        CleanupRequests(db, -1, -23);

        // Tourist creates request
        var createResult = ((ObjectResult)touristController.RequestToJoin(-1).Result)?.Value as ClubJoinRequestDto;
        var requestId = createResult.Id;

        // Act - Owner accepts request
        var result = ((ObjectResult)ownerController.AcceptJoinRequest(requestId).Result)?.Value as ClubJoinRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Accepted");

        // Assert - Database: tourist should be added to club members
        var club = db.Clubs.FirstOrDefault(c => c.Id == -1);
        club.ShouldNotBeNull();
        club.MemberIds.ShouldContain(-23);

        // Assert - Database: request should be deleted after acceptance
        var deletedRequest = db.ClubJoinRequests.FirstOrDefault(r => r.Id == requestId);
        deletedRequest.ShouldBeNull();

        // Assert - Notification should be created
        var notification = db.Notifications.FirstOrDefault(n => 
            n.UserId == -23 && 
            n.Title == "Join Request Accepted");
        notification.ShouldNotBeNull();
    }

    [Fact]
    public void Owner_rejects_join_request_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateController(scope, "-21"); // turista1
        var ownerController = CreateController(scope, "-22"); // turista2 is also owner of club -2
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -2, -21);

        // Tourist creates request to join club -2
        var createResult = ((ObjectResult)touristController.RequestToJoin(-2).Result)?.Value as ClubJoinRequestDto;
        var requestId = createResult.Id;

        // Act - Owner rejects request
        var result = ((ObjectResult)ownerController.RejectJoinRequest(requestId).Result)?.Value as ClubJoinRequestDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Rejected");

        // Assert - Database: tourist should NOT be added to club members
        var club = db.Clubs.FirstOrDefault(c => c.Id == -2);
        club.ShouldNotBeNull();
        club.MemberIds.ShouldNotContain(-21);

        // Assert - Database: request should be deleted after rejection
        var deletedRequest = db.ClubJoinRequests.FirstOrDefault(r => r.Id == requestId);
        deletedRequest.ShouldBeNull();

        // Assert - Notification should be created
        var notification = db.Notifications.FirstOrDefault(n => 
            n.UserId == -21 && 
            n.Title == "Join Request Rejected");
        notification.ShouldNotBeNull();
    }

    [Fact]
    public void Non_owner_cannot_accept_join_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateController(scope, "-22"); // turista2
        var nonOwnerController = CreateController(scope, "-23"); // turista3 (not owner of club -1)
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -1, -22);

        // Tourist creates request
        var createResult = ((ObjectResult)touristController.RequestToJoin(-1).Result)?.Value as ClubJoinRequestDto;
        var requestId = createResult.Id;

        // Act & Assert - Non-owner tries to accept
        Should.Throw<UnauthorizedAccessException>(() => nonOwnerController.AcceptJoinRequest(requestId));
    }

    [Fact]
    public void Non_owner_cannot_reject_join_request()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var touristController = CreateController(scope, "-23"); // turista3
        var nonOwnerController = CreateController(scope, "-21"); // turista1 (not owner of club -3)
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        CleanupRequests(db, -3, -23);

        // Tourist creates request
        var createResult = ((ObjectResult)touristController.RequestToJoin(-3).Result)?.Value as ClubJoinRequestDto;
        var requestId = createResult.Id;

        // Act & Assert - Non-owner tries to reject
        Should.Throw<UnauthorizedAccessException>(() => nonOwnerController.RejectJoinRequest(requestId));
    }

    [Fact]
    public void Owner_gets_all_pending_join_requests_for_club()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tourist1Controller = CreateController(scope, "-21"); // turista1
        var tourist2Controller = CreateController(scope, "-22"); // turista2
        var ownerController = CreateController(scope, "-11"); // autor1 (owner of club -1)
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Clean ALL requests for club -1 to ensure clean state
        var allRequestsForClub = db.ClubJoinRequests.Where(r => r.ClubId == -1).ToList();
        db.ClubJoinRequests.RemoveRange(allRequestsForClub);
        db.SaveChanges();

        // Two tourists create requests
        tourist1Controller.RequestToJoin(-1);
        tourist2Controller.RequestToJoin(-1);

        // Act - Owner gets all requests
        var result = ((ObjectResult)ownerController.GetClubJoinRequests(-1).Result)?.Value as IEnumerable<ClubJoinRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        var requests = result.ToList();
        requests.Count.ShouldBe(2);
        requests.ShouldAllBe(r => r.ClubId == -1);
        requests.ShouldAllBe(r => r.Status == "Pending");
    }

    [Fact]
    public void Non_owner_cannot_get_club_join_requests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var nonOwnerController = CreateController(scope, "-22"); // turista2 (not owner of club -1)

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => nonOwnerController.GetClubJoinRequests(-1));
    }

    [Fact]
    public void Tourist_gets_own_pending_join_requests()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-23"); // turista3
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        
        // Clean ALL requests for tourist -23 to ensure clean state
        var allRequestsForTourist = db.ClubJoinRequests.Where(r => r.TouristId == -23).ToList();
        db.ClubJoinRequests.RemoveRange(allRequestsForTourist);
        
        // Clean up any existing memberships
        RemoveMemberFromClub(db, -1, -23);
        RemoveMemberFromClub(db, -2, -23);
        db.SaveChanges();

        // Create requests to two different clubs
        controller.RequestToJoin(-1);
        controller.RequestToJoin(-2);

        // Act
        var result = ((ObjectResult)controller.GetMyJoinRequests().Result)?.Value as IEnumerable<ClubJoinRequestDto>;

        // Assert
        result.ShouldNotBeNull();
        var requests = result.ToList();
        requests.Count.ShouldBe(2);
        requests.ShouldAllBe(r => r.TouristId == -23);
        requests.ShouldAllBe(r => r.Status == "Pending");
    }

    private static ClubsController CreateController(IServiceScope scope, string userId)
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}

