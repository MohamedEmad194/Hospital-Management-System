using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all departments
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, "An error occurred while retrieving departments");
            }
        }

        /// <summary>
        /// Get department by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);
                if (department == null)
                    return NotFound($"Department with ID {id} not found");

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
                return StatusCode(500, "An error occurred while retrieving the department");
            }
        }

        /// <summary>
        /// Create a new department
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto createDepartmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var department = await _departmentService.CreateDepartmentAsync(createDepartmentDto);
                return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, "An error occurred while creating the department");
            }
        }

        /// <summary>
        /// Update an existing department
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var department = await _departmentService.UpdateDepartmentAsync(id, updateDepartmentDto);
                if (department == null)
                    return NotFound($"Department with ID {id} not found");

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department with ID {DepartmentId}", id);
                return StatusCode(500, "An error occurred while updating the department");
            }
        }

        /// <summary>
        /// Delete a department
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(id);
                if (!result)
                    return NotFound($"Department with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department with ID {DepartmentId}", id);
                return StatusCode(500, "An error occurred while deleting the department");
            }
        }

        /// <summary>
        /// Check if department exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> DepartmentExists(int id)
        {
            try
            {
                var exists = await _departmentService.DepartmentExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if department exists with ID {DepartmentId}", id);
                return StatusCode(500, "An error occurred while checking department existence");
            }
        }
    }
}
