using Microsoft.AspNetCore.Mvc;
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
        /// Get all appointments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found");

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
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByPatient(int patientId)
        {
            try
            {
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
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
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
        public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
        public async Task<ActionResult> CancelAppointment(int id)
        {
            try
            {
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
        /// Delete an appointment
        /// </summary>
        [HttpDelete("{id}")]
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
