using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly ILogger<FeaturesController> _logger;

        public FeaturesController(HospitalDbContext context, ILogger<FeaturesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active features (public endpoint)
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFeatures([FromQuery] string? lang = "en")
        {
            try
            {
                // Check if database is accessible
                if (!await _context.Database.CanConnectAsync())
                {
                    _logger.LogWarning("Database connection failed");
                    return Ok(new List<object>()); // Return empty array - frontend will use fallback
                }

                // Try to get features - if table doesn't exist, return empty array
                try
                {
                    var features = await _context.Features
                        .Where(f => !f.IsDeleted && f.IsActive)
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new
                        {
                            id = f.Id,
                            icon = f.Icon,
                            title = lang == "ar" ? f.TitleAr : f.TitleEn,
                            desc = lang == "ar" ? f.DescriptionAr : f.DescriptionEn,
                            color = f.Color,
                            displayOrder = f.DisplayOrder
                        })
                        .ToListAsync();

                    return Ok(features);
                }
                catch (Exception dbEx)
                {
                    // Check if it's a table not found error
                    var errorMessage = dbEx.Message.ToLower();
                    if (errorMessage.Contains("invalid object name") || 
                        errorMessage.Contains("does not exist") ||
                        errorMessage.Contains("cannot find the object") ||
                        errorMessage.Contains("table") && errorMessage.Contains("not found"))
                    {
                        _logger.LogWarning("Features table does not exist. Returning empty array. Please run migration: dotnet ef migrations add AddFeaturesTable && dotnet ef database update");
                        return Ok(new List<object>()); // Return empty array - frontend will use fallback
                    }
                    // Re-throw if it's a different error
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving features: {ErrorMessage}", ex.Message);
                // Return empty array instead of error - frontend will use fallback data
                return Ok(new List<object>());
            }
        }

        /// <summary>
        /// Get feature by ID (public endpoint)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFeature(int id, [FromQuery] string? lang = "en")
        {
            try
            {
                // Check if database is accessible
                if (!await _context.Database.CanConnectAsync())
                {
                    _logger.LogWarning("Database connection failed");
                    return StatusCode(503, new { message = "Database is not available" });
                }

                try
                {
                    var feature = await _context.Features
                        .Where(f => f.Id == id && !f.IsDeleted)
                        .Select(f => new
                        {
                            id = f.Id,
                            icon = f.Icon,
                            title = lang == "ar" ? f.TitleAr : f.TitleEn,
                            desc = lang == "ar" ? f.DescriptionAr : f.DescriptionEn,
                            color = f.Color,
                            displayOrder = f.DisplayOrder
                        })
                        .FirstOrDefaultAsync();

                    if (feature == null)
                        return NotFound($"Feature with ID {id} not found");

                    return Ok(feature);
                }
                catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Message.Contains("Invalid object name") || sqlEx.Message.Contains("does not exist"))
                {
                    _logger.LogWarning("Features table does not exist");
                    return NotFound($"Feature with ID {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feature with ID {FeatureId}: {ErrorMessage}", id, ex.Message);
                return StatusCode(500, new { message = "An error occurred while retrieving the feature", error = ex.Message });
            }
        }
    }
}

