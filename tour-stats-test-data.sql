-- ============================================================
-- SQL SKRIPTA ZA TESTIRANJE TOURSTATS FUNKCIONALNOSTI
-- ============================================================
-- Autor: autor1234 / autor1234
-- Skripta kreira:
-- - 1 Autor korisnika (autor1234)
-- - 10 Turista korisnika (razlicitih nivoa)
-- - 2 Ture za autor1234
-- - 15 TourExecutions sa raznovrsnim podacima
-- ============================================================

-- ============================================================
-- 1. KREIRANJE KORISNIKA (STAKEHOLDERS SCHEMA)
-- ============================================================

-- Kreiranje autora autor1234
INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (78451234, 'autor1234', 'autor1234', 1, true);

-- Kreiranje Person profila za autora
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (78451234, 78451234, 'Marko', 'Marković', 'autor1234@example.com',
    'https://i.pravatar.cc/150?u=autor1234',
    'Strastveni kreator turističkih avantura sa 5+ godina iskustva',
    'Exploring the world, one tour at a time!',
    0, 1);

-- Kreiranje 10 turista sa razlicitim nivoima (Level: 1=Beginner, 2=Intermediate, 3=Professional)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES
    (78459001, 'turista9001@test.com', 'test123', 2, true),
    (78459002, 'turista9002@test.com', 'test123', 2, true),
    (78459003, 'turista9003@test.com', 'test123', 2, true),
    (78459004, 'turista9004@test.com', 'test123', 2, true),
    (78459005, 'turista9005@test.com', 'test123', 2, true),
    (78459006, 'turista9006@test.com', 'test123', 2, true),
    (78459007, 'turista9007@test.com', 'test123', 2, true),
    (78459008, 'turista9008@test.com', 'test123', 2, true),
    (78459009, 'turista9009@test.com', 'test123', 2, true),
    (78459010, 'turista9010@test.com', 'test123', 2, true);

-- Kreiranje Person profila za turiste
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES
    (78459001, 78459001, 'Jovana', 'Jovanović', 'turista9001@test.com', NULL, 'Početnik avanturista', 'Learning to explore!', 0, 1),
    (78459002, 78459002, 'Nikola', 'Nikolić', 'turista9002@test.com', NULL, 'Iskusni putnik', 'Travel is life', 150, 2),
    (78459003, 78459003, 'Milica', 'Milić', 'turista9003@test.com', NULL, 'Profesionalni istraživač', 'Adventure expert', 500, 3),
    (78459004, 78459004, 'Stefan', 'Stefanović', 'turista9004@test.com', NULL, 'Omiljena hobija je putovanje', 'Wanderlust', 50, 1),
    (78459005, 78459005, 'Ana', 'Anić', 'turista9005@test.com', NULL, 'Fotografkinja na putovanjima', 'Capture moments', 200, 2),
    (78459006, 78459006, 'Luka', 'Lukić', 'turista9006@test.com', NULL, 'Početnik u turističkim avanturama', 'Just started', 10, 1),
    (78459007, 78459007, 'Marija', 'Marković', 'turista9007@test.com', NULL, 'Turističk guide', 'Know every corner', 450, 3),
    (78459008, 78459008, 'Petar', 'Petrović', 'turista9008@test.com', NULL, 'Weekend warrior', 'Short trips, big memories', 120, 2),
    (78459009, 78459009, 'Jelena', 'Jelenković', 'turista9009@test.com', NULL, 'Solo traveler', 'Alone but never lonely', 300, 3),
    (78459010, 78459010, 'Dusan', 'Dušanović', 'turista9010@test.com', NULL, 'Nature lover', 'Mountains are calling', 80, 1);

-- ============================================================
-- 2. KREIRANJE TURA (TOURS SCHEMA)
-- ============================================================

-- TURA 1: Visok completion rate (80% completed)
-- Naziv: "Beogradska kulturna tura"
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452001,
    'Beogradska kulturna tura',
    'Istraživanje najpoznatijih kulturnih znamenitosti Beograda. Tura obuhvata posetu Kalemegdanu, Skadarliji, Svetom Savi i još mnogo toga. Idealna za upoznavanje sa bogatom istorijom srpske prestonice.',
    2,  -- Difficulty: 2 (Medium)
    1,  -- Status: 1 (Published)
    1500.00,
    78451234,  -- AuthorId = autor1234
    8.5,
    '2024-12-01 10:00:00'
);

