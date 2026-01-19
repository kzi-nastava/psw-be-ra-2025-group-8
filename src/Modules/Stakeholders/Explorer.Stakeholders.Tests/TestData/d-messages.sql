INSERT INTO stakeholders."Messages"
    ("Id", "SenderId", "RecipientId", "Content",
     "TimestampCreated", "TimestampUpdated", "IsDeleted", "AttachmentType", "AttachmentId")
VALUES
    (-1, 1, 2, 'Test poruka 1', NOW(), NOW(), FALSE, NULL, NULL),
    (-2, 2, 1, 'Test poruka 2', NOW(), NOW(), FALSE, NULL, NULL),
    (-3, -11, 1, 'Evo kupona za tebe!', NOW(), NOW(), FALSE, 'Coupon', -1);
