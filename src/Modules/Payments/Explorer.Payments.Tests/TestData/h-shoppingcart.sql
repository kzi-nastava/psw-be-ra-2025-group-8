-- h-shoppingcart.sql
-- Test podaci za Shopping Cart

-- Prazna korpa za turista1 (-21)
DELETE FROM payments."OrderItems" WHERE "ShoppingCartId" = -500;
DELETE FROM payments."ShoppingCarts" WHERE "Id" = -500 OR "UserId" = -21;
INSERT INTO payments."ShoppingCarts"("Id", "UserId")
VALUES (-500, -21);

-- Test korpa sa jednom stavkom za turista2 (-22)
DELETE FROM payments."OrderItems" WHERE "ShoppingCartId" = -202;
DELETE FROM payments."ShoppingCarts" WHERE "Id" = -202 OR "UserId" = -22;
INSERT INTO payments."ShoppingCarts"("Id", "UserId")
VALUES (-202, -22);

INSERT INTO payments."OrderItems"
("Id", "ShoppingCartId", "TourId", "OriginalPrice", "DiscountedPrice", "IsBundle", "BundleId", "CouponId", "SaleId")
VALUES
(-301, -202, -511, 50.00, 50.00, FALSE, NULL, NULL, NULL);
