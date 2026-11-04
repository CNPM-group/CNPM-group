$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

if (-not (Test-Path .env)) { Copy-Item .env.example .env }
docker compose up -d db

for ($i = 0; $i -lt 20; $i++) {
    docker compose exec -T db pg_isready -U drone -d drone *> $null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL san sang o localhost:5432 (db=drone, user=drone)"
        exit 0
    }
    Start-Sleep -Seconds 2
}

Write-Error "PostgreSQL chua len sau 40s, xem: docker compose logs db"
