using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all schedules (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetSchedules()
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");

                IEnumerable<ScheduleDto> schedules;

                if (userRole == "Admin" || userRole == "Staff")
                {
                    // Admin and Staff see all schedules
                    schedules = await _scheduleService.GetAllSchedulesAsync();
                }
                else if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    // Doctor sees only their schedules
                    schedules = await _scheduleService.GetSchedulesByDoctorAsync(doctorId);
                }
                else
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view schedules" });
                }

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules");
                return StatusCode(500, "An error occurred while retrieving schedules");
            }
        }

        /// <summary>
        /// Get schedule by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ScheduleDto>> GetSchedule(int id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (schedule == null)
                    return NotFound($"Schedule with ID {id} not found");

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedule with ID {ScheduleId}", id);
                return StatusCode(500, "An error occurred while retrieving the schedule");
            }
        }

        /// <summary>
        /// Get schedules by doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetSchedulesByDoctor(int doctorId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByDoctorAsync(doctorId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving schedules");
            }
        }

        /// <summary>
        /// Create a new schedule
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule(CreateScheduleDto createScheduleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var schedule = await _scheduleService.CreateScheduleAsync(createScheduleDto);
                return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, schedule);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule");
                return StatusCode(500, "An error occurred while creating the schedule");
            }
        }

        /// <summary>
        /// Update an existing schedule
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ScheduleDto>> UpdateSchedule(int id, UpdateScheduleDto updateScheduleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var schedule = await _scheduleService.UpdateScheduleAsync(id, updateScheduleDto);
                if (schedule == null)
                    return NotFound($"Schedule with ID {id} not found");

                return Ok(schedule);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule with ID {ScheduleId}", id);
                return StatusCode(500, "An error occurred while updating the schedule");
            }
        }

        /// <summary>
        /// Delete a schedule
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> DeleteSchedule(int id)
        {
            try
            {
                var result = await _scheduleService.DeleteScheduleAsync(id);
                if (!result)
                    return NotFound($"Schedule with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule with ID {ScheduleId}", id);
                return StatusCode(500, "An error occurred while deleting the schedule");
            }
        }

        /// <summary>
        /// Check if schedule exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> ScheduleExists(int id)
        {
            try
            {
                var exists = await _scheduleService.ScheduleExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if schedule exists with ID {ScheduleId}", id);
                return StatusCode(500, "An error occurred while checking schedule existence");
            }
        }
    }
}
