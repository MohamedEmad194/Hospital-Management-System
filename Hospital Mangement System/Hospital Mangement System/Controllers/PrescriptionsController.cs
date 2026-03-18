using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(IPrescriptionService prescriptionService, ILogger<PrescriptionsController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all prescriptions (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptions()
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                IEnumerable<PrescriptionDto> prescriptions;

                if (userRole == "Admin" || userRole == "Staff")
                {
                    // Admin and Staff see all prescriptions
                    prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
                }
                else if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    // Doctor sees only prescriptions they created
                    prescriptions = await _prescriptionService.GetPrescriptionsByDoctorAsync(doctorId);
                }
                else if (userRole == "Patient" && !string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId))
                {
                    // Patient sees only their own prescriptions
                    prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId);
                }
                else
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view prescriptions" });
                }

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions");
                return StatusCode(500, "An error occurred while retrieving prescriptions");
            }
        }

        /// <summary>
        /// Get prescription by ID (filtered by role)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PrescriptionDto>> GetPrescription(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
                if (prescription == null)
                    return NotFound($"Prescription with ID {id} not found");

                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId)))
                    {
                        if (prescription.DoctorId != doctorId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own prescriptions" });
                    }
                    else if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                    {
                        if (prescription.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own prescriptions" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view this prescription" });
                    }
                }

                return Ok(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescription with ID {PrescriptionId}", id);
                return StatusCode(500, "An error occurred while retrieving the prescription");
            }
        }

        /// <summary>
        /// Get prescriptions by patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptionsByPatient(int patientId)
        {
            try
            {
                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int loggedInPatientId)))
                {
                    if (patientId != loggedInPatientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own prescriptions" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view prescriptions" });
                }

                var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while retrieving prescriptions");
            }
        }

        /// <summary>
        /// Get prescriptions by doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptionsByDoctor(int doctorId)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByDoctorAsync(doctorId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving prescriptions");
            }
        }

        /// <summary>
        /// Create a new prescription
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<PrescriptionDto>> CreatePrescription(CreatePrescriptionDto createPrescriptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var prescription = await _prescriptionService.CreatePrescriptionAsync(createPrescriptionDto);
                return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, "An error occurred while creating the prescription");
            }
        }

        /// <summary>
        /// Update an existing prescription
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<PrescriptionDto>> UpdatePrescription(int id, UpdatePrescriptionDto updatePrescriptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check permissions - Admin and Staff can update any prescription, Doctor can only update their own
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");

                if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    var existingPrescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
                    if (existingPrescription == null)
                        return NotFound($"Prescription with ID {id} not found");

                    if (existingPrescription.DoctorId != doctorId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only update your own prescriptions" });
                }

                var prescription = await _prescriptionService.UpdatePrescriptionAsync(id, updatePrescriptionDto);
                if (prescription == null)
                    return NotFound($"Prescription with ID {id} not found");

                return Ok(prescription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prescription with ID {PrescriptionId}", id);
                return StatusCode(500, "An error occurred while updating the prescription");
            }
        }

        /// <summary>
        /// Dispense a prescription
        /// </summary>
        [HttpPut("{id}/dispense")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> DispensePrescription(int id)
        {
            try
            {
                var result = await _prescriptionService.DispensePrescriptionAsync(id);
                if (!result)
                    return NotFound($"Prescription with ID {id} not found");

                return Ok(new { message = "Prescription dispensed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispensing prescription with ID {PrescriptionId}", id);
                return StatusCode(500, "An error occurred while dispensing the prescription");
            }
        }

        /// <summary>
        /// Delete a prescription
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePrescription(int id)
        {
            try
            {
                var result = await _prescriptionService.DeletePrescriptionAsync(id);
                if (!result)
                    return NotFound($"Prescription with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription with ID {PrescriptionId}", id);
                return StatusCode(500, "An error occurred while deleting the prescription");
            }
        }

        /// <summary>
        /// Check if prescription exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> PrescriptionExists(int id)
        {
            try
            {
                var exists = await _prescriptionService.PrescriptionExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if prescription exists with ID {PrescriptionId}", id);
                return StatusCode(500, "An error occurred while checking prescription existence");
            }
        }
    }
}
