using Microsoft.AspNetCore.Mvc;
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
        /// Get all prescriptions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptions()
        {
            try
            {
                var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions");
                return StatusCode(500, "An error occurred while retrieving prescriptions");
            }
        }

        /// <summary>
        /// Get prescription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDto>> GetPrescription(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
                if (prescription == null)
                    return NotFound($"Prescription with ID {id} not found");

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
        public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetPrescriptionsByPatient(int patientId)
        {
            try
            {
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
        public async Task<ActionResult<PrescriptionDto>> UpdatePrescription(int id, UpdatePrescriptionDto updatePrescriptionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
