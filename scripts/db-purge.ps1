$DB_USER = "postgres"
$DB_HOST = "localhost"
$DB_PORT = "5432"

$env:PGPASSWORD = "super"

Write-Host "Running SQL commands from file..."
psql -U $DB_USER -h $DB_HOST -p $DB_PORT -f "../scripts/sql/purge.sql"

Remove-Item Env:\PGPASSWORD

Write-Host "Operation completed."
