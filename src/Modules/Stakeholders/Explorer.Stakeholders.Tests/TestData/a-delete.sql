-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)

DELETE FROM stakeholders."ClubInvitations";
DELETE FROM stakeholders."ClubJoinRequests";
DELETE FROM stakeholders."Notifications";
DELETE FROM stakeholders."FollowerMessages";
DELETE FROM stakeholders."Followers";
DELETE FROM stakeholders."Ratings";
DELETE FROM stakeholders."Messages";
DELETE FROM stakeholders."ClubMessages";
DELETE FROM stakeholders."Wallets";
-- People depends on Users
DELETE FROM stakeholders."People";
-- Users is the root table (has no dependencies)
DELETE FROM stakeholders."Users";
DELETE FROM stakeholders."Meetups";
DELETE FROM stakeholders."Clubs";
