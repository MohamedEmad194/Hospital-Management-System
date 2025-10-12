@echo off
echo Starting Hospital Management System...
echo.

cd "Hospital Mangement System"

echo Restoring packages...
dotnet restore

echo.
echo Building project...
dotnet build

echo.
echo Starting application...
echo.
echo Application will be available at:
echo - HTTPS: https://localhost:7102
echo - HTTP:  http://localhost:5230
echo - Swagger UI: https://localhost:7102/swagger
echo.
echo Default Admin User:
echo - Email: admin@hospital.com
echo - Password: Admin@123
echo.

dotnet run

pause
