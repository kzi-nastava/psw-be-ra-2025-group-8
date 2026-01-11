-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
TRUNCATE TABLE payments."PurchasedItems" RESTART IDENTITY CASCADE;
TRUNCATE TABLE payments."OrderItems" RESTART IDENTITY CASCADE;
TRUNCATE TABLE payments."ShoppingCarts" RESTART IDENTITY CASCADE;
TRUNCATE TABLE payments."Coupons" RESTART IDENTITY CASCADE;

