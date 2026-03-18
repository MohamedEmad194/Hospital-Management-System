using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        /// <summary>
        /// Get all patients (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, "An error occurred while retrieving patients");
            }
        }

        /// <summary>
        /// Get patient by ID (filtered by role)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found");

                // Check permissions - Patient can only see their own data
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int loggedInPatientId)))
                {
                    if (patient.Id != loggedInPatientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own patient data" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view patient data" });
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient with ID {PatientId}", id);
                return StatusCode(500, "An error occurred while retrieving the patient");
            }
        }

        /// <summary>
        /// Get patient by National ID
        /// </summary>
        [HttpGet("national-id/{nationalId}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<PatientDto>> GetPatientByNationalId(string nationalId)
        {
            try
            {
                var patient = await _patientService.GetPatientByNationalIdAsync(nationalId);
                if (patient == null)
                    return NotFound($"Patient with National ID {nationalId} not found");

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient with National ID {NationalId}", nationalId);
                return StatusCode(500, "An error occurred while retrieving the patient");
            }
        }

        /// <summary>
        /// Search patients
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchPatients([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var patients = await _patientService.SearchPatientsAsync(searchTerm);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching patients with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching patients");
            }
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<PatientDto>> CreatePatient(CreatePatientDto createPatientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var patient = await _patientService.CreatePatientAsync(createPatientDto);
                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, "An error occurred while creating the patient");
            }
        }

        /// <summary>
        /// Update an existing patient
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, UpdatePatientDto updatePatientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check permissions - Admin, Doctor, Staff can update any patient, Patient can only update themselves
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int loggedInPatientId)))
                {
                    if (id != loggedInPatientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only update your own profile" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have permission to update patients" });
                }

                var patient = await _patientService.UpdatePatientAsync(id, updatePatientDto);
                if (patient == null)
                    return NotFound($"Patient with ID {id} not found");

                return Ok(patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with ID {PatientId}", id);
                return StatusCode(500, "An error occurred while updating the patient");
            }
        }

        /// <summary>
        /// Delete a patient
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(id);
                if (!result)
                    return NotFound($"Patient with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient with ID {PatientId}", id);
                return StatusCode(500, "An error occurred while deleting the patient");
            }
        }

        /// <summary>
        /// Check if patient exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> PatientExists(int id)
        {
            try
            {
                var exists = await _patientService.PatientExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if patient exists with ID {PatientId}", id);
                return StatusCode(500, "An error occurred while checking patient existence");
            }
        }
    }
}
