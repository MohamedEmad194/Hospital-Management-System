using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using Hospital_Management_System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly HospitalDbContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            HospitalDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    NationalId = registerDto.NationalId,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Address = registerDto.Address,
                    PhoneNumber = registerDto.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                // Add Patient role for registration
                await _userManager.AddToRoleAsync(user, "Patient");

                var token = await GenerateJwtTokenAsync(user);
                var userDto = await MapUserToDtoAsync(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, "An error occurred during registration");
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if user exists
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                
                if (user == null)
                {
                    // Try to create user account if entity exists but user doesn't
                    var requestedRole = loginDto.Role?.Trim();
                    if (requestedRole == "Nurse") requestedRole = "Staff";

                    if (requestedRole == "Doctor")
                    {
                        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == loginDto.Email && !d.IsDeleted);
                        if (doctor != null)
                        {
                            user = new User
                            {
                                UserName = doctor.Email,
                                Email = doctor.Email,
                                FirstName = doctor.FirstName,
                                LastName = doctor.LastName,
                                NationalId = doctor.NationalId,
                                PhoneNumber = doctor.PhoneNumber,
                                DateOfBirth = doctor.DateOfBirth,
                                Gender = doctor.Gender,
                                IsActive = doctor.IsActive,
                                CreatedAt = DateTime.UtcNow
                            };
                            var createResult = await _userManager.CreateAsync(user, loginDto.Password);
                            if (createResult.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, "Doctor");
                                doctor.UserId = user.Id;
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Auto-created user account for doctor: {Email}", loginDto.Email);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to create user for doctor {Email}: {Errors}", 
                                    loginDto.Email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                            }
                        }
                    }
                    else if (requestedRole == "Patient")
                    {
                        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == loginDto.Email && !p.IsDeleted);
                        if (patient != null)
                        {
                            user = new User
                            {
                                UserName = patient.Email,
                                Email = patient.Email,
                                FirstName = patient.FirstName,
                                LastName = patient.LastName,
                                NationalId = patient.NationalId,
                                PhoneNumber = patient.PhoneNumber,
                                DateOfBirth = patient.DateOfBirth,
                                Gender = patient.Gender,
                                IsActive = patient.IsActive,
                                CreatedAt = DateTime.UtcNow
                            };
                            var createResult = await _userManager.CreateAsync(user, loginDto.Password);
                            if (createResult.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, "Patient");
                                patient.UserId = user.Id;
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Auto-created user account for patient: {Email}", loginDto.Email);
                            }
                        }
                    }
                    else if (requestedRole == "Staff")
                    {
                        var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == loginDto.Email && !s.IsDeleted);
                        if (staff != null)
                        {
                            user = new User
                            {
                                UserName = staff.Email,
                                Email = staff.Email,
                                FirstName = staff.FirstName,
                                LastName = staff.LastName,
                                NationalId = staff.NationalId,
                                PhoneNumber = staff.PhoneNumber,
                                DateOfBirth = staff.DateOfBirth,
                                Gender = staff.Gender,
                                IsActive = staff.IsActive,
                                CreatedAt = DateTime.UtcNow
                            };
                            var createResult = await _userManager.CreateAsync(user, loginDto.Password);
                            if (createResult.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, "Staff");
                                await _userManager.AddToRoleAsync(user, "Nurse");
                                staff.UserId = user.Id;
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Auto-created user account for staff: {Email}", loginDto.Email);
                            }
                        }
                    }

                    // If still null, return error with helpful message
                    if (user == null)
                    {
                        _logger.LogWarning("Login attempt with non-existent email: {Email}, Role: {Role}", loginDto.Email, loginDto.Role);
                        
                        // Check if email exists in any entity
                        var doctorExists = await _context.Doctors.AnyAsync(d => d.Email == loginDto.Email && !d.IsDeleted);
                        var patientExists = await _context.Patients.AnyAsync(p => p.Email == loginDto.Email && !p.IsDeleted);
                        var staffExists = await _context.Staff.AnyAsync(s => s.Email == loginDto.Email && !s.IsDeleted);
                        
                        string details = "Email not found in system. ";
                        if (doctorExists || patientExists || staffExists)
                        {
                            details += "Entity exists but user account not created. Please call POST /api/TestCredentials/ensure-users first.";
                        }
                        else
                        {
                            details += "Please ensure the email exists in Doctors, Patients, or Staff records.";
                        }
                        
                        return Unauthorized(new { 
                            message = "Invalid email or password", 
                            details = details,
                            hint = "Default passwords: Admin@123, Doctor@123, Patient@123, Staff@123"
                        });
                    }
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Account is inactive", details = "Your account has been deactivated. Please contact administrator." });
                }

                // Verify password
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Invalid password for user: {Email}", loginDto.Email);
                    
                    // Get user roles to help with debugging
                    var userRoles = await _userManager.GetRolesAsync(user);
                    
                    return Unauthorized(new { 
                        message = "Invalid email or password", 
                        details = $"Incorrect password for user {loginDto.Email}. Default passwords: Admin@123, Doctor@123, Patient@123, Staff@123",
                        userRoles = userRoles,
                        hint = $"User exists and has roles: {string.Join(", ", userRoles)}"
                    });
                }

                // Verify user has the selected role
                var userRoles2 = await _userManager.GetRolesAsync(user);
                var requestedRole2 = loginDto.Role?.Trim();
                
                // Map "Nurse" to "Staff" for backward compatibility
                if (requestedRole2 == "Nurse")
                    requestedRole2 = "Staff";

                if (string.IsNullOrEmpty(requestedRole2))
                {
                    return BadRequest(new { message = "Role is required", details = "Please select a role (Admin, Doctor, Patient, or Nurse)" });
                }

                if (!userRoles2.Contains(requestedRole2))
                {
                    _logger.LogWarning("User {Email} attempted login with role {RequestedRole} but has roles: {UserRoles}", 
                        loginDto.Email, requestedRole2, string.Join(", ", userRoles2));
                    return Unauthorized(new { 
                        message = $"User does not have the {loginDto.Role} role", 
                        details = $"Available roles for this user: {string.Join(", ", userRoles2)}. Please select the correct role.",
                        availableRoles = userRoles2,
                        requestedRole = loginDto.Role
                    });
                }

                // Verify the user is linked to the appropriate entity based on role
                if (!string.IsNullOrEmpty(requestedRole2))
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id && !d.IsDeleted);
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id && !p.IsDeleted);
                    var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == user.Id && !s.IsDeleted);

                    if (requestedRole2 == "Doctor" && doctor == null)
                    {
                        _logger.LogWarning("User {Email} has Doctor role but no doctor record linked. Attempting to find and link doctor record.", loginDto.Email);
                        
                        // Try to find doctor by email and link it
                        var doctorByEmail = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == loginDto.Email && !d.IsDeleted && d.UserId == null);
                        if (doctorByEmail != null)
                        {
                            doctorByEmail.UserId = user.Id;
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Successfully linked doctor record {DoctorId} to user {UserId}", doctorByEmail.Id, user.Id);
                        }
                        else
                        {
                            return Unauthorized(new { 
                                message = "No doctor record found for this user", 
                                details = "User account exists but is not linked to a doctor record. Please call POST /api/TestCredentials/ensure-users to create the link."
                            });
                        }
                    }
                    else if (requestedRole2 == "Patient" && patient == null)
                    {
                        _logger.LogWarning("User {Email} has Patient role but no patient record linked. Attempting to find and link patient record.", loginDto.Email);
                        
                        // Try to find patient by email and link it
                        var patientByEmail = await _context.Patients.FirstOrDefaultAsync(p => p.Email == loginDto.Email && !p.IsDeleted && p.UserId == null);
                        if (patientByEmail != null)
                        {
                            patientByEmail.UserId = user.Id;
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Successfully linked patient record {PatientId} to user {UserId}", patientByEmail.Id, user.Id);
                        }
                        else
                        {
                            return Unauthorized(new { 
                                message = "No patient record found for this user", 
                                details = "User account exists but is not linked to a patient record. Please call POST /api/TestCredentials/ensure-users to create the link."
                            });
                        }
                    }
                    else if (requestedRole2 == "Staff" && staff == null && requestedRole2 != "Admin")
                    {
                        // For Staff role, we're more lenient - it's OK if no Staff record exists
                        // as long as user has the Staff role
                        _logger.LogInformation("User {Email} has Staff role but no staff record linked - allowing login", loginDto.Email);
                    }
                }

                var token = await GenerateJwtTokenAsync(user);
                var userDto = await MapUserToDtoAsync(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "An error occurred while changing password");
            }
        }

        /// <summary>
        /// Send password reset email (returns generic success to avoid user enumeration)
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Return generic response to avoid leaking which emails exist
                    return Ok(new { message = "If an account exists, a reset link has been sent." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Build reset link (frontend URL from config or fallback)
                var frontendBaseUrl = _configuration["FrontendBaseUrl"] ?? "http://localhost:3000";
                var resetLink = $"{frontendBaseUrl.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(token)}";

                // TODO: wire up a real email sender; for now we log the link for devs
                _logger.LogInformation("Password reset link for {Email}: {ResetLink}", dto.Email, resetLink);

                return Ok(new { message = "تم إرسال رابط إعادة التعيين إلى بريدك إذا كان الحساب موجوداً." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email for {Email}", dto.Email);
                return StatusCode(500, new { message = "حدث خطأ أثناء إرسال رابط إعادة التعيين" });
            }
        }

        /// <summary>
        /// Reset password using token sent to email
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return Ok(new { message = "تم إعادة تعيين كلمة المرور إذا كان الحساب موجوداً." });

                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                return Ok(new { message = "تم إعادة تعيين كلمة المرور بنجاح" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", dto.Email);
                return StatusCode(500, new { message = "حدث خطأ أثناء إعادة تعيين كلمة المرور" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var userDto = await MapUserToDtoAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, "An error occurred while retrieving profile");
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                // Update user properties
                if (!string.IsNullOrEmpty(updateProfileDto.FirstName))
                    user.FirstName = updateProfileDto.FirstName;

                if (!string.IsNullOrEmpty(updateProfileDto.LastName))
                    user.LastName = updateProfileDto.LastName;

                if (!string.IsNullOrEmpty(updateProfileDto.PhoneNumber))
                    user.PhoneNumber = updateProfileDto.PhoneNumber;

                if (updateProfileDto.Address != null)
                    user.Address = updateProfileDto.Address;

                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                var userDto = await MapUserToDtoAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, "An error occurred while updating profile");
            }
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured");
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("firstName", user.FirstName ?? string.Empty),
                new Claim("lastName", user.LastName ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add DoctorId, PatientId, StaffId to claims if user is linked
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id && !d.IsDeleted);
            if (doctor != null)
            {
                claims.Add(new Claim("DoctorId", doctor.Id.ToString()));
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id && !p.IsDeleted);
            if (patient != null)
            {
                claims.Add(new Claim("PatientId", patient.Id.ToString()));
            }

            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == user.Id && !s.IsDeleted);
            if (staff != null)
            {
                claims.Add(new Claim("StaffId", staff.Id.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetJwtExpiryMinutes()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expiryMinutes = jwtSettings["ExpiryInMinutes"];
            return int.Parse(expiryMinutes ?? "60");
        }

        private async Task<UserDto> MapUserToDtoAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            // Get linked entity IDs
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id && !d.IsDeleted);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id && !p.IsDeleted);
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == user.Id && !s.IsDeleted);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                NationalId = user.NationalId ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender ?? string.Empty,
                Address = user.Address ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                PhoneNumber2 = user.PhoneNumber2 ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList(),
                DoctorId = doctor?.Id,
                PatientId = patient?.Id,
                StaffId = staff?.Id
            };
        }
    }

    public class UpdateProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
