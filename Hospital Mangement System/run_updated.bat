@echo off
echo ========================================
echo   Hospital Management System Setup
echo ========================================
echo.

cd "Hospital Mangement System"

echo [1/5] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo [2/5] Building project...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo [3/5] Checking SQL Server connection...
echo Please ensure SQL Server is running and accessible
echo.

echo [4/5] Starting application...
echo.
echo ========================================
echo   Application Information
echo ========================================
echo - HTTPS URL: https://localhost:7102
echo - HTTP URL:  http://localhost:5230
echo - Swagger UI: https://localhost:7102/swagger
echo.
echo ========================================
echo   Default Login Credentials
echo ========================================
echo - Email: admin@hospital.com
echo - Password: Admin@123
echo.
echo ========================================
echo   Database Information
echo ========================================
echo - Database: HospitalManagementSystem
echo - Server: . (Local SQL Server)
echo - Authentication: Windows Authentication
echo.
echo The application will create the database automatically
echo if it doesn't exist with sample data.
echo.
echo Press Ctrl+C to stop the application
echo ========================================
echo.

dotnet run --configuration Release

pause
