using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Configuration;
using Hospital_Management_System.Data;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Controllers
{
    /// <summary>
    /// Development-only helpers for linking entities to Identity users. Never exposes passwords.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestCredentialsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly HospitalDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TestCredentialsController> _logger;

        public TestCredentialsController(
            UserManager<User> userManager,
            HospitalDbContext context,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILogger<TestCredentialsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        private ActionResult? RejectIfNotDevelopment()
        {
            if (!_environment.IsDevelopment())
                return NotFound(new { message = "This endpoint is only available in Development." });
            return null;
        }

        [HttpPost("create-test-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateTestUser([FromBody] CreateTestUserDto dto)
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Role))
                    return BadRequest(new { message = "Email, Password, and Role are required" });

                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Ok(new
                    {
                        message = "User already exists",
                        email = dto.Email,
                        role = dto.Role,
                        created = false
                    });
                }

                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName ?? "Test",
                    LastName = dto.LastName ?? "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to create user",
                        errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var role = dto.Role.Trim();
                if (role == "Nurse") role = "Staff";
                await _userManager.AddToRoleAsync(user, role);

                return Ok(new
                {
                    message = "User created. Password is stored as a hash only.",
                    email = dto.Email,
                    role = dto.Role,
                    created = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test user");
                return StatusCode(500, new { message = "An error occurred while creating test user" });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllTestCredentials()
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            try
            {
                await EnsureUsersForEntitiesAsync();

                return Ok(new
                {
                    note = "Passwords are never returned. They are stored hashed in AspNetUsers.",
                    admin = new { Email = "admin@hospital.com", Role = "Admin", HasUserAccount = await _userManager.FindByEmailAsync("admin@hospital.com") != null },
                    doctors = await _context.Doctors.Where(d => !d.IsDeleted).Select(d => new
                    {
                        d.Email,
                        Role = "Doctor",
                        Name = $"{d.FirstName} {d.LastName}",
                        d.Specialization,
                        HasUserAccount = d.UserId != null
                    }).Take(20).ToListAsync(),
                    patients = await _context.Patients.Where(p => !p.IsDeleted).Select(p => new
                    {
                        p.Email,
                        Role = "Patient",
                        Name = $"{p.FirstName} {p.LastName}",
                        HasUserAccount = p.UserId != null
                    }).Take(20).ToListAsync(),
                    staff = await _context.Staff.Where(s => !s.IsDeleted).Select(s => new
                    {
                        s.Email,
                        Role = "Nurse",
                        Name = $"{s.FirstName} {s.LastName}",
                        HasUserAccount = s.UserId != null
                    }).Take(20).ToListAsync()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving test credentials");
                return StatusCode(500, "An error occurred while retrieving test credentials");
            }
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetCredentialsByRole(string role)
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            try
            {
                await EnsureUsersForEntitiesAsync();
                role = role.ToLowerInvariant();
                object? credentials = role switch
                {
                    "admin" => new { Email = "admin@hospital.com", Role = "Admin" },
                    "doctor" or "doctors" => await _context.Doctors.Where(d => !d.IsDeleted).Select(d => new
                    {
                        d.Email,
                        Role = "Doctor",
                        Name = $"{d.FirstName} {d.LastName}",
                        HasUserAccount = d.UserId != null
                    }).Take(20).ToListAsync(),
                    "patient" or "patients" => await _context.Patients.Where(p => !p.IsDeleted).Select(p => new
                    {
                        p.Email,
                        Role = "Patient",
                        Name = $"{p.FirstName} {p.LastName}",
                        HasUserAccount = p.UserId != null
                    }).Take(20).ToListAsync(),
                    "staff" or "nurse" => await _context.Staff.Where(s => !s.IsDeleted).Select(s => new
                    {
                        s.Email,
                        Role = "Nurse",
                        Name = $"{s.FirstName} {s.LastName}",
                        HasUserAccount = s.UserId != null
                    }).Take(20).ToListAsync(),
                    _ => null
                };

                if (credentials == null)
                    return BadRequest("Invalid role. Valid roles: admin, doctor, patient, staff, nurse");

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving credentials for role {Role}", role);
                return StatusCode(500, "An error occurred while retrieving credentials");
            }
        }

        [HttpPost("ensure-users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EnsureUsersForEntities()
        {
            var dev = RejectIfNotDevelopment();
            if (dev != null) return dev;

            try
            {
                var result = await EnsureUsersForEntitiesAsync();
                return Ok(new
                {
                    message = "Users ensured. Passwords hashed via ASP.NET Identity.",
                    result.DoctorsLinked,
                    result.PatientsLinked,
                    result.StaffLinked
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring users for entities");
                return StatusCode(500, "An error occurred while ensuring users");
            }
        }

        private async Task<(int DoctorsLinked, int PatientsLinked, int StaffLinked)> EnsureUsersForEntitiesAsync()
        {
            int doctorsLinked = 0, patientsLinked = 0, staffLinked = 0;

            var doctorPassword = SeedPasswordProvider.ResolveForProvisioning(
                _configuration, "Doctor", _logger, true, _environment.IsDevelopment());
            var patientPassword = SeedPasswordProvider.ResolveForProvisioning(
                _configuration, "Patient", _logger, true, _environment.IsDevelopment());
            var staffPassword = SeedPasswordProvider.ResolveForProvisioning(
                _configuration, "Staff", _logger, true, _environment.IsDevelopment());

            var doctorsWithoutUsers = await _context.Doctors.Where(d => d.UserId == null && !d.IsDeleted).ToListAsync();
            foreach (var doctor in doctorsWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(doctor.Email);
                if (existingUser == null)
                {
                    if (string.IsNullOrEmpty(doctorPassword)) continue;
                    var user = new User
                    {
                        UserName = doctor.Email,
                        Email = doctor.Email,
                        FirstName = doctor.FirstName,
                        LastName = doctor.LastName,
                        NationalId = doctor.NationalId,
                        PhoneNumber = doctor.PhoneNumber,
                        PhoneNumber2 = doctor.PhoneNumber2,
                        DateOfBirth = doctor.DateOfBirth,
                        Gender = doctor.Gender,
                        Address = doctor.Address,
                        IsActive = doctor.IsActive,
                        CreatedAt = doctor.CreatedAt
                    };
                    var result = await _userManager.CreateAsync(user, doctorPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Doctor");
                        doctor.UserId = user.Id;
                        doctorsLinked++;
                    }
                }
                else
                {
                    doctor.UserId = existingUser.Id;
                    if (!await _userManager.IsInRoleAsync(existingUser, "Doctor"))
                        await _userManager.AddToRoleAsync(existingUser, "Doctor");
                    doctorsLinked++;
                }
            }

            var patientsWithoutUsers = await _context.Patients.Where(p => p.UserId == null && !p.IsDeleted).ToListAsync();
            foreach (var patient in patientsWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(patient.Email);
                if (existingUser == null)
                {
                    if (string.IsNullOrEmpty(patientPassword)) continue;
                    var user = new User
                    {
                        UserName = patient.Email,
                        Email = patient.Email,
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        NationalId = patient.NationalId,
                        PhoneNumber = patient.PhoneNumber,
                        PhoneNumber2 = patient.PhoneNumber2,
                        DateOfBirth = patient.DateOfBirth,
                        Gender = patient.Gender,
                        Address = patient.Address,
                        IsActive = patient.IsActive,
                        CreatedAt = patient.CreatedAt
                    };
                    var result = await _userManager.CreateAsync(user, patientPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Patient");
                        patient.UserId = user.Id;
                        patientsLinked++;
                    }
                }
                else
                {
                    patient.UserId = existingUser.Id;
                    if (!await _userManager.IsInRoleAsync(existingUser, "Patient"))
                        await _userManager.AddToRoleAsync(existingUser, "Patient");
                    patientsLinked++;
                }
            }

            var staffWithoutUsers = await _context.Staff.Where(s => s.UserId == null && !s.IsDeleted).ToListAsync();
            foreach (var staff in staffWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(staff.Email);
                if (existingUser == null)
                {
                    if (string.IsNullOrEmpty(staffPassword)) continue;
                    var user = new User
                    {
                        UserName = staff.Email,
                        Email = staff.Email,
                        FirstName = staff.FirstName,
                        LastName = staff.LastName,
                        NationalId = staff.NationalId,
                        PhoneNumber = staff.PhoneNumber,
                        PhoneNumber2 = staff.PhoneNumber2,
                        DateOfBirth = staff.DateOfBirth,
                        Gender = staff.Gender,
                        Address = staff.Address,
                        IsActive = staff.IsActive,
                        CreatedAt = staff.CreatedAt
                    };
                    var result = await _userManager.CreateAsync(user, staffPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");
                        await _userManager.AddToRoleAsync(user, "Nurse");
                        staff.UserId = user.Id;
                        staffLinked++;
                    }
                }
                else
                {
                    staff.UserId = existingUser.Id;
                    if (!await _userManager.IsInRoleAsync(existingUser, "Staff"))
                        await _userManager.AddToRoleAsync(existingUser, "Staff");
                    if (!await _userManager.IsInRoleAsync(existingUser, "Nurse"))
                        await _userManager.AddToRoleAsync(existingUser, "Nurse");
                    staffLinked++;
                }
            }

            await _context.SaveChangesAsync();
            return (doctorsLinked, patientsLinked, staffLinked);
        }
    }

    public class CreateTestUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
