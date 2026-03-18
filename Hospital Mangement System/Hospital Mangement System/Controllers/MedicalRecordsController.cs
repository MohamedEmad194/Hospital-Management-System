using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService, ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _logger = logger;
        }

        /// <summary>
        /// Get all medical records (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecords()
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                IEnumerable<MedicalRecordDto> records;

                if (userRole == "Admin" || userRole == "Staff")
                {
                    // Admin and Staff see all records
                    records = await _medicalRecordService.GetAllMedicalRecordsAsync();
                }
                else if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    // Doctor sees only records they created
                    records = await _medicalRecordService.GetMedicalRecordsByDoctorAsync(doctorId);
                }
                else if (userRole == "Patient" && !string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId))
                {
                    // Patient sees only their own records
                    records = await _medicalRecordService.GetMedicalRecordsByPatientAsync(patientId);
                }
                else
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view medical records" });
                }

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records");
                return StatusCode(500, "An error occurred while retrieving medical records");
            }
        }

        /// <summary>
        /// Get medical record by ID (filtered by role)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int id)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
                if (record == null)
                    return NotFound($"Medical record with ID {id} not found");

                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole != "Admin" && userRole != "Staff")
                {
                    if (userRole == "Doctor" && (!string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId)))
                    {
                        if (record.DoctorId != doctorId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own medical records" });
                    }
                    else if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                    {
                        if (record.PatientId != patientId)
                            return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own medical records" });
                    }
                    else
                    {
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view this medical record" });
                    }
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical record with ID {RecordId}", id);
                return StatusCode(500, "An error occurred while retrieving the medical record");
            }
        }

        /// <summary>
        /// Get medical records by patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByPatient(int patientId)
        {
            try
            {
                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int loggedInPatientId)))
                {
                    if (patientId != loggedInPatientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own medical records" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view medical records" });
                }

                var records = await _medicalRecordService.GetMedicalRecordsByPatientAsync(patientId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while retrieving medical records");
            }
        }

        /// <summary>
        /// Get medical records by doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByDoctor(int doctorId)
        {
            try
            {
                var records = await _medicalRecordService.GetMedicalRecordsByDoctorAsync(doctorId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving medical records");
            }
        }

        /// <summary>
        /// Create a new medical record
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(CreateMedicalRecordDto createMedicalRecordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var record = await _medicalRecordService.CreateMedicalRecordAsync(createMedicalRecordDto);
                return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.Id }, record);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical record");
                return StatusCode(500, "An error occurred while creating the medical record");
            }
        }

        /// <summary>
        /// Update an existing medical record
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(int id, UpdateMedicalRecordDto updateMedicalRecordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check permissions - Admin and Staff can update any record, Doctor can only update their own
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var doctorIdClaim = User.FindFirstValue("DoctorId");

                if (userRole == "Doctor" && !string.IsNullOrEmpty(doctorIdClaim) && int.TryParse(doctorIdClaim, out int doctorId))
                {
                    var existingRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
                    if (existingRecord == null)
                        return NotFound($"Medical record with ID {id} not found");

                    if (existingRecord.DoctorId != doctorId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only update your own medical records" });
                }

                var record = await _medicalRecordService.UpdateMedicalRecordAsync(id, updateMedicalRecordDto);
                if (record == null)
                    return NotFound($"Medical record with ID {id} not found");

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical record with ID {RecordId}", id);
                return StatusCode(500, "An error occurred while updating the medical record");
            }
        }

        /// <summary>
        /// Delete a medical record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMedicalRecord(int id)
        {
            try
            {
                var result = await _medicalRecordService.DeleteMedicalRecordAsync(id);
                if (!result)
                    return NotFound($"Medical record with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record with ID {RecordId}", id);
                return StatusCode(500, "An error occurred while deleting the medical record");
            }
        }

        /// <summary>
        /// Check if medical record exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> MedicalRecordExists(int id)
        {
            try
            {
                var exists = await _medicalRecordService.MedicalRecordExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if medical record exists with ID {RecordId}", id);
                return StatusCode(500, "An error occurred while checking medical record existence");
            }
        }
    }
}
