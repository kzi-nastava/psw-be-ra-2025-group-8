-- TourExecution za Tour -10 (Test Tour1 - Novi Sad)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "Status", "LastActivity")
VALUES (-1, -10, 19.8335, 45.2671, 1, 0, '2024-01-15 10:30:00');

-- TourExecution za Tour -11 (Test Tour2 - Beograd)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "Status", "LastActivity")
VALUES (-2, -11, 20.4489, 44.7866, 2, 1, '2024-01-16 14:45:00');

-- TourExecution za Tour -10 (Test Tour1 - drugi turista)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "Status", "LastActivity")
VALUES (-3, -10, 19.8400, 45.2700, 3, 0, '2024-01-17 09:15:00');

-- TourExecution za Tour -12 (Other Author Tour - Kopaonik)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "Status", "LastActivity")
VALUES (-4, -12, 19.8500, 45.2800, 1, 1, '2024-01-18 16:20:00');