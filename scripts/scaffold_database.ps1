Write-Host "Starting DB"
docker compose up db -d 2>&1 > $null
Write-Host "Applying migrations"
.\apply_migrations.ps1
Write-Host "Stoping DB"
docker stop ksummarizedcom-db-1 2>&1 > $null
Write-Host "Done"