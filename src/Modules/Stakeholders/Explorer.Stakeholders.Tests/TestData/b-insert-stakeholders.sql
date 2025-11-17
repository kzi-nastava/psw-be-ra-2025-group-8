INSERT INTO stakeholders."Users" ("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-100, 'admin@test.com', 'admin', 0, true),
(-101, 'author1@test.com', 'author1', 1, true),
(-102, 'tourist1@test.com', 'tourist1', 2, true);

INSERT INTO stakeholders."People" ("Id", "UserId", "Email", "Name", "Surname")
VALUES
(-200, -100, 'admin@test.com', 'Admin', 'Test'),
(-201, -101, 'author1@test.com', 'Author', 'One'),
(-202, -102, 'tourist1@test.com', 'Tourist', 'One');
