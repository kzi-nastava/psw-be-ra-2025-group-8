-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)
-- PreferenceTags depends on TouristPreferences and Tags
DELETE FROM tours."PreferenceTags";
-- TransportTypePreferences depends on TouristPreferences  
DELETE FROM tours."TransportTypePreferences";
-- TouristPreferences depends on People
DELETE FROM tours."TouristPreferences";
-- Tags is independent, but PreferenceTags depends on it (already deleted)
DELETE FROM tours."Tags";

DELETE FROM tours."Equipment";
-- IssueMessages depends on ReportProblem
DELETE FROM tours."IssueMessages";
DELETE FROM tours."ReportProblem";
DELETE FROM tours."Tours";
DELETE FROM tours."PersonEquipment";
DELETE FROM tours."Facilities";
DELETE FROM tours."Monument";