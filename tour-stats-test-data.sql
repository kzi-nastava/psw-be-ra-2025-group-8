-- ============================================================
-- SQL SKRIPTA ZA TESTIRANJE TOURSTATS I RECOMMENDATIONS FUNKCIONALNOSTI
-- ============================================================
-- Autor: autor1234 / autor1234
-- Skripta kreira:
-- - 1 Autor korisnika (autor1234)
-- - 10 Turista korisnika (razlicitih nivoa)
-- - 8 Tura za autor1234 (svaka generiše drugačiji tip preporuke)
-- - 50+ TourExecutions sa raznovrsnim podacima
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
-- 2. KREIRANJE TOURIST PREFERENCES
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

-- ============================================================
-- 3. KREIRANJE TURA (TOURS SCHEMA)
-- ============================================================
-- Svaka tura je dizajnirana da generiše specifičan tip preporuke

-- -------------------------------------------------------------
-- TURA 1: ODLIČNA TURA (POZITIVNA PREPORUKA - "odličan procenat završenosti")
-- Scenario: CompletionRate >85%, AvgCompletion >85%
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452001,
    'Petrovaradinska tvrđava - Zlatna tura',
    'Savršena tura po Petrovaradinskoj tvrđavi sa visokim procentom završenosti. Idealna za sve nivoe turista.',
    1,  -- Difficulty: 1 (Easy)
    1,  -- Status: 1 (Published)
    1200.00,
    78451234,
    3.5,
    '2024-12-01 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 2: KRATKA USPEŠNA TURA (POZITIVNA PREPORUKA - "idealna za turiste sa ograničenim vremenom")
-- Scenario: Duration <2h, CompletionRate >85%
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452002,
    'Brza šetnja centrom Novog Sada',
    'Kratka ali sveobuhvatna tura kroz centar grada. Perfektna za ljude sa ograničenim vremenom.',
    1,  -- Difficulty: 1 (Easy)
    1,  -- Status: 1 (Published)
    800.00,
    78451234,
    2.0,
    '2024-12-05 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 3: TURA SA VISOKIM COMPLETION RATE (POZITIVNA PREPORUKA - "možete produžiti turu")
-- Scenario: CompletionRate >85%, ali ne oba visoka
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452003,
    'Fruška gora - Manastirska ruta',
    'Popularna tura po manastirima Fruške gore. Većina turista završi turu.',
    2,  -- Difficulty: 2 (Medium)
    1,  -- Status: 1 (Published)
    2500.00,
    78451234,
    12.0,
    '2024-11-20 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 4: LJUDI RADE VEĆINU ALI NE ZAVRŠAVAJU (NEGATIVNA PREPORUKA)
-- Scenario: AvgCompletion >85%, CompletionRate <30%
-- Preporuka: "Turisti prolaze većinu ture ali je retko završavaju do kraja"
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452004,
    'Beogradska kulturna tura - Produžena verzija',
    'Detaljna tura kroz sve kulturne znamenitosti Beograda. Mnogi turisti završe 90% ali odustanu pred sam kraj.',
    2,  -- Difficulty: 2 (Medium)
    1,  -- Status: 1 (Published)
    1800.00,
    78451234,
    9.0,
    '2024-11-15 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 5: OBA NISKA (NEGATIVNA PREPORUKA - "predugačka ili preteška")
-- Scenario: AvgCompletion <30%, CompletionRate <30%
-- Preporuka: "Tura je možda predugačka ili preteška"
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452005,
    'Ekstremna planinska avantura - Kopaonik',
    'Izuzetno zahtevna planinska tura. Većina turista odustane rano zbog težine.',
    3,  -- Difficulty: 3 (Hard)
    1,  -- Status: 1 (Published)
    5000.00,
    78451234,
    25.0,
    '2024-11-10 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 6: MARATONSKA TURA (NEGATIVNA PREPORUKA - "preko 6 sati")
-- Scenario: AvgDuration > 6h (360 min)
-- Preporuka: "Razmislite o podeli ture na više kraćih tura"
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452006,
    'Celodnevna avantura - Tara i Drina',
    'Maratonska tura koja traje ceo dan. Prekrasni pejzaži ali veoma dugačka.',
    2,  -- Difficulty: 2 (Medium)
    1,  -- Status: 1 (Published)
    4500.00,
    78451234,
    30.0,
    '2024-11-05 10:00:00'
);

