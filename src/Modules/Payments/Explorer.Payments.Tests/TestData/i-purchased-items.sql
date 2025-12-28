-- i-purchased-items.sql
-- Test data for purchased items with UserId and AdventureCoinsSpent

-- Add some purchased items for testing
INSERT INTO payments."PurchasedItems"("Id", "UserId", "TourId", "Price", "AdventureCoinsSpent", "PurchaseDate")
VALUES 
(-1001, -21, -511, 50.00, 50, '2024-01-15 10:30:00+00'),
(-1002, -21, -522, 100.00, 100, '2024-01-16 14:20:00+00'),
(-1003, -22, -533, 70.00, 70, '2024-01-17 09:15:00+00');
