using Microsoft.AspNetCore.Mvc;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        /// <summary>
        /// Get all doctors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors");
                return StatusCode(500, "An error occurred while retrieving doctors");
            }
        }

        /// <summary>
        /// Get doctor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        /// <summary>
        /// Get doctor by National ID
        /// </summary>
        [HttpGet("national-id/{nationalId}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByNationalId(string nationalId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByNationalIdAsync(nationalId);
                if (doctor == null)
                    return NotFound($"Doctor with National ID {nationalId} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor with National ID {NationalId}", nationalId);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        /// <summary>
        /// Get doctor by License Number
        /// </summary>
        [HttpGet("license/{licenseNumber}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByLicenseNumber(string licenseNumber)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByLicenseNumberAsync(licenseNumber);
                if (doctor == null)
                    return NotFound($"Doctor with License Number {licenseNumber} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor with License Number {LicenseNumber}", licenseNumber);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        /// <summary>
        /// Get doctors by department
        /// </summary>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsByDepartment(int departmentId)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsByDepartmentAsync(departmentId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for department {DepartmentId}", departmentId);
                return StatusCode(500, "An error occurred while retrieving doctors");
            }
        }

        /// <summary>
        /// Get available doctors
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAvailableDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAvailableDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available doctors");
                return StatusCode(500, "An error occurred while retrieving available doctors");
            }
        }

        /// <summary>
        /// Search doctors
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> SearchDoctors([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var doctors = await _doctorService.SearchDoctorsAsync(searchTerm);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching doctors");
            }
        }

        /// <summary>
        /// Create a new doctor
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> CreateDoctor(CreateDoctorDto createDoctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var doctor = await _doctorService.CreateDoctorAsync(createDoctorDto);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An error occurred while creating the doctor");
            }
        }

        /// <summary>
        /// Update an existing doctor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(int id, UpdateDoctorDto updateDoctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var doctor = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                return Ok(doctor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor with ID {DoctorId}", id);
                return StatusCode(500, "An error occurred while updating the doctor");
            }
        }

        /// <summary>
        /// Delete a doctor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            try
            {
                var result = await _doctorService.DeleteDoctorAsync(id);
                if (!result)
                    return NotFound($"Doctor with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor with ID {DoctorId}", id);
                return StatusCode(500, "An error occurred while deleting the doctor");
            }
        }

        /// <summary>
        /// Check if doctor exists
        /// </summary>
        [HttpHead("{id}")]
        public async Task<ActionResult> DoctorExists(int id)
        {
            try
            {
                var exists = await _doctorService.DoctorExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if doctor exists with ID {DoctorId}", id);
                return StatusCode(500, "An error occurred while checking doctor existence");
            }
        }
    }
}