-- -------------------------------------------------------------
-- TURA 7: POGREŠNA PROCENA VREMENA (NEGATIVNA PREPORUKA)
-- Scenario: Stvarno trajanje > 150% od predviđenog
-- Preporuka: "Tura traje značajno duže od predviđenog"
-- TransportTime: 90 min (Walk), ali stvarno traje ~180 min
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452007,
    'Gradska tura sa potcenjenim vremenom',
    'Tura koja redovno traje duže nego što je planirano.',
    1,  -- Difficulty: 1 (Easy)
    1,  -- Status: 1 (Published)
    1000.00,
    78451234,
    5.0,
    '2024-10-25 10:00:00'
);

-- Dodaj TransportTime za turu 7 (procenjeno 90 min)
INSERT INTO tours."TourTransportTimes" ("TourId", "Transport", "DurationMinutes")
VALUES (78452007, 0, 90);  -- Walk = 0, 90 minuta

-- -------------------------------------------------------------
-- TURA 8: DUGO TRAJANJE + NIZAK PROCENAT (NEGATIVNA PREPORUKA - kombinovana)
-- Scenario: AvgDuration > 6h AND AvgCompletion < 30%
-- Preporuka: "Tura je predugačka - turisti provode mnogo vremena ali prolaze mali procenat"
-- -------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId", "LengthInKilometers", "PublishedAt")
VALUES (
    78452008,
    'Ultra maraton - Đerdap',
    'Ekstremno dugačka i zahtevna tura. Turisti provode satima ali završe malo.',
    3,  -- Difficulty: 3 (Hard)
    1,  -- Status: 1 (Published)
    6000.00,
    78451234,
    40.0,
    '2024-10-15 10:00:00'
);

-- ============================================================
-- 4. KREIRANJE TOUR EXECUTIONS
-- ============================================================

-- -------------------------------------------------------------
-- TURA 1 (78452001): Petrovaradinska tvrđava - Zlatna tura
-- POZITIVNA: Oba visoka (>85%)
-- CompletionRate: 4/4 = 100%, AvgCompletion: ~95%
-- Duration: ~2h
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453001, 78452001, 19.8617, 45.2519, 78459001, 100.0, 1, '2025-01-10 12:00:00', '2025-01-10 10:00:00'),
    (78453002, 78452001, 19.8617, 45.2519, 78459002, 95.0, 1, '2025-01-11 12:15:00', '2025-01-11 10:00:00'),
    (78453003, 78452001, 19.8617, 45.2519, 78459003, 92.0, 1, '2025-01-12 12:10:00', '2025-01-12 10:00:00'),
    (78453004, 78452001, 19.8617, 45.2519, 78459004, 90.0, 1, '2025-01-13 12:05:00', '2025-01-13 10:00:00');

-- -------------------------------------------------------------
-- TURA 2 (78452002): Brza šetnja centrom
-- POZITIVNA: Kratka (<2h) + visok uspeh (>85%)
-- CompletionRate: 4/4 = 100%, Duration: ~80 min
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453010, 78452002, 19.8425, 45.2551, 78459001, 100.0, 1, '2025-01-14 11:20:00', '2025-01-14 10:00:00'),
    (78453011, 78452002, 19.8425, 45.2551, 78459002, 100.0, 1, '2025-01-15 11:25:00', '2025-01-15 10:00:00'),
    (78453012, 78452002, 19.8425, 45.2551, 78459003, 100.0, 1, '2025-01-16 11:30:00', '2025-01-16 10:00:00'),
    (78453013, 78452002, 19.8425, 45.2551, 78459004, 100.0, 1, '2025-01-17 11:15:00', '2025-01-17 10:00:00');

