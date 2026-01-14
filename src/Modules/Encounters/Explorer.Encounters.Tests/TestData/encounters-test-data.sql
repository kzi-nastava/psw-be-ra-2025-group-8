INSERT INTO encounters."Encounters"
("Id", "Name", "Description", "Location", "Latitude", "Longitude", "Status", "Type", "XPReward", "PublishedAt", "ArchivedAt")
VALUES
(-1, 'Draft encounter', 'Draft description', 'Novi Sad', 45.2671, 19.8335, 0, 0, 50, NULL, NULL),

(-2, 'Published social encounter', 'Social challenge', 'Belgrade', 44.7866, 20.4489, 1, 0, 100, NOW(), NULL),

(-3, 'Published location encounter', 'Visit location', 'Niš', 43.3209, 21.8958, 1, 1, 120, NOW(), NULL),

(-4, 'Archived encounter', 'Old challenge', 'Subotica', 46.1005, 19.6653, 2, 2, 80, NOW(), NOW());

INSERT INTO encounters."EncounterParticipations"
("Id", "PersonId", "EncounterId", "Status", "ActivatedAt", "CompletedAt", "XPAwarded", "StartTimeInRange")
VALUES
(-1, 1, -3, 0, NOW(), NULL, NULL, NULL), -- Turista 1 je aktivirao Niski izazov
(-2, 2, -3, 0, NOW(), NULL, NULL, NULL); -- Turista 2 je aktivirao Niski izazov

UPDATE encounters."Encounters" 
SET "ImageLatitude" = 43.3209, "ImageLongitude" = 21.8958, "ActivationRangeMeters" = 5 
WHERE "Id" = -3;