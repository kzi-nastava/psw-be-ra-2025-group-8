-- Deleting in the correct order to respect foreign key constraints
-- First, we delete child tables (tables that have foreign keys)

-- PreferenceTags depends on TouristPreferences and Tags
TRUNCATE TABLE tours."TourRatingImages" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."TourRatings" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."BundleTours" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."Bundles" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."PreferenceTags" RESTART IDENTITY CASCADE;
-- TransportTypePreferences depends on TouristPreferences  
TRUNCATE TABLE tours."TransportTypePreferences" RESTART IDENTITY CASCADE;

-- TouristPreferences depends on People
TRUNCATE TABLE tours."TouristPreferences" RESTART IDENTITY CASCADE;
-- Tags is independent, but PreferenceTags depends on it (already deleted)
TRUNCATE TABLE tours."Tags" RESTART IDENTITY CASCADE;

TRUNCATE TABLE tours."TourTransportTimes" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."TourEquipment" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."TourTags" RESTART IDENTITY CASCADE;

TRUNCATE TABLE tours."KeyPointsReached" RESTART IDENTITY CASCADE;   
TRUNCATE TABLE tours."PersonEquipment" RESTART IDENTITY CASCADE;   
TRUNCATE TABLE tours."TourExecutions" RESTART IDENTITY CASCADE; 
TRUNCATE TABLE tours."TourChatMessages" RESTART IDENTITY CASCADE; 
TRUNCATE TABLE tours."TourChatMembers" RESTART IDENTITY CASCADE; 
TRUNCATE TABLE tours."TourChatRooms" RESTART IDENTITY CASCADE; 
TRUNCATE TABLE tours."KeyPoints" RESTART IDENTITY CASCADE;      
TRUNCATE TABLE tours."Equipment" RESTART IDENTITY CASCADE;
-- IssueMessages depends on ReportProblem
TRUNCATE TABLE tours."IssueMessages" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."ReportProblem" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."Tours" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."Facilities" RESTART IDENTITY CASCADE;
TRUNCATE TABLE tours."Monument" RESTART IDENTITY CASCADE;

