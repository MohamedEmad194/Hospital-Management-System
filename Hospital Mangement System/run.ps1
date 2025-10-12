Write-Host "Starting Hospital Management System..." -ForegroundColor Green
Write-Host ""

Set-Location "Hospital Mangement System"

Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

Write-Host ""
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

Write-Host ""
Write-Host "Starting application..." -ForegroundColor Green
Write-Host ""
Write-Host "Application will be available at:" -ForegroundColor Cyan
Write-Host "- HTTPS: https://localhost:7102" -ForegroundColor White
Write-Host "- HTTP:  http://localhost:5230" -ForegroundColor White
Write-Host "- Swagger UI: https://localhost:7102/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Default Admin User:" -ForegroundColor Cyan
Write-Host "- Email: admin@hospital.com" -ForegroundColor White
Write-Host "- Password: Admin@123" -ForegroundColor White
Write-Host ""

dotnet run
