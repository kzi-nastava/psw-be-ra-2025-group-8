-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
-- PreferenceTags depends on TouristPreferences and Tags
DELETE FROM stakeholders."PreferenceTags";
-- TransportTypePreferences depends on TouristPreferences  
DELETE FROM stakeholders."TransportTypePreferences";
-- TouristPreferences depends on People
DELETE FROM stakeholders."TouristPreferences";
-- Tags is independent, but PreferenceTags depends on it (already deleted)
DELETE FROM stakeholders."Tags";
DELETE FROM stakeholders."Ratings";
DELETE FROM stakeholders."Messages";
-- People depends on Users
DELETE FROM stakeholders."People";
-- Users is the root table (has no dependencies)
DELETE FROM stakeholders."Users";
DELETE FROM stakeholders."Meetups";
DELETE FROM stakeholders."Clubs";
