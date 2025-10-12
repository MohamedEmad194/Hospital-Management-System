param(
    [string]$BaseUrl = 'http://localhost:5230',
    [string]$Email = 'admin@hospital.com',
    [string]$Password = 'Admin@123',
    [switch]$Insecure
)

$ErrorActionPreference = 'Stop'

if ($Insecure) {
    try {
        add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public static class TrustAllCerts {
    public static void Enable() {
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    }
}
"@
        [TrustAllCerts]::Enable()
        Write-Host "TLS certificate validation disabled for this session." -ForegroundColor Yellow
    } catch {}
}

function Write-Result {
    param(
        [string]$Method,
        [string]$Path,
        [int]$StatusCode,
        [double]$Ms,
        [string]$Note = ''
    )
    $statusText = if ($StatusCode -ge 200 -and $StatusCode -lt 300) { 'OK' } elseif ($StatusCode -ge 400 -and $StatusCode -lt 500) { 'CLIENT_ERROR' } elseif ($StatusCode -ge 500) { 'SERVER_ERROR' } else { 'OTHER' }
    Write-Host ("{0,-6} {1,-45} -> {2,3} {3,-12} {4}" -f $Method.ToUpper(), $Path, $StatusCode, ("{0}ms" -f [math]::Round($Ms)), $statusText) -ForegroundColor (if ($StatusCode -ge 200 -and $StatusCode -lt 300) { 'Green' } elseif ($StatusCode -ge 500) { 'Red' } else { 'Yellow' })
}

function Invoke-RequestSafe {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        $Body
    )
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        if ($null -ne $Body) {
            $response = Invoke-WebRequest -Method $Method -Uri $Url -Headers $Headers -ContentType 'application/json' -Body ($Body | ConvertTo-Json -Depth 8)
        } else {
            $response = Invoke-WebRequest -Method $Method -Uri $Url -Headers $Headers
        }
        $sw.Stop()
        return @{ StatusCode = [int]$response.StatusCode; TimeMs = $sw.Elapsed.TotalMilliseconds; Content = $response.Content }
    }
    catch {
        $sw.Stop()
        $status = 0
        try { if ($_.Exception.Response -and $_.Exception.Response.StatusCode) { $status = [int]$_.Exception.Response.StatusCode } } catch {}
        return @{ StatusCode = $status; TimeMs = $sw.Elapsed.TotalMilliseconds; Error = $_.Exception.Message }
    }
}

function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Path,
        $Body = $null
    )
    $url = "$BaseUrl$Path"
    $headers = @{}
    if ($script:Token) { $headers['Authorization'] = "Bearer $($script:Token)" }
    $result = Invoke-RequestSafe -Method $Method -Url $url -Headers $headers -Body $Body
    Write-Result -Method $Method -Path $Path -StatusCode $result.StatusCode -Ms $result.TimeMs
}

Write-Host "Logging in to $BaseUrl ..." -ForegroundColor Cyan
try {
    # Wait for server readiness by polling swagger json
    $healthUrl = "$BaseUrl/swagger/v1/swagger.json"
    $ready = $false
    for ($i=0; $i -lt 20; $i++) {
        $probe = Invoke-RequestSafe -Method 'GET' -Url $healthUrl -Headers @{} -Body $null
        if ($probe.StatusCode -ge 200 -and $probe.StatusCode -lt 300) { $ready = $true; break }
        Start-Sleep -Seconds 1
    }
    if (-not $ready) { Write-Host "Server not ready (swagger unreachable)." -ForegroundColor Yellow }
}
catch {}
try {
    $loginPayload = @{ email = $Email; password = $Password }
    $loginResp = Invoke-RequestSafe -Method 'POST' -Url "$BaseUrl/api/auth/login" -Headers @{} -Body $loginPayload
    if ($loginResp.StatusCode -lt 200 -or $loginResp.StatusCode -ge 300) {
        Write-Host "Login failed with status $($loginResp.StatusCode). Cannot proceed." -ForegroundColor Red
        exit 1
    }
    $parsed = $null
    try { $parsed = $loginResp.Content | ConvertFrom-Json } catch {}
    if ($null -eq $parsed -or [string]::IsNullOrWhiteSpace($parsed.Token)) {
        Write-Host "Login response did not include a token. Cannot proceed." -ForegroundColor Red
        exit 1
    }
    $script:Token = $parsed.Token
    Write-Host "Login OK, token acquired." -ForegroundColor Green
}
catch {
    Write-Host "Unexpected error during login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Testing endpoints..." -ForegroundColor Cyan

# Auth
Test-Endpoint -Method 'GET' -Path '/api/auth/profile'

# Core entities (mainly GETs to avoid mutations)
Test-Endpoint -Method 'GET' -Path '/api/patients'
Test-Endpoint -Method 'GET' -Path '/api/patients/1'

Test-Endpoint -Method 'GET' -Path '/api/doctors'
Test-Endpoint -Method 'GET' -Path '/api/doctors/1'

Test-Endpoint -Method 'GET' -Path '/api/appointments'

Test-Endpoint -Method 'GET' -Path '/api/medicalrecords/patient/1'

Test-Endpoint -Method 'GET' -Path '/api/prescriptions/patient/1'

Test-Endpoint -Method 'GET' -Path '/api/bills/patient/1'

Test-Endpoint -Method 'GET' -Path '/api/departments'
Test-Endpoint -Method 'GET' -Path '/api/departments/1'

Test-Endpoint -Method 'GET' -Path '/api/medicines'
Test-Endpoint -Method 'GET' -Path '/api/medicines/1'

Test-Endpoint -Method 'GET' -Path '/api/rooms'
Test-Endpoint -Method 'GET' -Path '/api/rooms/1'

Test-Endpoint -Method 'GET' -Path '/api/schedules'

# Dashboard
Test-Endpoint -Method 'GET' -Path '/api/dashboard/stats'
Test-Endpoint -Method 'GET' -Path '/api/dashboard/recent-appointments?count=10'
Test-Endpoint -Method 'GET' -Path '/api/dashboard/revenue'

Write-Host "Done." -ForegroundColor Cyan


