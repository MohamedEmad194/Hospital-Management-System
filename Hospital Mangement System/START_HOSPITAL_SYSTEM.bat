@echo off
title Hospital Management System
color 0A

echo.
echo ================================================================
echo                    HOSPITAL MANAGEMENT SYSTEM
echo ================================================================
echo.
echo Starting Hospital Management System...
echo.

cd "Hospital Mangement System"

echo [1/4] Checking prerequisites...
echo - Checking .NET 8.0...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET 8.0 is not installed!
    echo Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo - .NET 8.0: OK

echo - Checking SQL Server...
sqlcmd -S . -E -Q "SELECT 1" >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: SQL Server connection failed!
    echo Please ensure SQL Server is running.
    echo.
    echo Trying to continue anyway...
) else (
    echo - SQL Server: OK
)

echo.
echo [2/4] Restoring packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo [3/4] Building project...
dotnet build --configuration Release --verbosity quiet
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo [4/4] Starting application...
echo.
echo ================================================================
echo                        APPLICATION READY
echo ================================================================
echo.
echo 🌐 Application URLs:
echo    - HTTPS: https://localhost:7102
echo    - HTTP:  http://localhost:5230
echo    - Swagger: https://localhost:7102/swagger
echo.
echo 🔑 Default Login:
echo    - Email: admin@hospital.com
echo    - Password: Admin@123
echo.
echo 📊 Database:
echo    - Name: HospitalManagementSystem
echo    - Server: . (Local SQL Server)
echo    - Auto-created with sample data
echo.
echo ⚠️  Important Notes:
echo    - Make sure SQL Server is running
echo    - The database will be created automatically
echo    - Press Ctrl+C to stop the application
echo.
echo ================================================================
echo.

echo Starting server...
dotnet run --configuration Release

echo.
echo Application stopped.
pause
