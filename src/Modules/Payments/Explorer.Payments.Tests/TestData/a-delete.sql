-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
DELETE FROM tours."PurchasedItems";
DELETE FROM tours."OrderItems";
DELETE FROM tours."ShoppingCarts";
