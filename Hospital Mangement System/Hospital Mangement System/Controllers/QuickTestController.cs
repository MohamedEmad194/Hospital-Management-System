using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Controllers
{
    /// <summary>
    /// Development-only login diagnostics. Does not expose passwords.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QuickTestController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly HospitalDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public QuickTestController(
            UserManager<User> userManager,
            HospitalDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _context = context;
            _environment = environment;
        }

        private ActionResult? RejectIfNotDevelopment()
        {
            if (!_environment.IsDevelopment())
                return NotFound(new { message = "This endpoint is only available in Development." });
            return null;
        }

        [HttpPost("test-login")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TestLogin([FromBody] LoginDto loginDto)
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "User not found",
                    email = loginDto.Email,
                    role = loginDto.Role
                });
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                success = passwordValid && roles.Contains(loginDto.Role),
                email = loginDto.Email,
                role = loginDto.Role,
                passwordValid,
                userRoles = roles,
                hasRequestedRole = roles.Contains(loginDto.Role),
                isActive = user.IsActive
            });
        }

        [HttpGet("quick-credentials")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetQuickCredentials()
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            var admin = await _userManager.FindByEmailAsync("admin@hospital.com");
            var firstDoctor = await _context.Doctors.Where(d => !d.IsDeleted).FirstOrDefaultAsync();
            var firstPatient = await _context.Patients.Where(p => !p.IsDeleted).FirstOrDefaultAsync();

            return Ok(new
            {
                note = "Emails only — passwords are hashed in the database.",
                admin = new { email = "admin@hospital.com", role = "Admin", exists = admin != null },
                doctor = firstDoctor == null ? null : new
                {
                    email = firstDoctor.Email,
                    role = "Doctor",
                    exists = await _userManager.FindByEmailAsync(firstDoctor.Email) != null
                },
                patient = firstPatient == null ? null : new
                {
                    email = firstPatient.Email,
                    role = "Patient",
                    exists = await _userManager.FindByEmailAsync(firstPatient.Email) != null
                }
            });
        }
    }
}
