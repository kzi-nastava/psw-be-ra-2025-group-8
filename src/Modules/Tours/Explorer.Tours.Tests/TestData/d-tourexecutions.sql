-- TourExecution za Tour -10 (Test Tour1 - Novi Sad)
-- Duration: 2 hours 45 minutes (165 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity","CreatedAt")
VALUES (-1, -10, 19.8335, 45.2671, -21, 33.5, 0, '2024-01-15 13:15:00','2024-01-15 10:30:00');

-- TourExecution za Tour -11 (Test Tour2 - Beograd)
-- Duration: 1 hour 30 minutes (90 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-2, -11, 20.4489, 44.7866, -22, 100.0, 1, '2024-01-16 16:15:00','2024-01-16 14:45:00');

-- TourExecution za Tour -10 (Test Tour1 - drugi turista)
-- Duration: 3 hours 20 minutes (200 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-3, -10, 19.8400, 45.2700, -23, 0.0, 0, '2024-01-17 12:35:00','2024-01-17 09:15:00');

-- TourExecution za Tour -12 (Other Author Tour - Kopaonik)
-- Duration: 4 hours 15 minutes (255 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-4, -12, 19.8500, 45.2800, -21, 66.7, 2, '2024-01-18 20:35:00', '2024-01-18 16:20:00');

-- TourExecution za Tour -10 - Completed execution
-- Duration: 2 hours (120 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-5, -10, 19.8350, 45.2680, -22, 100.0, 1, '2024-01-19 12:00:00', '2024-01-19 10:00:00');

-- TourExecution za Tour -10 - Abandoned execution
-- Duration: 1 hour 30 minutes (90 minutes)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-6, -10, 19.8360, 45.2690, -21, 50.0, 2, '2024-01-20 11:30:00', '2024-01-20 10:00:00');

-- TourExecution za Tour -10 - Very long execution (15 hours - should be excluded from average duration)
-- Duration: 15 hours (900 minutes) - exceeds 12 hour limit
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES (-7, -10, 19.8370, 45.2700, -23, 100.0, 1, '2024-01-21 23:00:00', '2024-01-21 08:00:00');
