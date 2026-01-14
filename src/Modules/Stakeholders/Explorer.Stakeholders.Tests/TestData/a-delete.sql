-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)

TRUNCATE TABLE stakeholders."ClubInvitations" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."ClubJoinRequests" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Notifications" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."FollowerMessages" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Followers" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Ratings" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Messages" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."ClubMessages" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Wallets" RESTART IDENTITY CASCADE;
-- People depends on Users
TRUNCATE TABLE stakeholders."People" RESTART IDENTITY CASCADE;
-- Users is the root table (has no dependencies)
TRUNCATE TABLE stakeholders."Users" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Meetups" RESTART IDENTITY CASCADE;
TRUNCATE TABLE stakeholders."Clubs" RESTART IDENTITY CASCADE;

