using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.Models;
using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Controllers
{
    /// <summary>
    /// Quick Test Controller - For testing login quickly
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QuickTestController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly HospitalDbContext _context;
        private readonly ILogger<QuickTestController> _logger;

        public QuickTestController(
            UserManager<User> userManager,
            HospitalDbContext context,
            ILogger<QuickTestController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Quick test login - Test if credentials work
        /// </summary>
        [HttpPost("test-login")]
        [AllowAnonymous]
        public async Task<ActionResult> TestLogin([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                
                if (user == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "User not found",
                        email = loginDto.Email,
                        role = loginDto.Role,
                        suggestion = "Call POST /api/TestCredentials/ensure-users to create user accounts"
                    });
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    success = passwordValid && roles.Contains(loginDto.Role),
                    email = loginDto.Email,
                    role = loginDto.Role,
                    userExists = true,
                    passwordValid = passwordValid,
                    userRoles = roles,
                    hasRequestedRole = roles.Contains(loginDto.Role),
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get quick login credentials (first available of each type)
        /// </summary>
        [HttpGet("quick-credentials")]
        [AllowAnonymous]
        public async Task<ActionResult> GetQuickCredentials()
        {
            try
            {
                var admin = await _userManager.FindByEmailAsync("admin@hospital.com");
                var firstDoctor = await _context.Doctors
                    .Where(d => !d.IsDeleted)
                    .FirstOrDefaultAsync();
                var firstPatient = await _context.Patients
                    .Where(p => !p.IsDeleted)
                    .FirstOrDefaultAsync();
                var firstStaff = await _context.Staff
                    .Where(s => !s.IsDeleted)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    admin = admin != null ? new
                    {
                        email = "admin@hospital.com",
                        password = "Admin@123",
                        role = "Admin",
                        exists = true
                    } : new { email = "admin@hospital.com", password = "Admin@123", role = "Admin", exists = false },
                    
                    doctor = firstDoctor != null ? new
                    {
                        email = firstDoctor.Email,
                        password = "Doctor@123",
                        role = "Doctor",
                        name = $"{firstDoctor.FirstName} {firstDoctor.LastName}",
                        exists = await _userManager.FindByEmailAsync(firstDoctor.Email) != null
                    } : null,
                    
                    patient = firstPatient != null ? new
                    {
                        email = firstPatient.Email,
                        password = "Patient@123",
                        role = "Patient",
                        name = $"{firstPatient.FirstName} {firstPatient.LastName}",
                        exists = await _userManager.FindByEmailAsync(firstPatient.Email) != null
                    } : null,
                    
                    staff = firstStaff != null ? new
                    {
                        email = firstStaff.Email,
                        password = "Staff@123",
                        role = "Nurse",
                        name = $"{firstStaff.FirstName} {firstStaff.LastName}",
                        exists = await _userManager.FindByEmailAsync(firstStaff.Email) != null
                    } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

