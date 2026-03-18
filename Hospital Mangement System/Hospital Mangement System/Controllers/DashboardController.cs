using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(HospitalDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard statistics (public endpoint)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                // Check if database is accessible
                if (!await _context.Database.CanConnectAsync())
                {
                    _logger.LogWarning("Database connection failed");
                    return StatusCode(503, new { message = "Database is not available" });
                }

                var stats = new DashboardStatsDto
                {
                    TotalPatients = await _context.Patients.CountAsync(p => !p.IsDeleted),
                    TotalDoctors = await _context.Doctors.CountAsync(d => !d.IsDeleted),
                    TotalAppointments = await _context.Appointments.CountAsync(a => !a.IsDeleted),
                    TotalDepartments = await _context.Departments.CountAsync(d => !d.IsDeleted),
                    TotalRooms = await _context.Rooms.CountAsync(r => !r.IsDeleted),
                    TotalMedicines = await _context.Medicines.CountAsync(m => !m.IsDeleted),
                    TotalBills = await _context.Bills.CountAsync(b => !b.IsDeleted),
                    PendingAppointments = await _context.Appointments.CountAsync(a => !a.IsDeleted && a.Status == "Scheduled"),
                    CompletedAppointments = await _context.Appointments.CountAsync(a => !a.IsDeleted && a.Status == "Completed"),
                    OverdueBills = await _context.Bills.CountAsync(b => !b.IsDeleted && b.Status != "Paid" && b.DueDate < DateTime.UtcNow),
                    LowStockMedicines = await _context.Medicines.CountAsync(m => !m.IsDeleted && m.StockQuantity <= m.MinimumStockLevel)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get recent appointments
        /// </summary>
        [HttpGet("recent-appointments")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<RecentAppointmentDto>>> GetRecentAppointments([FromQuery] int count = 10)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => !a.IsDeleted)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(count)
                    .Select(a => new RecentAppointmentDto
                    {
                        Id = a.Id,
                        PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                        AppointmentDate = a.AppointmentDate,
                        AppointmentTime = a.AppointmentTime,
                        Status = a.Status,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent appointments");
                return StatusCode(500, "An error occurred while retrieving recent appointments");
            }
        }

        /// <summary>
        /// Get appointments by status
        /// </summary>
        [HttpGet("appointments-by-status")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<AppointmentStatusDto>>> GetAppointmentsByStatus()
        {
            try
            {
                var appointmentsByStatus = await _context.Appointments
                    .Where(a => !a.IsDeleted)
                    .GroupBy(a => a.Status)
                    .Select(g => new AppointmentStatusDto
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return Ok(appointmentsByStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments by status");
                return StatusCode(500, "An error occurred while retrieving appointments by status");
            }
        }

        /// <summary>
        /// Get revenue statistics
        /// </summary>
        [HttpGet("revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RevenueStatsDto>> GetRevenueStats()
        {
            try
            {
                var totalRevenue = await _context.Bills
                    .Where(b => !b.IsDeleted && b.Status == "Paid")
                    .SumAsync(b => b.PaidAmount);

                var pendingRevenue = await _context.Bills
                    .Where(b => !b.IsDeleted && b.Status != "Paid")
                    .SumAsync(b => b.RemainingAmount);

                var monthlyRevenue = await _context.Bills
                    .Where(b => !b.IsDeleted && b.Status == "Paid" && 
                               b.PaymentDate.HasValue && 
                               b.PaymentDate.Value.Year == DateTime.UtcNow.Year &&
                               b.PaymentDate.Value.Month == DateTime.UtcNow.Month)
                    .SumAsync(b => b.PaidAmount);

                var stats = new RevenueStatsDto
                {
                    TotalRevenue = totalRevenue,
                    PendingRevenue = pendingRevenue,
                    MonthlyRevenue = monthlyRevenue
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue statistics");
                return StatusCode(500, "An error occurred while retrieving revenue statistics");
            }
        }

        /// <summary>
        /// Get department statistics
        /// </summary>
        [HttpGet("department-stats")]
        [Authorize(Roles = "Admin,Doctor,Staff")]
        public async Task<ActionResult<IEnumerable<DepartmentStatsDto>>> GetDepartmentStats()
        {
            try
            {
                var departmentStats = await _context.Departments
                    .Where(d => !d.IsDeleted)
                    .Select(d => new DepartmentStatsDto
                    {
                        DepartmentId = d.Id,
                        DepartmentName = d.Name,
                        DoctorCount = _context.Doctors.Count(doc => doc.DepartmentId == d.Id && !doc.IsDeleted),
                        RoomCount = _context.Rooms.Count(r => r.DepartmentId == d.Id && !r.IsDeleted),
                        AppointmentCount = _context.Appointments.Count(a => a.Doctor.DepartmentId == d.Id && !a.IsDeleted)
                    })
                    .ToListAsync();

                return Ok(departmentStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department statistics");
                return StatusCode(500, "An error occurred while retrieving department statistics");
            }
        }
    }

    public class DashboardStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalRooms { get; set; }
        public int TotalMedicines { get; set; }
        public int TotalBills { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int OverdueBills { get; set; }
        public int LowStockMedicines { get; set; }
    }

    public class RecentAppointmentDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RevenueStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class DepartmentStatsDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int DoctorCount { get; set; }
        public int RoomCount { get; set; }
        public int AppointmentCount { get; set; }
    }
}
