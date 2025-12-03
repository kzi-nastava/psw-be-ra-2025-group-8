-- c-shoppingcart.sql
-- Test podaci za Shopping Cart

-- Test korisnik (turista)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-41, 'test_tourist', 'pass', 2, true);

-- Test ture
INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId","LengthInKilometers")
VALUES
(-511, 'Beogradska avantura', 'Tura po Beogradu', 1, 1, 50, -11,1),
(-522, 'Planinska tura', 'Tara i Zlatibor', 2, 1, 100, -11,1),
(-533, 'Dunavska ruta', 'Tura duž Dunava', 1, 1, 70, -11,1);

-- Prazna korpa za testnog korisnika
INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-500, -41);

-- Test korpa sa jednom stavkom (korisnik -12)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-42, 'test_tourist2', 'pass', 2, true);

INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-202, -42);

INSERT INTO tours."OrderItems"("Id", "ShoppingCartId", "TourId", "Price")
VALUES 
(-301, -202, -101, 50);
