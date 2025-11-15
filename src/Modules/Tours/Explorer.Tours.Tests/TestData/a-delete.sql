DELETE FROM tours."Equipment";
DELETE FROM tours."Facilities";

-- Reset sequences to start from 1 for new inserts
ALTER SEQUENCE tours."Equipment_Id_seq" RESTART WITH 1;
ALTER SEQUENCE tours."Facilities_Id_seq" RESTART WITH 1;