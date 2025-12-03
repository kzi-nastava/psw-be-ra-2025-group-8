-- c-shoppingcart.sql
-- Test podaci za Shopping Cart

-- Test korisnik (turista)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-41, 'test_tourist', 'pass', 2, true)

-- Test ture
INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId")
VALUES
(-101, 'Beogradska avantura', 'Tura po Beogradu', 1, ARRAY('city','serbia'), 1, 50, -101),
(-102, 'Planinska tura', 'Tara i Zlatibor', 2, ARRAY('nature','mountain'), 1, 100, -101),
(-103, 'Dunavska ruta', 'Tura duž Dunava', 1, ARRAY('river','cycling'), 1, 70, -101)

-- Prazna korpa za testnog korisnika
INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-201, -41)

-- Test korpa sa jednom stavkom (korisnik -12)
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES 
(-42, 'test_tourist2', 'pass', 2, true)

INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-202, -42)

INSERT INTO tours."OrderItems"("Id", "ShoppingCartId", "TourId", "Price")
VALUES 
(-301, -202, -101, 50)
