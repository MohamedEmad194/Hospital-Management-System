using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all appointments (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            try
            {
                // Check if user is authenticated - [Authorize] should handle this, but double-check
                if (User?.Identity == null || !User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthenticated request to GetAppointments - User.Identity is null or not authenticated");
                    return StatusCode(401, new { message = "Authentication required", details = "Please login to access this resource" });
                }

                // Get current user role
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                _logger.LogInformation("GetAppointments called - Role: {Role}, DoctorId: {DoctorId}, PatientId: {PatientId}", 
                    userRole, doctorIdClaim, patientIdClaim);

                IEnumerable<AppointmentDto> appointments;

                if (userRole == "Admin")
                {
                    // Admin sees all appointments
                    appointments = await _appointmentService.GetAllAppointmentsAsync();
                }
                else if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    // Doctor sees only their appointments
                    appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                }
                else if (userRole == "Patient" && !string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId))
                {
                    // Patient sees only their appointments
                    appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
                }
                else if (userRole == "Staff")
                {
                    // Staff sees all appointments (read-only)
                    appointments = await _appointmentService.GetAllAppointmentsAsync();
                }
                else
                {
                    _logger.LogWarning("Insufficient permissions - Role: {Role}, DoctorId: {DoctorId}, PatientId: {PatientId}", 
                        userRole, doctorIdClaim, patientIdClaim);
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view appointments" });
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return StatusCode(500, new 
                { 
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    details = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Get appointment by ID (filtered by role)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found");

                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId)))
                    {
                        if (appointment.DoctorId != doctorId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own appointments" });
                    }
                    else if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                    {
                        if (appointment.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own appointments" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view this appointment" });
                    }
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while retrieving the appointment");
            }
        }

        /// <summary>
        /// Get appointments by patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByPatient(int patientId)
        {
            try
            {
                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int loggedInPatientId)))
                {
                    if (patientId != loggedInPatientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own appointments" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view appointments" });
                }

                var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Get appointments by doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");

                if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int loggedInDoctorId)))
                {
                    if (doctorId != loggedInDoctorId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own appointments" });
                }
                else if (userRole != "Admin" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view appointments" });
                }

                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Get appointments by date
        /// </summary>
        [HttpGet("date/{date}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDate(DateTime date)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDateAsync(date);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for date {Date}", date);
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Get appointments by date range
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for date range {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Search appointments
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> SearchAppointments([FromQuery] AppointmentSearchDto searchDto)
        {
            try
            {
                var appointments = await _appointmentService.SearchAppointmentsAsync(searchDto);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching appointments");
                return StatusCode(500, "An error occurred while searching appointments");
            }
        }

        /// <summary>
        /// Get available time slots for a doctor on a specific date
        /// </summary>
        [HttpGet("available-slots/{doctorId}/{date}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TimeSpan>>> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            try
            {
                var timeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots for doctor {DoctorId} on {Date}", doctorId, date);
                return StatusCode(500, "An error occurred while retrieving available time slots");
            }
        }

        /// <summary>
        /// Create a new appointment
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check permissions - only Admin, Staff, and Patients can create appointments
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    // If Patient, they can only create appointments for themselves
                    if (userRole == "Patient" && !string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId))
                    {
                        if (createAppointmentDto.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only create appointments for yourself" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have permission to create appointments" });
                    }
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(createAppointmentDto);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while creating the appointment");
            }
        }

        /// <summary>
        /// Update an existing appointment
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if appointment exists and get it
                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (existingAppointment == null)
                    return NotFound($"Appointment with ID {id} not found");

                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId)))
                    {
                        if (existingAppointment.DoctorId != doctorId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only update your own appointments" });
                    }
                    else if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                    {
                        if (existingAppointment.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only update your own appointments" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have permission to update appointments" });
                    }
                }

                var appointment = await _appointmentService.UpdateAppointmentAsync(id, updateAppointmentDto);
                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found");

                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while updating the appointment");
            }
        }

        /// <summary>
        /// Cancel an appointment
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult> CancelAppointment(int id)
        {
            try
            {
                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                    if (appointment == null)
                        return NotFound($"Appointment with ID {id} not found");

                    if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId)))
                    {
                        if (appointment.DoctorId != doctorId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only cancel your own appointments" });
                    }
                    else if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                    {
                        if (appointment.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only cancel your own appointments" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have permission to cancel appointments" });
                    }
                }

                var result = await _appointmentService.CancelAppointmentAsync(id);
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return Ok(new { message = "Appointment cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while cancelling the appointment");
            }
        }

        /// <summary>
        /// Complete an appointment
        /// </summary>
        [HttpPut("{id}/complete")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult> CompleteAppointment(int id, [FromBody] CompleteAppointmentRequest request)
        {
            try
            {
                var result = await _appointmentService.CompleteAppointmentAsync(id, request.Diagnosis, request.Treatment);
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return Ok(new { message = "Appointment completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing appointment with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while completing the appointment");
            }
        }

        /// <summary>
        /// Delete an appointment (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            try
            {
                var result = await _appointmentService.DeleteAppointmentAsync(id);
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while deleting the appointment");
            }
        }

        /// <summary>
        /// Check if appointment exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> AppointmentExists(int id)
        {
            try
            {
                var exists = await _appointmentService.AppointmentExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if appointment exists with ID {AppointmentId}", id);
                return StatusCode(500, "An error occurred while checking appointment existence");
            }
        }
    }

    public class CompleteAppointmentRequest
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
}
