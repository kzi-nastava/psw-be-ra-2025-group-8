INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-1, -1, -21, 5, 'Odlična tura kroz Novi Sad! Petrovaradin je prelep.', '2024-01-15 18:30:00', 100.0);

-- TourRating za Tour -11 (Test Tour2 - Beograd)
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-2, -1, -21, 4, 'Beograd je uvek zanimljiv, ali malo previše gužve.', '2024-01-16 20:15:00', 75.5);

-- TourRating za Tour -10 (Test Tour1 - drugi turista)
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-3, -2, -22, 3, 'Solidan obilazak, ali moglo je biti bolje organizovano.', '2024-01-17 15:45:00', 50.0);

-- TourRating za Tour -12 (Other Author Tour - Kopaonik)
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-4, -2, -22, 5, 'Kopaonik je prelepo zimsko odredište!', '2024-01-18 21:00:00', 100.0);

-- TourRating za Tour -11 (Test Tour2 - Beograd, drugi turista)
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-5, -3, -23, 2, 'Nisam stigao da završim turu, razočaran sam.', '2024-01-19 10:30:00', 25.0);

-- TourRating za Tour -10 (Test Tour1 - bez komentara)
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (-6, -3, -23, 4, NULL, '2024-01-20 12:00:00', 90.0);