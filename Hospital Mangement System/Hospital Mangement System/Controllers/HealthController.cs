using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Microsoft.Data.SqlClient;

namespace Hospital_Management_System.Controllers;

/// <summary>
/// Lightweight checks for deployment (no secrets returned).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HospitalDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HospitalDbContext db, IConfiguration configuration, ILogger<HealthController> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Verifies SQL Server connectivity using the configured connection string.
    /// </summary>
    [HttpGet("database")]
    [AllowAnonymous]
    public async Task<IActionResult> Database(CancellationToken cancellationToken)
    {
        var cs = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cs))
        {
            return StatusCode(503, new
            {
                status = "misconfigured",
                message = "ConnectionStrings:DefaultConnection is missing or empty.",
                hint = "Option A: set Application Setting ConnectionStrings__DefaultConnection to the full string from your SQL panel. Option B: set Database__Server, Database__Name, Database__User, Database__Password (and optional Database__Encrypt) — the app builds the connection string safely for special characters in the password."
            });
        }

        try
        {
            var ok = await _db.Database.CanConnectAsync(cancellationToken);
            return Ok(new { status = ok ? "ok" : "failed", databaseReachable = ok });
        }
        catch (SqlException ex)
        {
            _logger.LogWarning(ex, "Health database check failed (SQL)");
            return StatusCode(503, new
            {
                status = "error",
                message = "Cannot connect to SQL Server.",
                sqlErrorNumber = ex.Number,
                hint = "Verify ConnectionStrings__DefaultConnection or Database__* settings on the host. Try Encrypt=true;TrustServerCertificate=true if TLS is required. If the hostname fails, try Server=db41886.public.databaseasp.net."
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health database check failed");
            return StatusCode(503, new
            {
                status = "error",
                message = "Database check failed.",
                hint = "See server logs. Verify ConnectionStrings__DefaultConnection on the host."
            });
        }
    }
}
