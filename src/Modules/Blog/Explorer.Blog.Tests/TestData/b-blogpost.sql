INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "LastModifiedAt", "Status")
VALUES
    (-1, -21, 'Test blog 1', 'Test opis 1', '2024-01-01 00:00:00', NULL, 0),
    (-2, -21, 'Test blog 2', 'Test opis 2', '2024-01-02 00:00:00', NULL, 1),
    (-3, -21, 'Test blog 3', 'Test opis 3', '2024-01-03 00:00:00', '2024-01-04 00:00:00', 2),
    (-4, -21, 'Test blog 4', 'Test opis 4', '2024-01-04 00:00:00', NULL, 1),
    (-5, -22, 'Test blog 5', 'Test opis 5', '2024-01-05 00:00:00', NULL, 0);

INSERT INTO blog."BlogImages" ("Id", "BlogPostId", "Url", "Order")
VALUES
    (-1, -1, 'https://example.com/img1.jpg', 0),
    (-2, -1, 'https://example.com/img2.jpg', 1),
    (-3, -2, 'https://example.com/img3.jpg', 0);
