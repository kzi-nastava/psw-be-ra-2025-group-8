-- i-coupons.sql
-- Test data for Coupons

-- Sample coupons for testing
-- These are pre-existing coupons that can be used in query tests

-- Author -11 coupons
INSERT INTO payments."Coupons"("Id", "Code", "DiscountPercentage", "ExpiryDate", "TourId", "AuthorId")
VALUES
(-1, 'WINTER20', 20, '2025-12-31 23:59:59+00', -511, -11),
(-2, 'SUMMER15', 15, NULL, NULL, -11);  -- Permanent coupon for all tours

-- Author -12 coupon
INSERT INTO payments."Coupons"("Id", "Code", "DiscountPercentage", "ExpiryDate", "TourId", "AuthorId")
VALUES
(-3, 'SPRING25', 25, '2025-06-30 23:59:59+00', -522, -12);
