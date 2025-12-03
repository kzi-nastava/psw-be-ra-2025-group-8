INSERT INTO blog."Comments" ("Id", "PersonId", "CreationTime", "Text", "LastEditTime", "BlogPostId")
VALUES
    (-1, -22, '2024-01-02 10:00:00', 'Super post, jako mi se sviđa!', NULL, -2),   -- Komentar na Published Blogu (-2) od Turiste 2 (-22)
    (-2, -21, '2024-01-02 10:05:00', 'Hvala, drago mi je!', '2024-01-02 10:10:00', -2), -- Komentar na Published Blogu (-2) od Turiste 1 (-21)
    (-3, -23, '2024-01-03 12:00:00', 'Odlična arhiva.', NULL, -3); -- Komentar na Archived Blogu (-3) od Turiste 3 (-23)