-- TURA 2: Nizak completion rate (30% completed, 70% abandoned)
-- Naziv: "Ekstremna planinska avantura"
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452002,
    'Ekstremna planinska avantura',
    'Zahtevna planinska tura preko Kopaonika sa uspinjanjem na vrhove visine preko 2000m. Predviđena je za iskusne planinare sa odličnom fizičkom kondicijom. Tura uključuje noćenje u planini i prelazak težih terena.',
    3,  -- Difficulty: 3 (Hard)
    1,  -- Status: 1 (Published)
    4500.00,
    78451234,  -- AuthorId = autor1234
    22.7,
    '2024-11-15 09:00:00'
);

-- ============================================================
-- 3. KREIRANJE TOURIST PREFERENCES
-- ============================================================

-- Beginner turisti (4x): turista9001, turista9004, turista9006, turista9010
INSERT INTO tours."TouristPreferences" ("Id", "PersonId", "Difficulty")
VALUES
    (784591001, 78459001, 'Beginner'),
    (784591004, 78459004, 'Beginner'),
    (784591006, 78459006, 'Beginner'),
    (784591010, 78459010, 'Beginner');

-- Intermediate turisti (3x): turista9002, turista9005, turista9008
INSERT INTO tours."TouristPreferences" ("Id", "PersonId", "Difficulty")
VALUES
    (784591002, 78459002, 'Intermediate'),
    (784591005, 78459005, 'Intermediate'),
    (784591008, 78459008, 'Intermediate');

-- Professional turisti (3x): turista9003, turista9007, turista9009
INSERT INTO tours."TouristPreferences" ("Id", "PersonId", "Difficulty")
VALUES
    (784591003, 78459003, 'Professional'),
    (784591007, 78459007, 'Professional'),
    (784591009, 78459009, 'Professional');

-- DISTRIBUCIJA:
-- - Tura 1 (Beogradska): 4 Beginner, 3 Intermediate, 3 Professional
-- - Tura 2 (Planinska): 4 Beginner (većina odustala), 1 Professional (završio)
-- NAJČEŠĆI TIP:
-- - Tura 1: Beginner (4/10 = 40%)
-- - Tura 2: Beginner (4/5 = 80% - iako su većinom odustali)

-- ============================================================
-- 4. KREIRANJE TOUR EXECUTIONS
-- ============================================================

-- -------------------------------------------------------------
-- TURA 1 (78452001): "Beogradska kulturna tura"
-- Completion Rate: 80% (8 Completed, 2 Abandoned, 0 InProgress)
-- Average Completion Percentage: ~75%
-- -------------------------------------------------------------

-- Completed executions (8x)
-- Duration ranges: 2h-4h (120-240 min)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    -- Turista 1 - Completed, 100% - Duration: 3h 15min
    (78453001, 78452001, 20.4489, 44.7866, 78459001, 100.0, 1, '2025-01-10 15:30:00', '2025-01-10 12:15:00'),

    -- Turista 2 - Completed, 100% - Duration: 2h 45min
    (78453002, 78452001, 20.4512, 44.7890, 78459002, 100.0, 1, '2025-01-11 16:45:00', '2025-01-11 14:00:00'),

    -- Turista 3 - Completed, 95% - Duration: 2h 20min
    (78453003, 78452001, 20.4501, 44.7877, 78459003, 95.0, 1, '2025-01-12 14:20:00', '2025-01-12 12:00:00'),

    -- Turista 4 - Completed, 85% - Duration: 3h 10min
    (78453004, 78452001, 20.4495, 44.7881, 78459004, 85.0, 1, '2025-01-13 17:10:00', '2025-01-13 14:00:00'),

    -- Turista 5 - Completed, 70% - Duration: 4h 0min
    (78453005, 78452001, 20.4488, 44.7869, 78459005, 70.0, 1, '2025-01-14 13:50:00', '2025-01-14 09:50:00'),

    -- Turista 6 - Completed, 65% - Duration: 2h 30min
    (78453006, 78452001, 20.4505, 44.7885, 78459006, 65.0, 1, '2025-01-15 12:30:00', '2025-01-15 10:00:00'),

    -- Turista 7 - Completed, 60% - Duration: 3h 45min
    (78453007, 78452001, 20.4490, 44.7872, 78459007, 60.0, 1, '2025-01-16 11:15:00', '2025-01-16 07:30:00'),

    -- Turista 8 - Completed, 55% - Duration: 2h 50min
    (78453008, 78452001, 20.4510, 44.7893, 78459008, 55.0, 1, '2025-01-17 10:00:00', '2025-01-17 07:10:00');

