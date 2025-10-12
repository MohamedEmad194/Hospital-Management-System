# Hospital Management System Startup Script
$Host.UI.RawUI.WindowTitle = "Hospital Management System"
$Host.UI.RawUI.BackgroundColor = "Black"
$Host.UI.RawUI.ForegroundColor = "Green"
Clear-Host

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "                    HOSPITAL MANAGEMENT SYSTEM" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Starting Hospital Management System..." -ForegroundColor Green
Write-Host ""

Set-Location "Hospital Mangement System"

Write-Host "[1/4] Checking prerequisites..." -ForegroundColor Yellow
Write-Host "- Checking .NET 8.0..." -ForegroundColor White
try {
    $dotnetVersion = dotnet --version
    Write-Host "- .NET 8.0: OK ($dotnetVersion)" -ForegroundColor Green
} catch {
    Write-Host "ERROR: .NET 8.0 is not installed!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "- Checking SQL Server..." -ForegroundColor White
try {
    sqlcmd -S . -E -Q "SELECT 1" | Out-Null
    Write-Host "- SQL Server: OK" -ForegroundColor Green
} catch {
    Write-Host "WARNING: SQL Server connection failed!" -ForegroundColor Yellow
    Write-Host "Please ensure SQL Server is running." -ForegroundColor Yellow
    Write-Host "Trying to continue anyway..." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[2/4] Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[3/4] Building project..." -ForegroundColor Yellow
dotnet build --configuration Release --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[4/4] Starting application..." -ForegroundColor Yellow
Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "                        APPLICATION READY" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Application URLs:" -ForegroundColor White
Write-Host "   - HTTPS: https://localhost:7102" -ForegroundColor Cyan
Write-Host "   - HTTP:  http://localhost:5230" -ForegroundColor Cyan
Write-Host "   - Swagger: https://localhost:7102/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "🔑 Default Login:" -ForegroundColor White
Write-Host "   - Email: admin@hospital.com" -ForegroundColor Yellow
Write-Host "   - Password: Admin@123" -ForegroundColor Yellow
Write-Host ""
Write-Host "📊 Database:" -ForegroundColor White
Write-Host "   - Name: HospitalManagementSystem" -ForegroundColor Cyan
Write-Host "   - Server: . (Local SQL Server)" -ForegroundColor Cyan
Write-Host "   - Auto-created with sample data" -ForegroundColor Cyan
Write-Host ""
Write-Host "⚠️  Important Notes:" -ForegroundColor White
Write-Host "   - Make sure SQL Server is running" -ForegroundColor Yellow
Write-Host "   - The database will be created automatically" -ForegroundColor Yellow
Write-Host "   - Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Starting server..." -ForegroundColor Green
dotnet run --configuration Release

Write-Host ""
Write-Host "Application stopped." -ForegroundColor Yellow
Read-Host "Press Enter to exit"
