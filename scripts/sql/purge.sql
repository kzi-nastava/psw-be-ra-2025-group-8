SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'explorer-v1'
  AND pid <> pg_backend_pid();

SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'explorer-v1-test'
  AND pid <> pg_backend_pid();

DROP DATABASE IF EXISTS "explorer-v1";
DROP DATABASE IF EXISTS "explorer-v1-test";