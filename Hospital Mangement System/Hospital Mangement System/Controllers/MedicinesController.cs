using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineService _medicineService;
        private readonly ILogger<MedicinesController> _logger;

        public MedicinesController(IMedicineService medicineService, ILogger<MedicinesController> logger)
        {
            _medicineService = medicineService;
            _logger = logger;
        }

        /// <summary>
        /// Get all medicines
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetMedicines()
        {
            try
            {
                var medicines = await _medicineService.GetAllMedicinesAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medicines");
                return StatusCode(500, "An error occurred while retrieving medicines");
            }
        }

        /// <summary>
        /// Get medicine by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MedicineDto>> GetMedicine(int id)
        {
            try
            {
                var medicine = await _medicineService.GetMedicineByIdAsync(id);
                if (medicine == null)
                    return NotFound($"Medicine with ID {id} not found");

                return Ok(medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medicine with ID {MedicineId}", id);
                return StatusCode(500, "An error occurred while retrieving the medicine");
            }
        }

        /// <summary>
        /// Search medicines
        /// </summary>
        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> SearchMedicines([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var medicines = await _medicineService.SearchMedicinesAsync(searchTerm);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching medicines with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching medicines");
            }
        }

        /// <summary>
        /// Get low stock medicines
        /// </summary>
        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetLowStockMedicines()
        {
            try
            {
                var medicines = await _medicineService.GetLowStockMedicinesAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock medicines");
                return StatusCode(500, "An error occurred while retrieving low stock medicines");
            }
        }

        /// <summary>
        /// Create a new medicine
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<MedicineDto>> CreateMedicine(CreateMedicineDto createMedicineDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var medicine = await _medicineService.CreateMedicineAsync(createMedicineDto);
                return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medicine");
                return StatusCode(500, "An error occurred while creating the medicine");
            }
        }

        /// <summary>
        /// Update an existing medicine
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<MedicineDto>> UpdateMedicine(int id, UpdateMedicineDto updateMedicineDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var medicine = await _medicineService.UpdateMedicineAsync(id, updateMedicineDto);
                if (medicine == null)
                    return NotFound($"Medicine with ID {id} not found");

                return Ok(medicine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medicine with ID {MedicineId}", id);
                return StatusCode(500, "An error occurred while updating the medicine");
            }
        }

        /// <summary>
        /// Delete a medicine
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMedicine(int id)
        {
            try
            {
                var result = await _medicineService.DeleteMedicineAsync(id);
                if (!result)
                    return NotFound($"Medicine with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medicine with ID {MedicineId}", id);
                return StatusCode(500, "An error occurred while deleting the medicine");
            }
        }

        /// <summary>
        /// Check if medicine exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> MedicineExists(int id)
        {
            try
            {
                var exists = await _medicineService.MedicineExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if medicine exists with ID {MedicineId}", id);
                return StatusCode(500, "An error occurred while checking medicine existence");
            }
        }
    }
}
