-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
DELETE FROM payments."PurchasedItems";
DELETE FROM payments."OrderItems";
DELETE FROM payments."ShoppingCarts";
DELETE FROM payments."Coupons";
DELETE FROM payments."SaleTours";
DELETE FROM payments."Sales";

