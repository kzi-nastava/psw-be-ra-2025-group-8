-- KeyPointsReached za TourExecution -1 (Tourist 1, Tour -10)
-- Turista je dostigao prva 2 keypointa
INSERT INTO tours."KeyPointsReached" ("Id", "TourExecutionId", "KeyPointOrder", "ReachedAt", "Latitude", "Longitude")
VALUES 
    (-1, -1, 1, '2024-01-15 10:35:00', 45.2671, 19.8335),
    (-2, -1, 2, '2024-01-15 11:15:00', 45.2551, 19.8451);

-- KeyPointsReached za TourExecution -3 (Tourist 3, Tour -10)
-- Turista je dostigao samo prvi keypoint
INSERT INTO tours."KeyPointsReached" ("Id", "TourExecutionId", "KeyPointOrder", "ReachedAt", "Latitude", "Longitude")
VALUES 
    (-3, -3, 1, '2024-01-17 09:20:00', 45.2671, 19.8335);

-- KeyPointsReached za TourExecution -2 (Tourist 2, Tour -11)
-- Turista je dostigao sva 3 keypointa (tura završena)
INSERT INTO tours."KeyPointsReached" ("Id", "TourExecutionId", "KeyPointOrder", "ReachedAt", "Latitude", "Longitude")
VALUES 
    (-4, -2, 1, '2024-01-16 14:50:00', 44.8176, 20.4633),
    (-5, -2, 2, '2024-01-16 15:30:00', 44.8125, 20.4612),
    (-6, -2, 3, '2024-01-16 16:20:00', 44.8048, 20.4781);

-- TourExecution -4 nema dostignutih keypointa (tek počinje)