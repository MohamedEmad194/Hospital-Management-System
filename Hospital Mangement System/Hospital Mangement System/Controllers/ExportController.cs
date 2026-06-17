using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly ILogger<ExportController> _logger;

        public ExportController(HospitalDbContext context, ILogger<ExportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Stream any of the main entities as a UTF-8 BOM CSV file (Excel opens these directly).
        /// </summary>
        [HttpGet("{entity}")]
        [AllowAnonymous]
        public async Task<IActionResult> Export(string entity)
        {
            try
            {
                var (rows, filename) = entity.ToLowerInvariant() switch
                {
                    "departments" => (await BuildDepartmentsAsync(), "departments.csv"),
                    "doctors"     => (await BuildDoctorsAsync(),     "doctors.csv"),
                    "patients"    => (await BuildPatientsAsync(),    "patients.csv"),
                    "appointments"=> (await BuildAppointmentsAsync(),"appointments.csv"),
                    "bills"       => (await BuildBillsAsync(),       "bills.csv"),
                    "medicines"   => (await BuildMedicinesAsync(),   "medicines.csv"),
                    "rooms"       => (await BuildRoomsAsync(),       "rooms.csv"),
                    _             => (null, null)
                };

                if (rows == null)
                {
                    return BadRequest(new
                    {
                        message = $"Unknown entity '{entity}'.",
                        allowed = new[] {
                            "departments","doctors","patients","appointments",
                            "bills","medicines","rooms"
                        }
                    });
                }

                var bytes = ToCsvBytes(rows!);
                return File(bytes, "text/csv; charset=utf-8", filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting {Entity}", entity);
                return StatusCode(500, new { message = "Export failed", error = ex.Message });
            }
        }

        // ------------------ entity-specific row builders ------------------

        private async Task<List<string[]>> BuildDepartmentsAsync()
        {
            var data = await _context.Departments
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Id)
                .Select(d => new
                {
                    d.Id, d.Name, d.Description, d.HeadOfDepartment,
                    d.PhoneNumber, d.Location, d.IsActive, d.CreatedAt
                })
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","Name","Description","HeadOfDepartment",
                        "PhoneNumber","Location","IsActive","CreatedAt" }
            };
            foreach (var d in data)
            {
                rows.Add(new[]
                {
                    d.Id.ToString(),
                    d.Name ?? "",
                    d.Description ?? "",
                    d.HeadOfDepartment ?? "",
                    d.PhoneNumber ?? "",
                    d.Location ?? "",
                    d.IsActive.ToString(),
                    d.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildDoctorsAsync()
        {
            var data = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Id)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","FirstName","LastName","NationalId","Email","PhoneNumber",
                        "DateOfBirth","Gender","Address","LicenseNumber","Specialization",
                        "SubSpecialization","YearsOfExperience","Education","Certifications",
                        "Languages","ConsultationFee","WorkingHoursStart","WorkingHoursEnd",
                        "Department","IsActive" }
            };
            foreach (var d in data)
            {
                rows.Add(new[]
                {
                    d.Id.ToString(),
                    d.FirstName ?? "",
                    d.LastName ?? "",
                    d.NationalId ?? "",
                    d.Email ?? "",
                    d.PhoneNumber ?? "",
                    d.DateOfBirth.ToString("yyyy-MM-dd"),
                    d.Gender ?? "",
                    d.Address ?? "",
                    d.LicenseNumber ?? "",
                    d.Specialization ?? "",
                    d.SubSpecialization ?? "",
                    d.YearsOfExperience.ToString(),
                    d.Education ?? "",
                    d.Certifications ?? "",
                    d.Languages ?? "",
                    d.ConsultationFee.ToString(CultureInfo.InvariantCulture),
                    d.WorkingHoursStart.ToString(),
                    d.WorkingHoursEnd.ToString(),
                    d.Department?.Name ?? "",
                    d.IsActive.ToString()
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildPatientsAsync()
        {
            var data = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Id)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","FirstName","LastName","NationalId","Email","PhoneNumber",
                        "DateOfBirth","Gender","Address","EmergencyContactName",
                        "EmergencyContactPhone","InsuranceProvider","InsuranceNumber",
                        "MedicalHistory","Allergies","IsActive","CreatedAt" }
            };
            foreach (var p in data)
            {
                rows.Add(new[]
                {
                    p.Id.ToString(),
                    p.FirstName ?? "",
                    p.LastName ?? "",
                    p.NationalId ?? "",
                    p.Email ?? "",
                    p.PhoneNumber ?? "",
                    p.DateOfBirth.ToString("yyyy-MM-dd"),
                    p.Gender ?? "",
                    p.Address ?? "",
                    p.EmergencyContactName ?? "",
                    p.EmergencyContactPhone ?? "",
                    p.InsuranceProvider ?? "",
                    p.InsuranceNumber ?? "",
                    p.MedicalHistory ?? "",
                    p.Allergies ?? "",
                    p.IsActive.ToString(),
                    p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildAppointmentsAsync()
        {
            var data = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Room)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","AppointmentDate","AppointmentTime","Status","Reason",
                        "Diagnosis","Treatment","Notes","ConsultationFee",
                        "Patient","Doctor","Room","CreatedAt" }
            };
            foreach (var a in data)
            {
                rows.Add(new[]
                {
                    a.Id.ToString(),
                    a.AppointmentDate.ToString("yyyy-MM-dd"),
                    a.AppointmentTime.ToString(),
                    a.Status ?? "",
                    a.Reason ?? "",
                    a.Diagnosis ?? "",
                    a.Treatment ?? "",
                    a.Notes ?? "",
                    a.ConsultationFee?.ToString(CultureInfo.InvariantCulture) ?? "",
                    a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : "",
                    a.Doctor != null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}" : "",
                    a.Room?.RoomNumber ?? "",
                    a.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildBillsAsync()
        {
            var data = await _context.Bills
                .Include(b => b.Patient)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","BillNumber","BillDate","DueDate","Status",
                        "SubTotal","TaxAmount","DiscountAmount","TotalAmount",
                        "PaidAmount","RemainingAmount","InsuranceProvider",
                        "InsuranceNumber","InsuranceCoverage","Patient","CreatedAt" }
            };
            foreach (var b in data)
            {
                rows.Add(new[]
                {
                    b.Id.ToString(),
                    b.BillNumber ?? "",
                    b.BillDate.ToString("yyyy-MM-dd"),
                    b.DueDate.ToString("yyyy-MM-dd"),
                    b.Status ?? "",
                    b.SubTotal.ToString(CultureInfo.InvariantCulture),
                    b.TaxAmount.ToString(CultureInfo.InvariantCulture),
                    b.DiscountAmount.ToString(CultureInfo.InvariantCulture),
                    b.TotalAmount.ToString(CultureInfo.InvariantCulture),
                    b.PaidAmount.ToString(CultureInfo.InvariantCulture),
                    b.RemainingAmount.ToString(CultureInfo.InvariantCulture),
                    b.InsuranceProvider ?? "",
                    b.InsuranceNumber ?? "",
                    b.InsuranceCoverage?.ToString(CultureInfo.InvariantCulture) ?? "",
                    b.Patient != null ? $"{b.Patient.FirstName} {b.Patient.LastName}" : "",
                    b.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildMedicinesAsync()
        {
            var data = await _context.Medicines
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Name)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","Name","GenericName","DosageForm","Strength","Manufacturer",
                        "Description","Indications","Price","StockQuantity",
                        "MinimumStockLevel","Unit","ExpiryDate","BatchNumber",
                        "RequiresPrescription","IsActive" }
            };
            foreach (var m in data)
            {
                rows.Add(new[]
                {
                    m.Id.ToString(),
                    m.Name ?? "",
                    m.GenericName ?? "",
                    m.DosageForm ?? "",
                    m.Strength ?? "",
                    m.Manufacturer ?? "",
                    m.Description ?? "",
                    m.Indications ?? "",
                    m.Price.ToString(CultureInfo.InvariantCulture),
                    m.StockQuantity.ToString(),
                    m.MinimumStockLevel.ToString(),
                    m.Unit ?? "",
                    m.ExpiryDate?.ToString("yyyy-MM-dd") ?? "",
                    m.BatchNumber ?? "",
                    m.RequiresPrescription.ToString(),
                    m.IsActive.ToString()
                });
            }
            return rows;
        }

        private async Task<List<string[]>> BuildRoomsAsync()
        {
            var data = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            var rows = new List<string[]>
            {
                new[] { "Id","RoomNumber","RoomType","Floor","Building","Description",
                        "Capacity","HourlyRate","IsAvailable","Department","IsActive" }
            };
            foreach (var r in data)
            {
                rows.Add(new[]
                {
                    r.Id.ToString(),
                    r.RoomNumber ?? "",
                    r.RoomType ?? "",
                    r.Floor ?? "",
                    r.Building ?? "",
                    r.Description ?? "",
                    r.Capacity.ToString(),
                    r.HourlyRate?.ToString(CultureInfo.InvariantCulture) ?? "",
                    r.IsAvailable.ToString(),
                    r.Department?.Name ?? "",
                    r.IsActive.ToString()
                });
            }
            return rows;
        }

        // ------------------ CSV serialization ------------------

        private static byte[] ToCsvBytes(List<string[]> rows)
        {
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(EscapeCsv(row[i]));
                }
                sb.Append("\r\n");
            }

            // UTF-8 BOM so Excel detects unicode automatically
            var encoder = new UTF8Encoding(true);
            var bom = encoder.GetPreamble();
            var body = encoder.GetBytes(sb.ToString());
            var result = new byte[bom.Length + body.Length];
            Buffer.BlockCopy(bom, 0, result, 0, bom.Length);
            Buffer.BlockCopy(body, 0, result, bom.Length, body.Length);
            return result;
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            var needsQuotes = value.Contains(',') || value.Contains('"') ||
                              value.Contains('\n') || value.Contains('\r');
            var escaped = value.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }
    }
}
