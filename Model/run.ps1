# Start X-Ray FastAPI and open Swagger UI in the default browser.
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$port = if ($env:XRAY_PORT) { $env:XRAY_PORT } else { "8000" }
$hostAddr = "127.0.0.1"
$docsUrl = "http://${hostAddr}:${port}/docs"

Write-Host "Installing Python dependencies (first run may take a while)..."
py -3 -m pip install -q -r requirements.txt

Write-Host "Starting API at $docsUrl"
Start-Process $docsUrl

py -3 -m uvicorn project:app --host $hostAddr --port $port --reload
