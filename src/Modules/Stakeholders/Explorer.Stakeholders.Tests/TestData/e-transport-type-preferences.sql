-- za preference -101 (four transport entries, default rating 0)
INSERT INTO stakeholders."TransportTypePreferences" ("Id","PreferenceId","Transport","Rating")
VALUES
(-1001, -101, 'Walk', 0),
(-1002, -101, 'Bicycle', 0),
(-1003, -101, 'Car', 0),
(-1004, -101, 'Boat', 0);

-- za preference -102 (some non-zero defaults so tests can assert changes)
INSERT INTO stakeholders."TransportTypePreferences" ("Id","PreferenceId","Transport","Rating")
VALUES
(-1101, -102, 'Walk', 1),
(-1102, -102, 'Bicycle', 2),
(-1103, -102, 'Car', 0),
(-1104, -102, 'Boat', 3);

