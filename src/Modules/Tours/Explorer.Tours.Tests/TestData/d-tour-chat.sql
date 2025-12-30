-- ==============================================
-- TOUR CHAT TEST DATA
-- ==============================================

-- Chat room za turu koja ve? ima tour executions
WITH active_tour AS (
    SELECT "Id", "Name" 
    FROM tours."Tours" 
    WHERE "Status" = 1 
    ORDER BY "Id" DESC 
    LIMIT 1
)
INSERT INTO tours."TourChatRooms" ("TourId", "Name", "Description", "CreatedAt", "IsActive")
SELECT 
    "Id",
    'Tour Chat: ' || "Name",
    'Chat room for tourists on tour #' || "Id",
    NOW() - INTERVAL '30 minutes',
    true
FROM active_tour;

-- Dodaj ?lanove (turiste koji su startovali turu)
WITH chat_room AS (
    SELECT "Id", "TourId"
    FROM tours."TourChatRooms"
    ORDER BY "Id" DESC
    LIMIT 1
),
active_executions AS (
    SELECT DISTINCT te."IdTourist"
    FROM tours."TourExecutions" te
    INNER JOIN chat_room cr ON te."IdTour" = cr."TourId"
    WHERE te."Status" = 0 -- InProgress
    LIMIT 5
)
INSERT INTO tours."TourChatMembers" ("TourChatRoomId", "UserId", "JoinedAt", "LeftAt", "IsActive", "LastReadAt")
SELECT 
    cr."Id",
    ae."IdTourist",
    NOW() - INTERVAL '25 minutes',
    NULL,
    true,
    NOW() - INTERVAL '5 minutes'
FROM chat_room cr
CROSS JOIN active_executions ae;

-- Dodaj test poruke
WITH chat_room AS (
    SELECT "Id"
    FROM tours."TourChatRooms"
    ORDER BY "Id" DESC
    LIMIT 1
),
members AS (
    SELECT "UserId", ROW_NUMBER() OVER (ORDER BY "JoinedAt") as rn
    FROM tours."TourChatMembers"
    WHERE "TourChatRoomId" = (SELECT "Id" FROM chat_room)
    LIMIT 3
)
INSERT INTO tours."TourChatMessages" ("TourChatRoomId", "SenderId", "Content", "SentAt", "EditedAt", "IsDeleted")
SELECT 
    cr."Id",
    m."UserId",
    msg.content,
    NOW() - (msg.minutes_ago || ' minutes')::INTERVAL,
    NULL,
    false
FROM (
    VALUES 
        (1, 'Zdravo svima! Ko je po?eo ovu turu?', 20),
        (2, 'Ja sam upravo startovao. Odli?an dan za turu!', 18),
        (3, 'Mogu da vam pomognem ako se izgubite ??', 15),
        (1, 'Hvala! Baš je lepo vreme', 12),
        (2, 'Da li neko zna gde je slede?a ta?ka?', 10)
) AS msg(member_rn, content, minutes_ago)
INNER JOIN members m ON m.rn = msg.member_rn
CROSS JOIN chat_room cr;

-- Provera
SELECT 
    cr."Id" as "ChatRoomId",
    cr."Name" as "ChatRoom",
    COUNT(DISTINCT cm."UserId") as "MemberCount",
    COUNT(msg."Id") as "MessageCount"
FROM tours."TourChatRooms" cr
LEFT JOIN tours."TourChatMembers" cm ON cr."Id" = cm."TourChatRoomId" AND cm."IsActive" = true
LEFT JOIN tours."TourChatMessages" msg ON cr."Id" = msg."TourChatRoomId" AND msg."IsDeleted" = false
GROUP BY cr."Id", cr."Name";
