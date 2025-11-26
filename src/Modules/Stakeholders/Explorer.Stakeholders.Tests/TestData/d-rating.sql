INSERT INTO stakeholders."Ratings" ("Id", "UserId", "Grade", "Comment", "CreationDate")
VALUES (-1, -12, 5, 'Odlična ocena za platformu!', '2025-11-01 10:00:00');

-- Ocena 2: Kreirana od strane korisnika sa ID = 3 (Turista)
INSERT INTO stakeholders."Ratings" ("Id", "UserId", "Grade", "Comment", "CreationDate")
VALUES (-2, -13, 4, 'Dobra ocena, ali može bolje.', '2025-11-05 15:30:00');

-- Ocena 3: Kreirana od strane korisnika sa ID = 3 (Turista) - namenjeno za brisanje
INSERT INTO stakeholders."Ratings" ("Id", "UserId", "Grade", "Comment", "CreationDate")
VALUES (-3, -21, 3, 'Ova ocena se briše.', '2025-11-10 09:00:00');

-- Ocena 4: Kreirana od strane korisnika sa ID = 2 (Autor) - namenjeno za izmenu
INSERT INTO stakeholders."Ratings" ("Id", "UserId", "Grade", "Comment", "CreationDate")
VALUES (-4, -22, 1, 'Loše.', '2025-11-15 11:00:00');
