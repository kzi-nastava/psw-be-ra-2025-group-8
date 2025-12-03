-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)

DELETE FROM stakeholders."Ratings";
DELETE FROM stakeholders."Messages";
-- People depends on Users
DELETE FROM stakeholders."People";
-- Users is the root table (has no dependencies)
DELETE FROM stakeholders."Users";
DELETE FROM stakeholders."Meetups";
DELETE FROM stakeholders."Clubs";
