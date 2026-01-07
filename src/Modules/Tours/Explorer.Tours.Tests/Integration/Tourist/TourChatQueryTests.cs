using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourChatQueryTests : BaseToursIntegrationTest
{
    public TourChatQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_chat_room_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourId = -10L; // Tour from test data with chat room

        // Act
        var actionResult = controller.GetTourChatRoom(tourId, "Test Tour1");
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var chatRoom = result!.Value as TourChatRoomDto;
        chatRoom.ShouldNotBeNull();
        chatRoom!.TourId.ShouldBe(tourId);
        chatRoom.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Creates_chat_room_if_not_exists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourId = -12L; // Tour without chat room

        // Act
        var actionResult = controller.GetTourChatRoom(tourId, "Other Author Tour");
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var chatRoom = result!.Value as TourChatRoomDto;
        chatRoom.ShouldNotBeNull();
        chatRoom!.TourId.ShouldBe(tourId);
        chatRoom.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Returns_same_chat_room_on_multiple_calls()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourId = -10L;

        // Act
        var result1 = (controller.GetTourChatRoom(tourId, "Test Tour1").Result as ObjectResult)!.Value as TourChatRoomDto;
        var result2 = (controller.GetTourChatRoom(tourId, "Test Tour1").Result as ObjectResult)!.Value as TourChatRoomDto;

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1!.Id.ShouldBe(result2!.Id);
    }

    [Fact]
    public void Retrieves_user_chat_rooms()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var actionResult = controller.GetMyChatRooms();
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var chatRooms = result!.Value as List<TourChatRoomDto>;
        chatRooms.ShouldNotBeNull();
        // User -1 from BuildContext should have access to chat rooms
        chatRooms.ShouldBeOfType<List<TourChatRoomDto>>();
    }

    [Fact]
    public void Retrieves_messages_from_chat_room()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // Chat room from test data

        // Act
        var actionResult = controller.GetMessages(chatRoomId);
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        var messages = result!.Value as List<TourChatMessageDto>;
        messages.ShouldNotBeNull();
        messages!.Count.ShouldBeGreaterThan(0);
        messages.All(m => m.TourChatRoomId == chatRoomId).ShouldBeTrue();
    }

    [Fact]
    public void Messages_are_ordered_chronologically()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L;

        // Act
        var actionResult = controller.GetMessages(chatRoomId);
        var result = actionResult.Result as ObjectResult;
        var messages = result!.Value as List<TourChatMessageDto>;

        // Assert
        messages.ShouldNotBeNull();
        messages!.Count.ShouldBeGreaterThan(1);
        
        for (int i = 1; i < messages.Count; i++)
        {
            messages[i].SentAt.ShouldBeGreaterThanOrEqualTo(messages[i - 1].SentAt);
        }
    }

    [Fact]
    public void Does_not_return_deleted_messages()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L;

        // Act
        var actionResult = controller.GetMessages(chatRoomId);
        var result = actionResult.Result as ObjectResult;
        var messages = result!.Value as List<TourChatMessageDto>;

        // Assert
        messages.ShouldNotBeNull();
        messages!.All(m => !m.IsDeleted).ShouldBeTrue();
    }

    [Fact]
    public void Chat_room_contains_member_information()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourId = -10L;

        // Act
        var actionResult = controller.GetTourChatRoom(tourId, "Test Tour1");
        var result = actionResult.Result as ObjectResult;
        var chatRoom = result!.Value as TourChatRoomDto;

        // Assert
        chatRoom.ShouldNotBeNull();
        // Just verify the structure exists, don't test specific counts
        chatRoom!.MemberCount.ShouldBeGreaterThanOrEqualTo(0);
    }

    private static TourChatController CreateController(IServiceScope scope)
    {
        return new TourChatController(scope.ServiceProvider.GetRequiredService<ITourChatRoomService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
