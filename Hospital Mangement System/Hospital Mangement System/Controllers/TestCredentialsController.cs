using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Controllers
{
    /// <summary>
    /// Test Credentials Controller - Provides test login credentials for testing
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestCredentialsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly HospitalDbContext _context;
        private readonly ILogger<TestCredentialsController> _logger;

        public TestCredentialsController(
            UserManager<User> userManager,
            HospitalDbContext context,
            ILogger<TestCredentialsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Create a test user account for quick testing
        /// </summary>
        [HttpPost("create-test-user")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateTestUser([FromBody] CreateTestUserDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Role))
                {
                    return BadRequest(new { message = "Email, Password, and Role are required" });
                }

                // Check if user already exists
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

                // Create user
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

                // Add role
                var role = dto.Role.Trim();
                if (role == "Nurse") role = "Staff";
                
                await _userManager.AddToRoleAsync(user, role);

                return Ok(new 
                { 
                    message = "Test user created successfully",
                    email = dto.Email,
                    password = dto.Password,
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

        /// <summary>
        /// Get all test credentials (Admin, Doctors, Patients, Staff)
        /// Creates user accounts if they don't exist
        /// </summary>
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllTestCredentials()
        {
            try
            {
                // Ensure users are created for entities
                await EnsureUsersForEntitiesAsync();

                var credentials = new
                {
                    Admin = new
                    {
                        Email = "admin@hospital.com",
                        Password = "Admin@123",
                        Role = "Admin",
                        Note = "Default admin account"
                    },
                    Doctors = await _context.Doctors
                        .Where(d => !d.IsDeleted)
                        .Select(d => new
                        {
                            Email = d.Email,
                            Password = "Doctor@123",
                            Role = "Doctor",
                            Name = $"{d.FirstName} {d.LastName}",
                            Specialization = d.Specialization,
                            DepartmentId = d.DepartmentId,
                            HasUserAccount = d.UserId != null
                        })
                        .Take(20)
                        .ToListAsync(),
                    Patients = await _context.Patients
                        .Where(p => !p.IsDeleted)
                        .Select(p => new
                        {
                            Email = p.Email,
                            Password = "Patient@123",
                            Role = "Patient",
                            Name = $"{p.FirstName} {p.LastName}",
                            NationalId = p.NationalId,
                            HasUserAccount = p.UserId != null
                        })
                        .Take(20)
                        .ToListAsync(),
                    Staff = await _context.Staff
                        .Where(s => !s.IsDeleted)
                        .Select(s => new
                        {
                            Email = s.Email,
                            Password = "Staff@123",
                            Role = "Nurse",
                            Name = $"{s.FirstName} {s.LastName}",
                            Position = s.Position,
                            HasUserAccount = s.UserId != null
                        })
                        .Take(20)
                        .ToListAsync(),
                    DefaultPasswords = new
                    {
                        Admin = "Admin@123",
                        Doctor = "Doctor@123",
                        Patient = "Patient@123",
                        Staff = "Staff@123",
                        Nurse = "Staff@123" // Nurse uses Staff password
                    }
                };

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving test credentials");
                return StatusCode(500, "An error occurred while retrieving test credentials");
            }
        }

        /// <summary>
        /// Get credentials for specific role
        /// </summary>
        [HttpGet("role/{role}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCredentialsByRole(string role)
        {
            try
            {
                await EnsureUsersForEntitiesAsync();

                role = role.ToLower();
                object? credentials = null;

                switch (role)
                {
                    case "admin":
                        credentials = new
                        {
                            Email = "admin@hospital.com",
                            Password = "Admin@123",
                            Role = "Admin"
                        };
                        break;

                    case "doctor":
                    case "doctors":
                        credentials = await _context.Doctors
                            .Where(d => !d.IsDeleted)
                            .Select(d => new
                            {
                                Email = d.Email,
                                Password = "Doctor@123",
                                Role = "Doctor",
                                Name = $"{d.FirstName} {d.LastName}",
                                Specialization = d.Specialization,
                                HasUserAccount = d.UserId != null
                            })
                            .Take(20)
                            .ToListAsync();
                        break;

                    case "patient":
                    case "patients":
                        credentials = await _context.Patients
                            .Where(p => !p.IsDeleted)
                            .Select(p => new
                            {
                                Email = p.Email,
                                Password = "Patient@123",
                                Role = "Patient",
                                Name = $"{p.FirstName} {p.LastName}",
                                HasUserAccount = p.UserId != null
                            })
                            .Take(20)
                            .ToListAsync();
                        break;

                    case "staff":
                    case "nurse":
                        credentials = await _context.Staff
                            .Where(s => !s.IsDeleted)
                            .Select(s => new
                            {
                                Email = s.Email,
                                Password = "Staff@123",
                                Role = "Nurse",
                                Name = $"{s.FirstName} {s.LastName}",
                                Position = s.Position,
                                HasUserAccount = s.UserId != null
                            })
                            .Take(20)
                            .ToListAsync();
                        break;

                    default:
                        return BadRequest($"Invalid role. Valid roles: admin, doctor, patient, staff, nurse");
                }

                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving credentials for role {Role}", role);
                return StatusCode(500, "An error occurred while retrieving credentials");
            }
        }

        /// <summary>
        /// Create/ensure user accounts exist for all doctors, patients, and staff
        /// </summary>
        [HttpPost("ensure-users")]
        [AllowAnonymous]
        public async Task<ActionResult> EnsureUsersForEntities()
        {
            try
            {
                var result = await EnsureUsersForEntitiesAsync();
                return Ok(new
                {
                    message = "Users ensured successfully",
                    doctorsLinked = result.DoctorsLinked,
                    patientsLinked = result.PatientsLinked,
                    staffLinked = result.StaffLinked
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
            int doctorsLinked = 0;
            int patientsLinked = 0;
            int staffLinked = 0;

            // Ensure users for Doctors
            var doctorsWithoutUsers = await _context.Doctors
                .Where(d => d.UserId == null && !d.IsDeleted)
                .ToListAsync();

            foreach (var doctor in doctorsWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(doctor.Email);
                if (existingUser == null)
                {
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

                    var result = await _userManager.CreateAsync(user, "Doctor@123");
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
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Doctor");
                    }
                    doctorsLinked++;
                }
            }

            // Ensure users for Patients
            var patientsWithoutUsers = await _context.Patients
                .Where(p => p.UserId == null && !p.IsDeleted)
                .ToListAsync();

            foreach (var patient in patientsWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(patient.Email);
                if (existingUser == null)
                {
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

                    var result = await _userManager.CreateAsync(user, "Patient@123");
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
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Patient");
                    }
                    patientsLinked++;
                }
            }

            // Ensure users for Staff
            var staffWithoutUsers = await _context.Staff
                .Where(s => s.UserId == null && !s.IsDeleted)
                .ToListAsync();

            foreach (var staff in staffWithoutUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(staff.Email);
                if (existingUser == null)
                {
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

                    var result = await _userManager.CreateAsync(user, "Staff@123");
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
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Staff");
                    }
                    if (!await _userManager.IsInRoleAsync(existingUser, "Nurse"))
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Nurse");
                    }
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
        public string Role { get; set; } = string.Empty; // Admin, Doctor, Patient, Staff, Nurse
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

