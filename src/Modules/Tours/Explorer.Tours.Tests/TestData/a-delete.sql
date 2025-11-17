DELETE FROM tours."Tours";
DELETE FROM tours."Equipment";

ALTER SEQUENCE tours."Tours_Id_seq" RESTART WITH 1;
ALTER SEQUENCE tours."Equipment_Id_seq" RESTART WITH 1;