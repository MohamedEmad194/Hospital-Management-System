using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hospital_Management_System.Data;
using Hospital_Management_System.Models;
using Hospital_Management_System.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Expand environment placeholders in configuration values used by Kestrel (e.g., ${USERPROFILE})
var httpsCertPath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:Path"];
if (!string.IsNullOrWhiteSpace(httpsCertPath))
{
    var expandedUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var normalized = httpsCertPath
        .Replace("${USERPROFILE}", expandedUserProfile)
        .Replace("%USERPROFILE%", expandedUserProfile);

    if (!string.Equals(normalized, httpsCertPath, StringComparison.Ordinal))
    {
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Kestrel:Endpoints:Https:Certificate:Path"] = normalized
        });
    }
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/hospital-management-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<HospitalDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
    
    // Handle authentication failures gracefully
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning(context.Exception, "JWT Authentication failed");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { 
                message = "Authentication required", 
                details = "Please login to access this resource" 
            });
            return context.Response.WriteAsync(result);
        }
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "https://localhost:3000",
                "http://localhost:5230",
                "https://localhost:7102"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24 hours
    });
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add custom services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IPaymentGatewayService, StripePaymentService>();
builder.Services.AddScoped<PayPalPaymentService>();
builder.Services.AddHttpClient<PaymobPaymentService>();
builder.Services.AddScoped<PaymobPaymentService>();
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<INursingUnitService, NursingUnitService>();
// Register OpenAI service (optional)
var useOpenAI = builder.Configuration.GetValue<bool>("ChatbotSettings:UseOpenAI", false);
var openAIKey = builder.Configuration["ChatbotSettings:OpenAIApiKey"];

if (useOpenAI && !string.IsNullOrEmpty(openAIKey))
{
    builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();
    builder.Services.AddScoped<IOpenAIService, OpenAIService>();
}

builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Hospital Management System API", 
        Version = "v1",
        Description = "A comprehensive hospital management system API"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline - Swagger enabled for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Management System API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root - opens when visiting the site
});

app.UseCors("AllowAll");
// Disable HTTPS redirection in development to avoid SSL certificate issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seed data (configurable)
await SeedDatabaseAsync(app);

app.Run();

async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
	try
	{
		context.Database.Migrate();
	}
	catch (Exception ex)
	{
		Log.Warning(ex, "Migration failed, using EnsureCreated instead");
		context.Database.EnsureCreated();
	}
    
    var seedOptions = app.Configuration.GetSection("SeedOptions");
    var enableRoles = seedOptions.GetValue<bool>("EnableRoles", true);
    var enableAdmin = seedOptions.GetValue<bool>("EnableAdmin", true);
    var enableSampleData = seedOptions.GetValue<bool>("EnableSampleData", false);

    if (enableRoles)
    {
        await SeedData.SeedRolesAsync(roleManager);
    }

    if (enableAdmin)
    {
        await SeedData.SeedAdminUserAsync(userManager);
    }

    if (enableSampleData)
    {
        try
        {
            await SeedData.SeedSampleDataAsync(context);
            // Automatically create user accounts for doctors, patients, and staff
            await EnsureUsersForEntitiesAsync(userManager, context);
        }
        catch (Exception ex)
        {
            // In development, avoid failing startup due to sample data conflicts
            Log.Warning(ex, "Sample data seeding skipped due to error");
        }
    }
    else
    {
        // Even if sample data is disabled, ensure we have essential rooms and medicines
        try
        {
            var roomsCount = await context.Rooms.CountAsync(r => !r.IsDeleted);
            var medicinesCount = await context.Medicines.CountAsync(m => !m.IsDeleted);
            
            // If we have very few rooms or medicines, try to seed essential data
            if (roomsCount < 5 || medicinesCount < 5)
            {
                Log.Information("Few rooms ({RoomsCount}) or medicines ({MedicinesCount}) found. Consider calling POST /api/Seed/essential-data", roomsCount, medicinesCount);
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error checking essential data");
        }
    }

    // Always try to ensure users for existing entities (even if sample data is disabled)
    try
    {
        await EnsureUsersForEntitiesAsync(userManager, context);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Error ensuring users for entities during startup");
    }
}

static async Task EnsureUsersForEntitiesAsync(UserManager<User> userManager, HospitalDbContext context)
{
    int doctorsLinked = 0;
    int patientsLinked = 0;
    int staffLinked = 0;

    // Ensure users for Doctors
    var doctorsWithoutUsers = await context.Doctors
        .Where(d => d.UserId == null && !d.IsDeleted)
        .ToListAsync();

    foreach (var doctor in doctorsWithoutUsers)
    {
        var existingUser = await userManager.FindByEmailAsync(doctor.Email);
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

            var result = await userManager.CreateAsync(user, "Doctor@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Doctor");
                doctor.UserId = user.Id;
                doctorsLinked++;
            }
        }
        else
        {
            doctor.UserId = existingUser.Id;
            if (!await userManager.IsInRoleAsync(existingUser, "Doctor"))
            {
                await userManager.AddToRoleAsync(existingUser, "Doctor");
            }
            doctorsLinked++;
        }
    }

    // Ensure users for Patients
    var patientsWithoutUsers = await context.Patients
        .Where(p => p.UserId == null && !p.IsDeleted)
        .ToListAsync();

    foreach (var patient in patientsWithoutUsers)
    {
        var existingUser = await userManager.FindByEmailAsync(patient.Email);
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

            var result = await userManager.CreateAsync(user, "Patient@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Patient");
                patient.UserId = user.Id;
                patientsLinked++;
            }
        }
        else
        {
            patient.UserId = existingUser.Id;
            if (!await userManager.IsInRoleAsync(existingUser, "Patient"))
            {
                await userManager.AddToRoleAsync(existingUser, "Patient");
            }
            patientsLinked++;
        }
    }

    // Ensure users for Staff
    var staffWithoutUsers = await context.Staff
        .Where(s => s.UserId == null && !s.IsDeleted)
        .ToListAsync();

    foreach (var staff in staffWithoutUsers)
    {
        var existingUser = await userManager.FindByEmailAsync(staff.Email);
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

            var result = await userManager.CreateAsync(user, "Staff@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Staff");
                await userManager.AddToRoleAsync(user, "Nurse");
                staff.UserId = user.Id;
                staffLinked++;
            }
        }
        else
        {
            staff.UserId = existingUser.Id;
            if (!await userManager.IsInRoleAsync(existingUser, "Staff"))
            {
                await userManager.AddToRoleAsync(existingUser, "Staff");
            }
            if (!await userManager.IsInRoleAsync(existingUser, "Nurse"))
            {
                await userManager.AddToRoleAsync(existingUser, "Nurse");
            }
            staffLinked++;
        }
    }

    await context.SaveChangesAsync();

    if (doctorsLinked > 0 || patientsLinked > 0 || staffLinked > 0)
    {
        Log.Information("Linked {DoctorsLinked} doctors, {PatientsLinked} patients, {StaffLinked} staff to user accounts", 
            doctorsLinked, patientsLinked, staffLinked);
    }
}