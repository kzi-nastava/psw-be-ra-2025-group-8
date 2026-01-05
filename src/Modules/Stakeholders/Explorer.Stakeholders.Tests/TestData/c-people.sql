INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-11, -11, 'Ana', 'Anić', 'autor1@gmail.com', 'https://example.com/ana.jpg', 'Passionate tour author', 'Adventure awaits!', 0, 1);
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-12, -12, 'Lena', 'Lenić', 'autor2@gmail.com', NULL, 'Tour guide and writer', NULL, 0, 1);
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-13, -13, 'Sara', 'Sarić', 'autor3@gmail.com', NULL, NULL, NULL, 0, 1);

INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-21, -21, 'Pera', 'Perić', 'turista1@gmail.com', 'https://example.com/pera.jpg', 'Travel enthusiast', 'Explore the world', 0, 1);
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-22, -22, 'Mika', 'Mikić', 'turista2@gmail.com', NULL, NULL, NULL, 0, 1);
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-23, -23, 'Steva', 'Stević', 'turista3@gmail.com', NULL, 'Just a tourist', 'Wanderlust', 0, 1);

-- Ensure persons exist for integration-only users
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-100, -100, 'Integration', 'Admin', 'integration_admin@example.com', NULL, 'Integration admin', NULL, 0, 1);

INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-101, -101, 'Existing', 'User1', 'existing_user1@example.com', NULL, NULL, NULL, 0, 1);

INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
VALUES (-102, -102, 'Existing', 'User2', 'existing_user2@example.com', NULL, NULL, NULL, 0, 1);
