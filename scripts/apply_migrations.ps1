#!/usr/bin/env pwsh
Write-Host "Loading Env"
get-content ../.env | foreach {
    if($_.StartsWith("#") -or [string]::IsNullOrWhitespace($_)){
        return
    }
    $name, $value = $_.split('=')
    set-content env:\$name $value
}

Write-Host "Starting DB"
docker compose up db -d 2>&1 > $null
Write-Output "Generateing migration script"
Set-Location ../backend
mkdir migration_scripts 2>&1 > $null
dotnet ef migrations script --idempotent -o ./migration_scripts/migration.sql -p ./src/infrastructure -s ./src/api

Write-Output "Applying.."
$passwd = "PGPASSWORD=" + $Env:POSTGRES_PASSWORD
$src = Join-Path $(Get-Location).Path migration_scripts
docker run --network host -e $passwd -v ${src}:/migrations/ --rm postgres `
    psql -h localhost -U $Env:POSTGRES_USER -d ksummarized -f /migrations/migration.sql `
    2>&1 > $null

Write-Output "Cleanup"
Remove-Item -Recurse -Force migration_scripts
Set-Location ../scripts
Write-Output "Migrations had been applyed!"
Write-Host "Stoping DB"
docker stop ks-database 2>&1 > $null
Write-Host "Done"
