-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
DELETE FROM payments."PurchasedItems";
DELETE FROM payments."OrderItems";
DELETE FROM payments."ShoppingCarts";
DELETE FROM payments."Coupons";

-- Also delete any test tours that might have been inserted by payments test data
DELETE FROM tours."Tours" WHERE "Id" IN (-511, -522, -533);
