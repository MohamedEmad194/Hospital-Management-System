Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Hospital Management System Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Set-Location "Hospital Mangement System"

Write-Host "[1/5] Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[2/5] Building project..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[3/5] Checking SQL Server connection..." -ForegroundColor Yellow
Write-Host "Please ensure SQL Server is running and accessible" -ForegroundColor White
Write-Host ""

Write-Host "[4/5] Starting application..." -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Application Information" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "- HTTPS URL: https://localhost:7102" -ForegroundColor White
Write-Host "- HTTP URL:  http://localhost:5230" -ForegroundColor White
Write-Host "- Swagger UI: https://localhost:7102/swagger" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Default Login Credentials" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "- Email: admin@hospital.com" -ForegroundColor White
Write-Host "- Password: Admin@123" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Database Information" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "- Database: HospitalManagementSystem" -ForegroundColor White
Write-Host "- Server: . (Local SQL Server)" -ForegroundColor White
Write-Host "- Authentication: Windows Authentication" -ForegroundColor White
Write-Host ""
Write-Host "The application will create the database automatically" -ForegroundColor Green
Write-Host "if it doesn't exist with sample data." -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

dotnet run --configuration Release
