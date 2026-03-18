using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;
using Hospital_Management_System.Data;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NursingUnitsController : ControllerBase
    {
        private readonly INursingUnitService _nursingUnitService;
        private readonly ILogger<NursingUnitsController> _logger;
        private readonly HospitalDbContext _context;

        public NursingUnitsController(
            INursingUnitService nursingUnitService, 
            ILogger<NursingUnitsController> logger,
            HospitalDbContext context)
        {
            _nursingUnitService = nursingUnitService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get all nursing units
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<NursingUnitDto>>> GetNursingUnits()
        {
            try
            {
                var units = await _nursingUnitService.GetAllNursingUnitsAsync();
                _logger.LogInformation("Retrieved {Count} nursing units", units.Count());
                return Ok(units);
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 208 || sqlEx.Message.Contains("Invalid object name"))
            {
                _logger.LogError(sqlEx, "NursingUnits table does not exist. Please create a migration and update the database.");
                return StatusCode(500, new 
                { 
                    message = "NursingUnits table does not exist", 
                    details = "Please create a migration for NursingUnits and update the database. Run: dotnet ef migrations add AddNursingUnits && dotnet ef database update",
                    error = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nursing units: {Error}", ex.Message);
                return StatusCode(500, new 
                { 
                    message = "An error occurred while retrieving nursing units",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Get nursing unit by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NursingUnitDto>> GetNursingUnit(int id)
        {
            try
            {
                var unit = await _nursingUnitService.GetNursingUnitByIdAsync(id);
                if (unit == null)
                    return NotFound($"Nursing unit with ID {id} not found");

                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nursing unit with ID {NursingUnitId}", id);
                return StatusCode(500, "An error occurred while retrieving the nursing unit");
            }
        }

        /// <summary>
        /// Get nursing unit by UnitId
        /// </summary>
        [HttpGet("unit-id/{unitId}")]
        public async Task<ActionResult<NursingUnitDto>> GetNursingUnitByUnitId(string unitId)
        {
            try
            {
                var unit = await _nursingUnitService.GetNursingUnitByUnitIdAsync(unitId);
                if (unit == null)
                    return NotFound($"Nursing unit with UnitId {unitId} not found");

                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nursing unit with UnitId {UnitId}", unitId);
                return StatusCode(500, "An error occurred while retrieving the nursing unit");
            }
        }

        /// <summary>
        /// Search nursing units
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<NursingUnitDto>>> SearchNursingUnits([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var units = await _nursingUnitService.SearchNursingUnitsAsync(searchTerm);
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching nursing units with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching nursing units");
            }
        }

        /// <summary>
        /// Create a new nursing unit (Admin only)
        /// </summary>
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult<NursingUnitDto>> CreateNursingUnit(CreateNursingUnitDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var unit = await _nursingUnitService.CreateNursingUnitAsync(createDto);
                return CreatedAtAction(nameof(GetNursingUnit), new { id = unit.Id }, unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating nursing unit");
                return StatusCode(500, "An error occurred while creating the nursing unit");
            }
        }

        /// <summary>
        /// Update an existing nursing unit (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult<NursingUnitDto>> UpdateNursingUnit(int id, UpdateNursingUnitDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var unit = await _nursingUnitService.UpdateNursingUnitAsync(id, updateDto);
                if (unit == null)
                    return NotFound($"Nursing unit with ID {id} not found");

                return Ok(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating nursing unit with ID {NursingUnitId}", id);
                return StatusCode(500, "An error occurred while updating the nursing unit");
            }
        }

        /// <summary>
        /// Delete a nursing unit (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteNursingUnit(int id)
        {
            try
            {
                var result = await _nursingUnitService.DeleteNursingUnitAsync(id);
                if (!result)
                    return NotFound($"Nursing unit with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting nursing unit with ID {NursingUnitId}", id);
                return StatusCode(500, "An error occurred while deleting the nursing unit");
            }
        }

        /// <summary>
        /// Check if NursingUnits table exists (for troubleshooting)
        /// </summary>
        [HttpGet("check-table")]
        [AllowAnonymous]
        public async Task<ActionResult> CheckTableExists()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return Ok(new
                    {
                        databaseConnected = false,
                        message = "Cannot connect to database",
                        solution = "Check your connection string in appsettings.json"
                    });
                }

                // Try to query the table
                try
                {
                    var count = await _context.NursingUnits.CountAsync();
                    var activeCount = await _context.NursingUnits.CountAsync(n => !n.IsDeleted && n.IsActive);
                    
                    return Ok(new
                    {
                        databaseConnected = true,
                        tableExists = true,
                        recordCount = count,
                        activeRecordCount = activeCount,
                        message = count > 0 
                            ? $"NursingUnits table exists with {count} record(s), {activeCount} active"
                            : "NursingUnits table exists but is empty. Call POST /api/NursingUnits/seed to add data",
                        needsData = count == 0
                    });
                }
                catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 208 || sqlEx.Message.Contains("Invalid object name"))
                {
                    return Ok(new
                    {
                        databaseConnected = true,
                        tableExists = false,
                        message = "NursingUnits table does not exist",
                        solution = "1. Stop the server\n2. Run: dotnet ef database update\n3. Restart the server",
                        error = sqlEx.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking table existence");
                return StatusCode(500, new 
                { 
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Seed nursing units data (Allow anonymous for initial setup)
        /// </summary>
        [HttpPost("seed")]
        [AllowAnonymous]
        public async Task<ActionResult> SeedNursingUnits()
        {
            try
            {
                var result = await _nursingUnitService.SeedNursingUnitsAsync();
                if (!result)
                {
                    var count = await _context.NursingUnits.CountAsync(n => !n.IsDeleted);
                    return Ok(new 
                    { 
                        message = "Nursing units data already exists",
                        existingCount = count,
                        note = "Data is already seeded. Use DELETE /api/NursingUnits/clear-all to clear and reseed."
                    });
                }

                var addedCount = await _context.NursingUnits.CountAsync(n => !n.IsDeleted);
                return Ok(new 
                { 
                    message = "Nursing units data seeded successfully",
                    count = addedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding nursing units");
                return StatusCode(500, new 
                { 
                    message = "An error occurred while seeding nursing units",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Clear all nursing units data (for reseeding) - Admin only
        /// </summary>
        [HttpDelete("clear-all")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<ActionResult> ClearAllNursingUnits()
        {
            try
            {
                var units = await _context.NursingUnits.ToListAsync();
                _context.NursingUnits.RemoveRange(units);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "All nursing units cleared successfully",
                    deletedCount = units.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing nursing units");
                return StatusCode(500, new 
                { 
                    message = "An error occurred while clearing nursing units",
                    error = ex.Message
                });
            }
        }
    }
}

