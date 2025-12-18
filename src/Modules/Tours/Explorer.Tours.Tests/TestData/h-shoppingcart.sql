-- h-shoppingcart.sql
-- Test podaci za Shopping Cart

-- Test ture za shopping cart testove
INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Status", "Price", "AuthorId","LengthInKilometers")
VALUES
(-511, 'Beogradska avantura', 'Tura po Beogradu', 1, 0, 50, -11, 1),
(-522, 'Planinska tura', 'Tara i Zlatibor', 2, 0, 100, -11, 1),
(-533, 'Dunavska ruta', 'Tura duž Dunava', 1, 0, 70, -11, 1);

-- Prazna korpa za turista1 (-21)
INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-500, -21);

-- Test korpa sa jednom stavkom za turista2 (-22)
INSERT INTO tours."ShoppingCarts"("Id", "UserId")
VALUES (-202, -22);

INSERT INTO tours."OrderItems"("Id", "ShoppingCartId", "TourId")
VALUES 
(-301, -202, -511);