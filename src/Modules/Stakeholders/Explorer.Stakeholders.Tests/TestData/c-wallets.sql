-- Insert test wallets for existing test users
-- This script should run after b-users.sql

-- Wallet for admin (though admins don't typically use wallets)
INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-1, -1, 0);

-- Wallets for authors (need AC for ShoppingCart tests)
INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-11, -11, 5000);

INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-12, -12, 5000);

INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-13, -13, 5000);

-- Wallets for tourists (with varying amounts for testing)
INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-21, -21, 1000);

INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-22, -22, 500);

INSERT INTO stakeholders."Wallets"("Id", "UserId", "AdventureCoins")
VALUES (-23, -23, 2000);

