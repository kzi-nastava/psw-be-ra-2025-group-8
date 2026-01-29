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
-- 3. KREIRANJE TOUR EXECUTIONS
-- ============================================================

-- -------------------------------------------------------------
-- TURA 1 (78452001): "Beogradska kulturna tura"
-- Completion Rate: 80% (8 Completed, 2 Abandoned, 0 InProgress)
-- Average Completion Percentage: ~75%
-- -------------------------------------------------------------

-- Completed executions (8x)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity")
VALUES
    -- Turista 1 - Completed, 100%
    (78453001, 78452001, 20.4489, 44.7866, 78459001, 100.0, 1, '2025-01-10 15:30:00'),

    -- Turista 2 - Completed, 100%
    (78453002, 78452001, 20.4512, 44.7890, 78459002, 100.0, 1, '2025-01-11 16:45:00'),

    -- Turista 3 - Completed, 95%
    (78453003, 78452001, 20.4501, 44.7877, 78459003, 95.0, 1, '2025-01-12 14:20:00'),

    -- Turista 4 - Completed, 85%
    (78453004, 78452001, 20.4495, 44.7881, 78459004, 85.0, 1, '2025-01-13 17:10:00'),

    -- Turista 5 - Completed, 70%
    (78453005, 78452001, 20.4488, 44.7869, 78459005, 70.0, 1, '2025-01-14 13:50:00'),

    -- Turista 6 - Completed, 65%
    (78453006, 78452001, 20.4505, 44.7885, 78459006, 65.0, 1, '2025-01-15 12:30:00'),

    -- Turista 7 - Completed, 60%
    (78453007, 78452001, 20.4490, 44.7872, 78459007, 60.0, 1, '2025-01-16 11:15:00'),

    -- Turista 8 - Completed, 55%
    (78453008, 78452001, 20.4510, 44.7893, 78459008, 55.0, 1, '2025-01-17 10:00:00');

-- Abandoned executions (2x)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity")
VALUES
    -- Turista 9 - Abandoned, 35%
    (78453009, 78452001, 20.4497, 44.7874, 78459009, 35.0, 2, '2025-01-18 09:20:00'),

    -- Turista 10 - Abandoned, 20%
    (78453010, 78452001, 20.4492, 44.7870, 78459010, 20.0, 2, '2025-01-19 08:45:00');

-- OČEKIVANE STATISTIKE ZA TURU 1:
-- Completion Rate: 8 / (8 + 2) * 100 = 80%
-- Average Completion Percentage: (100+100+95+85+70+65+60+55+35+20) / 10 = 68.5%

-- -------------------------------------------------------------
-- TURA 2 (78452002): "Ekstremna planinska avantura"
-- Completion Rate: 20% (1 Completed, 4 Abandoned, 0 InProgress)
-- Average Completion Percentage: ~35%
-- -------------------------------------------------------------

-- Completed execution (1x)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity")
VALUES
    -- Turista 3 (Professional) - Completed, 100%
    (78453011, 78452002, 20.8167, 43.2975, 78459003, 100.0, 1, '2025-01-08 18:30:00');

-- Abandoned executions (4x)
INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity")
VALUES
    -- Turista 1 (Beginner) - Abandoned, 15% (teška tura za početnike)
    (78453012, 78452002, 20.8145, 43.2960, 78459001, 15.0, 2, '2024-12-20 11:30:00'),

    -- Turista 4 (Beginner) - Abandoned, 25%
    (78453013, 78452002, 20.8152, 43.2968, 78459004, 25.0, 2, '2024-12-25 14:10:00'),

    -- Turista 6 (Beginner) - Abandoned, 30%
    (78453014, 78452002, 20.8160, 43.2972, 78459006, 30.0, 2, '2025-01-02 10:45:00'),

    -- Turista 10 (Beginner) - Abandoned, 50%
    (78453015, 78452002, 20.8165, 43.2973, 78459010, 50.0, 2, '2025-01-05 16:20:00');

-- OČEKIVANE STATISTIKE ZA TURU 2:
-- Completion Rate: 1 / (1 + 4) * 100 = 20%
-- Average Completion Percentage: (100 + 15 + 25 + 30 + 50) / 5 = 44%

-- ============================================================
-- 4. DODAVANJE IN-PROGRESS EXECUTIONS (NE UTIČU NA STATISTIKU)
-- ============================================================

-- Ovi TourExecutions su trenutno aktivni i NE ULAZE u statistiku
-- jer se racunaju samo Completed i Abandoned

INSERT INTO tours."TourExecutions"(
    "Id", "IdTour", "Longitude", "Latitude", "IdTourist", "CompletionPercentage", "Status", "LastActivity")
VALUES
    -- Turista 2 na Turi 2 - In Progress, 40%
    (78453016, 78452002, 20.8155, 43.2970, 78459002, 40.0, 0, '2025-01-20 12:00:00'),

    -- Turista 5 na Turi 2 - In Progress, 65%
    (78453017, 78452002, 20.8163, 43.2974, 78459005, 65.0, 0, '2025-01-20 14:30:00');

-- ============================================================
-- 5. KREIRANJE TOUR RATINGS (RECENZIJE)
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
-- 2. GET /api/author/tour/78452001/stats - Očekuje: CompletionRate=80%, AvgCompletion=68.5%
-- 3. GET /api/author/tour/78452002/stats - Očekuje: CompletionRate=20%, AvgCompletion=44%

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
