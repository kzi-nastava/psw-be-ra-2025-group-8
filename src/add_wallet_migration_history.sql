-- Add migration to history table
-- Run this AFTER you create the Wallets table

INSERT INTO stakeholders."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251225130000_AddWalletTable', '8.0.11');

-- Verify migration was added
SELECT * FROM stakeholders."__EFMigrationsHistory" ORDER BY "MigrationId";
