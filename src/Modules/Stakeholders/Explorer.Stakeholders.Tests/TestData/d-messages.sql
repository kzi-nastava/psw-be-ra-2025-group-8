INSERT INTO stakeholders."Messages"
    ("Id", "SenderId", "RecipientId", "Content",
     "TimestampCreated", "TimestampUpdated", "IsDeleted")
VALUES
    (-1, 1, 2, 'Test poruka 1', NOW(), NOW(), FALSE),
    (-2, 2, 1, 'Test poruka 2', NOW(), NOW(), FALSE);