-- Abandoned executions (2x)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    -- Turista 9 - Abandoned, 35% - Duration: 1h 20min
    (78453009, 78452001, 20.4497, 44.7874, 78459009, 35.0, 2, '2025-01-18 09:20:00', '2025-01-18 08:00:00'),

    -- Turista 10 - Abandoned, 20% - Duration: 1h 45min
    (78453010, 78452001, 20.4492, 44.7870, 78459010, 20.0, 2, '2025-01-19 08:45:00', '2025-01-19 07:00:00');

-- OČEKIVANE STATISTIKE ZA TURU 1:
-- Completion Rate: 8 / (8 + 2) * 100 = 80%
-- Average Completion Percentage: (100+100+95+85+70+65+60+55+35+20) / 10 = 68.5%
-- Average Duration: (195+165+140+190+240+150+225+170+80+105) / 10 = 166 min = 2h 46min

-- -------------------------------------------------------------
-- TURA 2 (78452002): "Ekstremna planinska avantura"
-- Completion Rate: 20% (1 Completed, 4 Abandoned, 0 InProgress)
-- Average Completion Percentage: ~35%
-- -------------------------------------------------------------

-- Completed execution (1x)
-- Professional turista - Duration: 8h 30min (dugačka planinska tura)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    -- Turista 3 (Professional) - Completed, 100% - Duration: 8h 30min
    (78453011, 78452002, 20.8167, 43.2975, 78459003, 100.0, 1, '2025-01-08 18:30:00', '2025-01-08 10:00:00');

-- Abandoned executions (4x)
-- Beginners odustali nakon kraćeg vremena (1h-3h)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    -- Turista 1 (Beginner) - Abandoned, 15% (teška tura za početnike) - Duration: 1h 30min
    (78453012, 78452002, 20.8145, 43.2960, 78459001, 15.0, 2, '2024-12-20 11:30:00', '2024-12-20 10:00:00'),

    -- Turista 4 (Beginner) - Abandoned, 25% - Duration: 2h 10min
    (78453013, 78452002, 20.8152, 43.2968, 78459004, 25.0, 2, '2024-12-25 14:10:00', '2024-12-25 12:00:00'),

    -- Turista 6 (Beginner) - Abandoned, 30% - Duration: 2h 45min
    (78453014, 78452002, 20.8160, 43.2972, 78459006, 30.0, 2, '2025-01-02 10:45:00', '2025-01-02 08:00:00'),

    -- Turista 10 (Beginner) - Abandoned, 50% - Duration: 4h 20min
    (78453015, 78452002, 20.8165, 43.2973, 78459010, 50.0, 2, '2025-01-05 16:20:00', '2025-01-05 12:00:00');

-- OČEKIVANE STATISTIKE ZA TURU 2:
-- Completion Rate: 1 / (1 + 4) * 100 = 20%
-- Average Completion Percentage: (100 + 15 + 25 + 30 + 50) / 5 = 44%
-- Average Duration: (510+90+130+165+260) / 5 = 231 min = 3h 51min

-- ============================================================
-- 5. DODAVANJE IN-PROGRESS EXECUTIONS (NE UTIČU NA STATISTIKU)
-- ============================================================

-- Ovi TourExecutions su trenutno aktivni i NE ULAZE u statistiku
-- jer se racunaju samo Completed i Abandoned

INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    -- Turista 2 na Turi 2 - In Progress, 40% - Duration: 3h 0min (trenutno)
    (78453016, 78452002, 20.8155, 43.2970, 78459002, 40.0, 0, '2025-01-20 12:00:00', '2025-01-20 09:00:00'),

    -- Turista 5 na Turi 2 - In Progress, 65% - Duration: 5h 30min (trenutno)
    (78453017, 78452002, 20.8163, 43.2974, 78459005, 65.0, 0, '2025-01-20 14:30:00', '2025-01-20 09:00:00');

-- ============================================================
-- 6. KREIRANJE TOUR RATINGS (RECENZIJE)
-- ============================================================

-- -------------------------------------------------------------
-- TURA 1 (78452001): "Beogradska kulturna tura" - 2 Recenzije
-- -------------------------------------------------------------

-- Recenzija 1: Odlična tura, 5 zvezda
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (
    78454001,
    78452001,
    78459002,  -- Turista 2 (Intermediate level)
    5,
    'Fantastična tura! Posetili smo sve najvažnije kulturne znamenitosti Beograda. Vodič je bio izuzetno informativan i zabavan. Preporučujem svima koji žele da upoznaju bogatu istoriju srpske prestonice. Kalemegdan i Skadarlija su bili vrhunac iskustva!',
    '2025-01-11 18:00:00',
    100.0  -- Završio je 100% ture
);

