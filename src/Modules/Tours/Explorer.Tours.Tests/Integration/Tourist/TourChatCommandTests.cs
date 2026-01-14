using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourChatCommandTests : BaseToursIntegrationTest
{
    public TourChatCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Sends_message_to_chat_room()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // Chat room from test data where user -1 is member

        var content = "Test message from unit test";

        // Act
        var result = controller.SendMessage(chatRoomId, content);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public void Fails_to_send_empty_message()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L;

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.SendMessage(chatRoomId, ""));
    }

    [Fact]
    public void Fails_to_send_whitespace_message()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L;

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.SendMessage(chatRoomId, "   "));
    }

    [Fact]
    public void Fails_to_send_null_message()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L;

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.SendMessage(chatRoomId, null));
    }

    [Fact]
    public void Creates_chat_room_on_first_access()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var tourId = -3L; // Tour without chat room
        var tourName = "Tour 3 for Issue Testing";

        // Act
        var actionResult = controller.GetTourChatRoom(tourId, tourName);
        var result = actionResult.Result as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result!.StatusCode.ShouldBe(200);
    }

    [Fact]
    public void Message_appears_in_chat_room()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // User -1 is member here
        var content = "Verification test message";

        // Act
        controller.SendMessage(chatRoomId, content);
        
        var actionResult = controller.GetMessages(chatRoomId);
        var messagesResult = actionResult.Result as ObjectResult;
        var messages = messagesResult!.Value as List<Explorer.Tours.API.Dtos.TourChatMessageDto>;

        // Assert
        messages.ShouldNotBeNull();
        messages!.Any(m => m.Content == content).ShouldBeTrue();
    }

    [Fact]
    public void Multiple_messages_can_be_sent()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // User -1 is member here

        // Act
        controller.SendMessage(chatRoomId, "First message");
        controller.SendMessage(chatRoomId, "Second message");
        controller.SendMessage(chatRoomId, "Third message");

        var actionResult = controller.GetMessages(chatRoomId);
        var messagesResult = actionResult.Result as ObjectResult;
        var messages = messagesResult!.Value as List<Explorer.Tours.API.Dtos.TourChatMessageDto>;

        // Assert
        messages.ShouldNotBeNull();
        messages!.Count(m => m.Content.StartsWith("First") || 
                             m.Content.StartsWith("Second") || 
                             m.Content.StartsWith("Third")).ShouldBe(3);
    }

    [Fact]
    public void Long_message_can_be_sent()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // User -1 is member here
        var longContent = new string('A', 500); // 500 characters

        // Act
        var result = controller.SendMessage(chatRoomId, longContent);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public void Chat_room_can_be_accessed_by_multiple_users()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var tourId = -10L;
        
        // User -21 (from test data, member of chat room)
        var controller1 = new TourChatController(scope.ServiceProvider.GetRequiredService<ITourChatRoomService>())
        {
            ControllerContext = BuildContext("-21")
        };
        
        // User -23 (from test data, member of chat room)
        var controller2 = new TourChatController(scope.ServiceProvider.GetRequiredService<ITourChatRoomService>())
        {
            ControllerContext = BuildContext("-23")
        };

        // Act
        var result1 = controller1.GetTourChatRoom(tourId, "Test Tour1");
        var result2 = controller2.GetTourChatRoom(tourId, "Test Tour1");

        // Assert
        var chatRoom1 = (result1.Result as ObjectResult)!.Value as Explorer.Tours.API.Dtos.TourChatRoomDto;
        var chatRoom2 = (result2.Result as ObjectResult)!.Value as Explorer.Tours.API.Dtos.TourChatRoomDto;
        
        chatRoom1.ShouldNotBeNull();
        chatRoom2.ShouldNotBeNull();
        chatRoom1!.Id.ShouldBe(chatRoom2!.Id);
    }

    [Fact]
    public void Message_with_special_characters_can_be_sent()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var chatRoomId = -1L; // User -1 is member here
        var content = "Hello! Test: @user #tag $price";

        // Act
        var result = controller.SendMessage(chatRoomId, content);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<OkObjectResult>();
    }

    private static TourChatController CreateController(IServiceScope scope)
    {
        return new TourChatController(scope.ServiceProvider.GetRequiredService<ITourChatRoomService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
