# Tour Chat Tests

## Test Classes

### TourChatQueryTests
Tests for retrieving chat room and message data.

**Tests:**
- `Retrieves_chat_room_for_tour` - Gets existing chat room
- `Creates_chat_room_if_not_exists` - Creates new chat room if not present
- `Returns_same_chat_room_on_multiple_calls` - Verifies idempotency
- `Retrieves_user_chat_rooms` - Gets all chat rooms for user
- `Retrieves_messages_from_chat_room` - Gets messages from specific room
- `Messages_are_ordered_chronologically` - Verifies message ordering
- `Does_not_return_deleted_messages` - Ensures deleted messages are filtered
- `Chat_room_contains_member_information` - Verifies member count

### TourChatCommandTests
Tests for sending messages and creating chat rooms.

**Tests:**
- `Sends_message_to_chat_room` - Successfully sends message
- `Fails_to_send_empty_message` - Validation test
- `Fails_to_send_whitespace_message` - Validation test
- `Fails_to_send_null_message` - Validation test
- `Creates_chat_room_on_first_access` - Auto-creation test
- `Message_appears_in_chat_room` - Verifies message persistence
- `Multiple_messages_can_be_sent` - Tests multiple sends
- `Long_message_can_be_sent` - Tests long content
- `Chat_room_can_be_accessed_by_multiple_users` - Multi-user test
- `Message_with_special_characters_can_be_sent` - Special chars test

## Test Data

### Files
- `c-tours.sql` - Base tour data
- `d-tourexecutions.sql` - Tour executions
- `e-tour-chat.sql` - Chat rooms, members, and messages

### Test Data Structure

**Chat Rooms:**
- ID: -1, TourId: -10 (Test Tour1)
- ID: -2, TourId: -11 (Test Tour2)

**Chat Members:**
- ChatRoom -1: Users -21, -23
- ChatRoom -2: User -22

**Messages:**
- ChatRoom -1: 3 messages
- ChatRoom -2: 1 message

## Running Tests

```bash
# All tour chat tests
dotnet test --filter "FullyQualifiedName~TourChat"

# Query tests only
dotnet test --filter "FullyQualifiedName~TourChatQueryTests"

# Command tests only
dotnet test --filter "FullyQualifiedName~TourChatCommandTests"
```

## Notes

- Test data uses negative IDs to avoid conflicts
- Chat rooms are linked to existing tours from test data
- Members correspond to tourists from tour executions
- All tests follow the same pattern as other integration tests in the project