-- -------------------------------------------------------------
-- TURA 3 (78452003): Fruška gora - Manastirska ruta
-- POZITIVNA: Visok completion rate (>85%)
-- CompletionRate: 5/5 = 100%, AvgCompletion: ~75%
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453020, 78452003, 19.7500, 45.1500, 78459001, 100.0, 1, '2025-01-05 15:00:00', '2025-01-05 10:00:00'),
    (78453021, 78452003, 19.7500, 45.1500, 78459002, 80.0, 1, '2025-01-06 15:30:00', '2025-01-06 10:00:00'),
    (78453022, 78452003, 19.7500, 45.1500, 78459003, 70.0, 1, '2025-01-07 14:45:00', '2025-01-07 10:00:00'),
    (78453023, 78452003, 19.7500, 45.1500, 78459004, 65.0, 1, '2025-01-08 15:15:00', '2025-01-08 10:00:00'),
    (78453024, 78452003, 19.7500, 45.1500, 78459005, 60.0, 1, '2025-01-09 14:30:00', '2025-01-09 10:00:00');

-- -------------------------------------------------------------
-- TURA 4 (78452004): Beogradska kulturna - Produžena
-- NEGATIVNA: Visok avg% (>85%) + nizak rate (<30%)
-- AvgCompletion: ~90%, CompletionRate: 1/5 = 20%
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453030, 78452004, 20.4489, 44.7866, 78459001, 92.0, 2, '2025-01-10 16:00:00', '2025-01-10 10:00:00'),
    (78453031, 78452004, 20.4489, 44.7866, 78459002, 88.0, 2, '2025-01-11 16:30:00', '2025-01-11 10:00:00'),
    (78453032, 78452004, 20.4489, 44.7866, 78459003, 95.0, 2, '2025-01-12 15:45:00', '2025-01-12 10:00:00'),
    (78453033, 78452004, 20.4489, 44.7866, 78459004, 85.0, 2, '2025-01-13 16:15:00', '2025-01-13 10:00:00'),
    (78453034, 78452004, 20.4489, 44.7866, 78459005, 100.0, 1, '2025-01-14 17:00:00', '2025-01-14 10:00:00');

-- -------------------------------------------------------------
-- TURA 5 (78452005): Ekstremna planinska - Kopaonik
-- NEGATIVNA: Oba niska (<30%)
-- AvgCompletion: ~22%, CompletionRate: 0/4 = 0%
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453040, 78452005, 20.8167, 43.2975, 78459001, 15.0, 2, '2024-12-20 12:00:00', '2024-12-20 10:00:00'),
    (78453041, 78452005, 20.8167, 43.2975, 78459004, 25.0, 2, '2024-12-22 13:00:00', '2024-12-22 10:00:00'),
    (78453042, 78452005, 20.8167, 43.2975, 78459006, 20.0, 2, '2024-12-25 12:30:00', '2024-12-25 10:00:00'),
    (78453043, 78452005, 20.8167, 43.2975, 78459010, 28.0, 2, '2024-12-28 14:00:00', '2024-12-28 10:00:00');

-- -------------------------------------------------------------
-- TURA 6 (78452006): Celodnevna avantura - Tara
-- NEGATIVNA: Trajanje > 6h
-- AvgDuration: ~7.5h (450 min), CompletionRate: 100%
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453050, 78452006, 19.5564, 43.8914, 78459003, 100.0, 1, '2025-01-02 17:30:00', '2025-01-02 10:00:00'),
    (78453051, 78452006, 19.5564, 43.8914, 78459007, 100.0, 1, '2025-01-03 17:45:00', '2025-01-03 10:00:00'),
    (78453052, 78452006, 19.5564, 43.8914, 78459009, 100.0, 1, '2025-01-04 17:15:00', '2025-01-04 10:00:00'),
    (78453053, 78452006, 19.5564, 43.8914, 78459002, 100.0, 1, '2025-01-05 18:00:00', '2025-01-05 10:00:00');

-- -------------------------------------------------------------
-- TURA 7 (78452007): Gradska tura sa potcenjenim vremenom
-- NEGATIVNA: Traje duže od predviđenog (>150%)
-- Predviđeno: 90 min, Stvarno: ~180 min
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453060, 78452007, 19.8425, 45.2551, 78459001, 100.0, 1, '2025-01-15 13:00:00', '2025-01-15 10:00:00'),
    (78453061, 78452007, 19.8425, 45.2551, 78459002, 100.0, 1, '2025-01-16 13:10:00', '2025-01-16 10:00:00'),
    (78453062, 78452007, 19.8425, 45.2551, 78459004, 100.0, 1, '2025-01-17 12:50:00', '2025-01-17 10:00:00'),
    (78453063, 78452007, 19.8425, 45.2551, 78459005, 100.0, 1, '2025-01-18 13:05:00', '2025-01-18 10:00:00');

