Write-Output "Generateing migration script"
Set-Location ../backend/src/api
mkdir migration_scripts 2>&1 > $null 
dotnet ef migrations script --idempotent -o ./migration_scripts/migration.sql 2>&1 > $null

Write-Output "Applying.."
$passwd = "PGPASSWORD=" + $Env:POSTGRES_PASSWORD
$src = Join-Path $(Get-Location).Path migration_scripts
docker run --network host -e $passwd -v ${src}:/migrations/ --rm postgres `
    psql -h localhost -U $Env:POSTGRES_USER -d users -f /migrations/migration.sql `
    2>&1 > $null

Write-Output "Cleanup"
Remove-Item -Recurse -Force migration_scripts
Set-Location ../../../scripts
Write-Output "Migrations had been applyed!"