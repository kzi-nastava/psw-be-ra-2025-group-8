-- ==============================================
-- TOUR CHAT TEST DATA
-- ==============================================

-- Chat room za Tour -10 (koji ima tour executions)
INSERT INTO tours."TourChatRooms" ("Id", "TourId", "Name", "Description", "CreatedAt", "IsActive")
VALUES 
(-1, -10, 'Chat: Test Tour1', 'Chat room for Test Tour1', '2024-01-15 10:00:00+00', true);

-- Chat room za Tour -11
INSERT INTO tours."TourChatRooms" ("Id", "TourId", "Name", "Description", "CreatedAt", "IsActive")
VALUES 
(-2, -11, 'Chat: Test Tour2', 'Chat room for Test Tour2', '2024-01-16 14:00:00+00', true);

-- Add members to chat room -1 (including test user -1 from BuildContext)
INSERT INTO tours."TourChatMembers" ("Id", "TourChatRoomId", "UserId", "JoinedAt", "LeftAt", "IsActive", "LastReadAt")
VALUES 
(-1, -1, -1, '2024-01-15 10:00:00+00', NULL, true, '2024-01-15 11:00:00+00'),
(-2, -1, -21, '2024-01-15 10:30:00+00', NULL, true, '2024-01-15 11:00:00+00'),
(-3, -1, -23, '2024-01-17 09:15:00+00', NULL, true, '2024-01-17 10:00:00+00');

-- Add members to chat room -2 (including test user -1)
INSERT INTO tours."TourChatMembers" ("Id", "TourChatRoomId", "UserId", "JoinedAt", "LeftAt", "IsActive", "LastReadAt")
VALUES 
(-4, -2, -1, '2024-01-16 14:00:00+00', NULL, true, '2024-01-16 15:00:00+00'),
(-5, -2, -22, '2024-01-16 14:45:00+00', NULL, true, '2024-01-16 15:00:00+00');

-- Add test messages to chat room -1
INSERT INTO tours."TourChatMessages" ("Id", "TourChatRoomId", "SenderId", "Content", "SentAt", "EditedAt", "IsDeleted")
VALUES 
(-1, -1, -21, 'Pozdrav! Upravo sam po?eo turu', '2024-01-15 10:35:00+00', NULL, false),
(-2, -1, -23, 'Cao! Ja sam tek stigao', '2024-01-17 09:20:00+00', NULL, false),
(-3, -1, -21, 'Super! Gde si sada?', '2024-01-17 09:25:00+00', NULL, false);

-- Add test messages to chat room -2
INSERT INTO tours."TourChatMessages" ("Id", "TourChatRoomId", "SenderId", "Content", "SentAt", "EditedAt", "IsDeleted")
VALUES 
(-4, -2, -22, 'Završio sam turu! Bilo je odli?no!', '2024-01-16 15:00:00+00', NULL, false);
