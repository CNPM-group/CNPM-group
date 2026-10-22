@echo off
chcp 65001 >nul
setlocal
REM ===== SmartDroneDelivery - chay thu + ghi ket qua =====
REM Dat file nay o thu muc goc cua repo (cung cap voi docker-compose.yml), double-click de chay.
cd /d "%~dp0"
REM Cho phep app net8.0 chay tren runtime .NET 10 (may chi cai .NET 10)
set DOTNET_ROLL_FORWARD=Major
set OUT=ket-qua-test.txt
echo ===== KET QUA CHAY THU ===== > %OUT%
echo Thoi gian: %date% %time% >> %OUT%

echo [1/6] Kiem tra .NET va Docker...
dotnet --version >nul 2>&1 || (echo THIEU .NET 8 SDK. Cai tai https://dotnet.microsoft.com/download/dotnet/8.0 & echo THIEU .NET SDK >> %OUT% & pause & exit /b 1)
docker --version >nul 2>&1 || (echo THIEU Docker Desktop. Cai va MO no len roi chay lai. & echo THIEU Docker >> %OUT% & pause & exit /b 1)
echo .NET: & dotnet --version
echo .NET version: >> %OUT% & dotnet --version >> %OUT%

echo [2/6] Tao file .env neu chua co...
if not exist .env copy .env.example .env >nul

echo [3/6] Bat database (Docker)...
docker compose up -d db
if errorlevel 1 (echo Docker loi. Hay chac chan Docker Desktop dang chay. & echo Docker compose: LOI >> %OUT% & pause & exit /b 1)
echo Docker compose: OK >> %OUT%
timeout /t 8 /nobreak >nul

echo [4/6] Build...
dotnet build src\SmartDrone.Api\SmartDrone.Api.csproj -nologo -v q
if errorlevel 1 (echo BUILD LOI. Chup man hinh gui lai. & echo Build: LOI >> %OUT% & pause & exit /b 1)
echo Build: THANH CONG >> %OUT%

echo [5/6] Tao migration neu chua co...
dotnet ef --version >nul 2>&1 || dotnet tool install --global dotnet-ef
if not exist src\SmartDrone.Infrastructure\Persistence\Migrations (
  dotnet ef migrations add Initial -p src\SmartDrone.Infrastructure -s src\SmartDrone.Api -o Persistence/Migrations
  if errorlevel 1 (echo Migration: LOI >> %OUT% & echo MIGRATION LOI. Chup man hinh gui lai. & pause & exit /b 1)
)
echo Migration: OK >> %OUT%

echo [6/6] Chay API va test dang nhap...
start "SmartDrone API" cmd /k dotnet run --project src\SmartDrone.Api --launch-profile http
set /a n=0
:wait
timeout /t 3 /nobreak >nul
set /a n+=1
curl -s -o nul -w "%%{http_code}" http://localhost:5080/health > health.tmp 2>nul
set /p H=<health.tmp
del health.tmp >nul 2>&1
if "%H%"=="200" goto up
if %n% lss 20 goto wait
echo API khong len duoc sau 60 giay. Xem cua so "SmartDrone API" de doc loi. >> %OUT%
echo API KHONG LEN. Xem cua so SmartDrone API, chup man hinh gui lai.
pause & exit /b 1

:up
echo Health check (/health): 200 OK >> %OUT%
curl -s -o login.tmp -w "%%{http_code}" -X POST http://localhost:5080/api/v1/auth/login -H "Content-Type: application/json" -d "{\"email\":\"admin@drone.local\",\"password\":\"Demo@12345\"}" > code.tmp
set /p L=<code.tmp
del code.tmp >nul 2>&1
echo Login admin@drone.local: HTTP %L% >> %OUT%
if "%L%"=="200" (echo Ket qua: DANG NHAP THANH CONG, nhan duoc token >> %OUT%) else (echo Ket qua: DANG NHAP LOI >> %OUT% & type login.tmp >> %OUT%)
del login.tmp >nul 2>&1

echo.
echo ===== XONG. Ket qua luu trong file ket-qua-test.txt =====
type %OUT%
start http://localhost:5080/swagger
echo.
echo Chup man hinh: (1) cua so nay, (2) trang Swagger vua mo.
pause