-- Recenzija 2: Dobra tura sa malim nedostacima, 4 zvezde
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (
    78454002,
    78452001,
    78459005,  -- Turista 5 (Intermediate level)
    4,
    'Vrlo lepa tura sa dobro organizovanim rutama. Jedini minus je što je bilo dosta hodanja, pa preporučujem udobnu obuću. Ukupno sam presao 70% ture zbog vremenskih uslova, ali ono što sam video je bilo vredno!',
    '2025-01-14 20:30:00',
    70.0  -- Završio je 70% ture
);

-- -------------------------------------------------------------
-- TURA 2 (78452002): "Ekstremna planinska avantura" - 3 Recenzije
-- -------------------------------------------------------------

-- Recenzija 1: Odlična za profesionalce, 5 zvezda
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (
    78454003,
    78452002,
    78459003,  -- Turista 3 (Professional level - jedini koji je završio)
    5,
    'Izuzetna avantura za iskusne planinare! Pejzaži su bili spektakularni, a uspinjanje na vrhove preko 2000m visine je bilo pravi izazov. Odlična organizacija i oprema. Preporučujem samo za ljude u odličnoj fizičkoj kondiciji. Noćenje pod zvezdama je bilo nezaboravno iskustvo!',
    '2025-01-09 09:00:00',
    100.0  -- Završio je 100% ture kao profesionalac
);

-- Recenzija 2: Teško za početnike, 2 zvezde
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (
    78454004,
    78452002,
    78459001,  -- Turista 1 (Beginner level - odustao)
    2,
    'Tura je bila previše zahtevna za moj nivo. Morao sam da odustanem nakon samo 15% jer nisam bio fizički spreman za ovakvo naporno uspinjanje. Organizacija je bila dobra, ali definitivno treba bolje naglasiti da ova tura nije za početnike.',
    '2024-12-20 15:00:00',
    15.0  -- Odustao vrlo rano
);

-- Recenzija 3: Srednja ocena zbog fizičke težine, 3 zvezde
INSERT INTO tours."TourRatings"(
    "Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES (
    78454005,
    78452002,
    78459010,  -- Turista 10 (Beginner level - odustao)
    3,
    'Pokušao sam da završim turu ali je bila previše naporna. Stigao sam do 50% i morao da odustanem. Pejzaži su prelepi i vodič je bio profesionalan, ali tura zaista zahteva odličnu kondiciju. Možda bih je pokušao ponovo nakon bolje pripreme.',
    '2025-01-05 19:30:00',
    50.0  -- Stigao do polovine
);

-- ============================================================
-- FINALNI REZIME TEST PODATAKA
-- ============================================================
--
-- KORISNICI:
-- - 1 Autor: autor1234 (ID: 78451234, Pass: autor1234)
-- - 10 Turista: turista9001-turista9010 (IDs: 78459001-78459010)
--
-- TURE:
-- - Tura 1 (ID: 78452001): "Beogradska kulturna tura" - Medium difficulty
--   * 8 Completed (100%, 100%, 95%, 85%, 70%, 65%, 60%, 55%)
--   * 2 Abandoned (35%, 20%)
--   * 2 Recenzije (5★ i 4★)
--   * Completion Rate: 80%
--   * Avg Completion: 68.5%
--
-- - Tura 2 (ID: 78452002): "Ekstremna planinska avantura" - Hard difficulty
--   * 1 Completed (100%)
--   * 4 Abandoned (15%, 25%, 30%, 50%)
--   * 2 In Progress (40%, 65%) - NE UTIČU NA STATISTIKU
--   * 3 Recenzije (5★, 2★, 3★)
--   * Completion Rate: 20%
--   * Avg Completion: 44%
--
-- RECENZIJE:
-- - Tura 1: 2 recenzije (prosek: 4.5★)
-- - Tura 2: 3 recenzije (prosek: 3.33★)
--
-- TESTIRANJE:
-- 1. Login: autor1234 / autor1234
-- 2. GET /api/author/tour/78452001/stats
--    Očekuje: CompletionRate=80%, AvgCompletion=68.5%, AvgDuration=2h 46min
-- 3. GET /api/author/tour/78452002/stats
--    Očekuje: CompletionRate=20%, AvgCompletion=44%, AvgDuration=3h 51min
-- ============================================================
