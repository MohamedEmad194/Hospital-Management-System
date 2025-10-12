using Microsoft.AspNetCore.Mvc;
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
        /// Get all medical records
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecords()
        {
            try
            {
                var records = await _medicalRecordService.GetAllMedicalRecordsAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records");
                return StatusCode(500, "An error occurred while retrieving medical records");
            }
        }

        /// <summary>
        /// Get medical record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(int id)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
                if (record == null)
                    return NotFound($"Medical record with ID {id} not found");

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
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByPatient(int patientId)
        {
            try
            {
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
        public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(int id, UpdateMedicalRecordDto updateMedicalRecordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
