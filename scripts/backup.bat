@echo off
rem Sao luu CSDL ra backups\drone_<ngay>.sql (Windows). Can Docker dang chay.
cd /d "%~dp0.."
if not exist backups mkdir backups
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss"') do set TS=%%i
docker compose exec -T db pg_dump -U drone -d drone --clean --if-exists --no-owner > backups\drone_%TS%.sql || goto fail
echo Da sao luu: backups\drone_%TS%.sql
exit /b 0
:fail
echo [LOI] Sao luu that bai. Kiem tra docker compose ps.
exit /b 1