-- -------------------------------------------------------------
-- TURA 8 (78452008): Ultra maraton - Đerdap
-- NEGATIVNA: Dugo trajanje (>6h) + nizak procenat (<30%)
-- AvgDuration: ~8h, AvgCompletion: ~25%
-- -------------------------------------------------------------
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity", "CreatedAt")
VALUES
    (78453070, 78452008, 22.0431, 44.6285, 78459001, 20.0, 2, '2024-12-10 18:00:00', '2024-12-10 10:00:00'),
    (78453071, 78452008, 22.0431, 44.6285, 78459004, 25.0, 2, '2024-12-12 18:30:00', '2024-12-12 10:00:00'),
    (78453072, 78452008, 22.0431, 44.6285, 78459006, 22.0, 2, '2024-12-15 17:45:00', '2024-12-15 10:00:00'),
    (78453073, 78452008, 22.0431, 44.6285, 78459010, 30.0, 2, '2024-12-18 18:15:00', '2024-12-18 10:00:00');

-- ============================================================
-- 5. KREIRANJE TOUR RATINGS (RECENZIJE)
-- ============================================================

-- Recenzije za Turu 1 (Petrovaradinska)
INSERT INTO tours."TourRatings"("Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES
    (78454001, 78452001, 78459001, 5, 'Savršena tura! Preporučujem svima.', '2025-01-10 13:00:00', 100.0),
    (78454002, 78452001, 78459002, 5, 'Odlična organizacija i prekrasni pejzaži.', '2025-01-11 13:00:00', 95.0);

-- Recenzije za Turu 5 (Ekstremna planinska)
INSERT INTO tours."TourRatings"("Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES
    (78454005, 78452005, 78459001, 2, 'Previše teška za početnike. Morao sam da odustanem.', '2024-12-20 13:00:00', 15.0),
    (78454006, 78452005, 78459004, 2, 'Nije za moj nivo. Preporučujem samo profesionalcima.', '2024-12-22 14:00:00', 25.0);

-- Recenzije za Turu 6 (Maratonska)
INSERT INTO tours."TourRatings"("Id", "IdTour", "IdTourist", "Rating", "Comment", "CreatedAt", "TourCompletionPercentage")
VALUES
    (78454007, 78452006, 78459003, 4, 'Prekrasna tura ali veoma dugačka. Treba ceo dan.', '2025-01-02 18:30:00', 100.0);

-- ============================================================
-- KREIRANJE KUPONA I POPUSTA (PAYMENTS SCHEMA)
-- ============================================================

-- Jedan globalni kupon (važi za sve ture autora)
INSERT INTO payments."Coupons"("Id", "Code", "DiscountPercentage", "ExpiryDate", "AuthorId", "TourId")
VALUES (-10, 'SAVE2025', 20, '2025-12-31', 78451234, NULL);

-- Jedan specifičan kupon za Turu 1
INSERT INTO payments."Coupons"("Id", "Code", "DiscountPercentage", "ExpiryDate", "AuthorId", "TourId")
VALUES (-11, 'BEOGRAD5', 15, '2025-06-01', 78451234, 78452001);

-- Kreiranje Sale-a (Popust na turu)
INSERT INTO payments."Sales"("Id", "AuthorId", "StartDate", "EndDate", "DiscountPercentage")
VALUES (-50, 78451234, '2025-01-01', '2025-01-14', 30);

INSERT INTO payments."SaleTours"("Id", "SaleId", "TourId")
VALUES (-1, -50, 78452001);

-- Prolećna akcija (April) - SaleId: -51
INSERT INTO payments."Sales"("Id", "AuthorId", "StartDate", "EndDate", "DiscountPercentage")
VALUES (-51, 78451234, '2025-04-01', '2025-04-15', 25);

INSERT INTO payments."SaleTours"("Id", "SaleId", "TourId")
VALUES (-2, -51, 78452001), (-3, -51, 78452002);

-- Letnji kupon (Jul/Avgust) - CouponId: -12
INSERT INTO payments."Coupons"("Id", "Code", "DiscountPercentage", "ExpiryDate", "AuthorId", "TourId")
VALUES (-12, 'SUMMER25', 10, '2025-08-31', 78451234, NULL);

-- ============================================================
-- PRODAJE (PurchasedItems) ZA GRAFIKON
-- ============================================================

-- Prodaja decembar: Puna cena (1500)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES (-101, 78459001, 78452001, '2024-12-10 14:00:00', 1500.00, 1500.00, 1500, NULL, NULL);

-- Prodaja decembar: Sa kuponom (-11, popust 15% -> 1275)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES (-102, 78459002, 78452001, '2024-12-20 10:00:00', 1500.00, 1275.00, 1275, NULL, -11);

-- Prodaja januar: Period Sale-a (-50, popust 30% -> 1050)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES (-103, 78459003, 78452001, '2025-01-05 09:00:00', 1500.00, 1050.00, 1050, -50, NULL);

-- Prodaja januar: Puna cena (da vidimo skok nakon završetka sale-a)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES (-104, 78459004, 78452001, '2025-01-20 18:30:00', 1500.00, 1500.00, 1500, NULL, NULL);

-- -------------------------------------------------------------
-- OKTOBAR 2024: (Puna cena i Kuponi)
-- -------------------------------------------------------------
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-201, 78459005, 78452001, '2024-10-05 10:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-202, 78459006, 78452001, '2024-10-12 11:30:00', 1500.00, 1275.00, 1275, NULL, -11), -- Kupon 15%
(-203, 78459007, 78452002, '2024-10-20 09:15:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-204, 78459008, 78452001, '2024-10-28 16:45:00', 1500.00, 1200.00, 1200, NULL, -10); -- Globalni Kupon 20%

-- -------------------------------------------------------------
-- NOVEMBAR 2024: (Fokus na skuplju Turu 2)
-- -------------------------------------------------------------
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-205, 78459001, 78452002, '2024-11-02 14:00:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-206, 78459002, 78452002, '2024-11-10 12:00:00', 4500.00, 3600.00, 3600, NULL, -10), -- Globalni Kupon 20%
(-207, 78459003, 78452001, '2024-11-15 13:20:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-208, 78459004, 78452001, '2024-11-22 17:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-209, 78459009, 78452002, '2024-11-28 10:00:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- -------------------------------------------------------------
-- DECEMBAR 2024: (Praznična kupovina)
-- -------------------------------------------------------------
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-210, 78459001, 78452001, '2024-12-05 18:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-211, 78459002, 78452001, '2024-12-12 15:30:00', 1500.00, 1275.00, 1275, NULL, -11),
(-212, 78459005, 78452002, '2024-12-18 20:00:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-213, 78459006, 78452002, '2024-12-22 08:45:00', 4500.00, 3600.00, 3600, NULL, -10),
(-214, 78459007, 78452001, '2024-12-28 14:20:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-215, 78459008, 78452002, '2024-12-30 22:10:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- -------------------------------------------------------------
-- JANUAR 2025: (Januarski popust - SaleId: -50)
-- -------------------------------------------------------------
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-216, 78459001, 78452001, '2025-01-02 09:00:00', 1500.00, 1050.00, 1050, -50, NULL), -- Akcija 30%
(-217, 78459002, 78452001, '2025-01-04 11:00:00', 1500.00, 1050.00, 1050, -50, NULL), -- Akcija 30%
(-218, 78459003, 78452001, '2025-01-08 15:45:00', 1500.00, 1050.00, 1050, -50, NULL), -- Akcija 30%
(-219, 78459010, 78452001, '2025-01-12 13:00:00', 1500.00, 1050.00, 1050, -50, NULL), -- Akcija 30%
(-220, 78459005, 78452001, '2025-01-22 16:30:00', 1500.00, 1500.00, 1500, NULL, NULL), -- Kraj akcije
(-221, 78459006, 78452002, '2025-01-25 19:15:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- FEBRUAR & MART 2025: Standardna prodaja (Zatišje)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-301, 78459001, 78452001, '2025-02-10 10:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-302, 78459002, 78452002, '2025-02-22 14:30:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-303, 78459003, 78452001, '2025-03-05 09:15:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-304, 78459004, 78452002, '2025-03-18 16:45:00', 4500.00, 3600.00, 3600, NULL, -10); -- Korišćen globalni kupon

-- APRIL 2025: Prolećna akcija (SaleId: -51, 25% popusta)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-305, 78459005, 78452001, '2025-04-02 11:00:00', 1500.00, 1125.00, 1125, -51, NULL),
(-306, 78459006, 78452002, '2025-04-05 13:20:00', 4500.00, 3375.00, 3375, -51, NULL),
(-307, 78459007, 78452001, '2025-04-10 10:00:00', 1500.00, 1125.00, 1125, -51, NULL),
(-308, 78459008, 78452002, '2025-04-14 15:50:00', 4500.00, 3375.00, 3375, -51, NULL);

-- MAJ & JUN 2025: Mešano (Puna cena i Kuponi)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-309, 78459009, 78452001, '2025-05-12 12:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-310, 78459010, 78452002, '2025-05-25 18:30:00', 4500.00, 3825.00, 3825, NULL, -11), -- Specifičan kupon
(-311, 78459001, 78452001, '2025-06-05 14:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-312, 78459002, 78452002, '2025-06-20 09:00:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- JUL & AVGUST 2025: Letnja sezona (Summer Coupon: -12, 10% popusta)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-313, 78459003, 78452001, '2025-07-04 11:30:00', 1500.00, 1350.00, 1350, NULL, -12),
(-314, 78459004, 78452002, '2025-07-15 17:00:00', 4500.00, 4050.00, 4050, NULL, -12),
(-315, 78459005, 78452001, '2025-08-01 10:20:00', 1500.00, 1350.00, 1350, NULL, -12),
(-316, 78459006, 78452002, '2025-08-12 14:45:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-317, 78459007, 78452001, '2025-08-25 19:10:00', 1500.00, 1350.00, 1350, NULL, -12);

-- SEPTEMBAR & OKTOBAR 2025: Jesenji trend
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-318, 78459008, 78452001, '2025-09-08 12:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-319, 78459009, 78452002, '2025-09-22 15:30:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-320, 78459010, 78452001, '2025-10-05 11:00:00', 1500.00, 1200.00, 1200, NULL, -10),
(-321, 78459001, 78452002, '2025-10-18 09:45:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-322, 78459002, 78452001, '2025-10-28 16:20:00', 1500.00, 1500.00, 1500, NULL, NULL);

-- NOVEMBAR & DECEMBAR 2025: Kraj godine (Snažan finiš)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-323, 78459003, 78452001, '2025-11-10 14:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-324, 78459004, 78452002, '2025-11-25 12:30:00', 4500.00, 3600.00, 3600, NULL, -10),
(-325, 78459005, 78452001, '2025-12-05 18:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-326, 78459006, 78452002, '2025-12-15 20:15:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-327, 78459007, 78452001, '2025-12-22 13:40:00', 1500.00, 1200.00, 1200, NULL, -10),
(-328, 78459008, 78452002, '2025-12-28 10:00:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- JANUAR 2026: (Tekući mesec - Fresh data)
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-329, 78459009, 78452001, '2026-01-05 11:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-330, 78459010, 78452002, '2026-01-15 15:30:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-331, 78459001, 78452001, '2026-01-25 09:00:00', 1500.00, 1200.00, 1200, NULL, -10);

INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-401, 78459001, 78452001, '2025-07-10 10:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-402, 78459002, 78452001, '2025-07-15 14:30:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-403, 78459003, 78452001, '2025-07-28 09:15:00', 1500.00, 1350.00, 1350, NULL, -12), -- Summer coupon
(-404, 78459004, 78452001, '2025-08-05 16:45:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-405, 78459005, 78452001, '2025-08-12 11:00:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-406, 78459006, 78452001, '2025-08-20 13:20:00', 1500.00, 1500.00, 1500, NULL, NULL),
(-407, 78459007, 78452001, '2025-08-29 10:00:00', 1500.00, 1350.00, 1350, NULL, -12); -- Summer coupon

-- PLANINSKA TURA (78452002) - Zimski "boom" (8 prodaja)
-- Cilj: Decembar 2025. i Januar 2026.
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "PurchaseDate", "OriginalPrice", "Price", "AdventureCoinsSpent", "SaleId", "CouponId")
VALUES 
(-408, 78459008, 78452002, '2025-12-10 15:50:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-409, 78459009, 78452002, '2025-12-15 12:00:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-410, 78459010, 78452002, '2025-12-20 18:30:00', 4500.00, 3600.00, 3600, NULL, -10), -- Global coupon
(-411, 78459001, 78452002, '2025-12-25 14:00:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-412, 78459002, 78452002, '2026-01-03 09:00:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-413, 78459003, 78452002, '2026-01-08 11:30:00', 4500.00, 4500.00, 4500, NULL, NULL),
(-414, 78459004, 78452002, '2026-01-12 17:00:00', 4500.00, 3600.00, 3600, NULL, -10), -- Global coupon
(-415, 78459005, 78452002, '2026-01-20 10:20:00', 4500.00, 4500.00, 4500, NULL, NULL);

-- ISPRAVKA DA BUDU KONZISTENTNI PODACI
UPDATE tours."Tours" 
SET "Price" = 1500.00 
WHERE "Id" = 78452001;

UPDATE tours."Tours" 
SET "Price" = 4500.00 
WHERE "Id" = 78452002;

-- ============================================================
-- FINALNI REZIME TEST PODATAKA
-- ============================================================
--
-- LOGIN: autor1234 / autor1234
-- ENDPOINT: GET /api/author/recommendations
--
-- OČEKIVANE PREPORUKE:
--
-- TURA 1 (78452001) - Petrovaradinska tvrđava:
--   ✅ POZITIVNA: "Ova tura ima odličan procenat završenosti!"
--   ✅ POZITIVNA: "Veliki procenat turista završava ovu turu"
--
-- TURA 2 (78452002) - Brza šetnja:
--   ✅ POZITIVNA: "Tura se brzo završava sa visokim procentom uspešnosti"
--   ✅ POZITIVNA: "Veliki procenat turista završava ovu turu"
--
-- TURA 3 (78452003) - Fruška gora:
--   ✅ POZITIVNA: "Veliki procenat turista završava ovu turu"
--
-- TURA 4 (78452004) - Beogradska produžena:
--   ❌ NEGATIVNA: "Turisti prolaze većinu ture ali je retko završavaju do kraja"
--
-- TURA 5 (78452005) - Ekstremna planinska:
--   ❌ NEGATIVNA: "Tura je možda predugačka ili preteška"
--
-- TURA 6 (78452006) - Celodnevna Tara:
--   ❌ NEGATIVNA: "Prosečno vreme izvršavanja ture je preko 6 sati"
--
-- TURA 7 (78452007) - Potcenjeno vreme:
--   ❌ NEGATIVNA: "Tura traje značajno duže od predviđenog vremena"
--
-- TURA 8 (78452008) - Ultra maraton:
--   ❌ NEGATIVNA: "Prosečno vreme izvršavanja ture je preko 6 sati"
--   ❌ NEGATIVNA: "Tura je predugačka - turisti provode mnogo vremena ali prolaze mali procenat"
--   ❌ NEGATIVNA: "Tura je možda predugačka ili preteška"
--
-- POPUSTI I KUPONI:
-- - Sale -50: Januarski popust (30%) - Testira drastičan pad cene.
-- - Sale -51: Prolećna akcija (25%) - Testira Aprilski skok prodaje.
-- - Coupon -10: Globalni SAVE2025 (20%) - Najčešće korišćen popust kroz celu godinu.
-- - Coupon -12: Letnji SUMMER25 (10%) - Testira sezonsku lojalnost.
--
-- SEZONALNOST (Trendovi na grafikonu):
-- 1. LETNJI PEAK (Jul/Avg): Fokus na "Beogradsku turu" (78452001). 
--    Simulira gradski turizam. Očekuje se veći broj transakcija manje vrednosti.
--
-- 2. ZIMSKI PEAK (Dec/Jan): Fokus na "Planinsku avanturu" (78452002).
--    Simulira ski sezonu. Iako je manje prodaja, prihod je ogroman zbog cene od 4500 EUR.
--
-- 3. PROLEĆNI "SPIKE" (April): Testira uspeh Sale akcije na obe ture istovremeno.
-- ============================================================
