using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management_System.Controllers
{
    /// <summary>
    /// Seed Controller - For seeding essential data (Rooms and Medicines)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(HospitalDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seed essential rooms and medicines data (can be called multiple times safely)
        /// </summary>
        [HttpPost("essential-data")]
        [AllowAnonymous] // Allow anonymous to make it easy to seed
        public async Task<ActionResult> SeedEssentialData()
        {
            try
            {
                int roomsAdded = 0;
                int medicinesAdded = 0;

                // Get department IDs
                var deptIds = await _context.Departments
                    .Where(d => !d.IsDeleted)
                    .ToDictionaryAsync(d => d.Name, d => d.Id);

                var cardiologyDeptId = deptIds.GetValueOrDefault("Cardiology", 0);
                var neurologyDeptId = deptIds.GetValueOrDefault("Neurology", 0);
                var orthopedicsDeptId = deptIds.GetValueOrDefault("Orthopedics", 0);
                var pediatricsDeptId = deptIds.GetValueOrDefault("Pediatrics", 0);
                var emergencyDeptId = deptIds.GetValueOrDefault("Emergency", 0);

                // Check existing room numbers
                var existingRoomNumbers = await _context.Rooms
                    .Where(r => !r.IsDeleted)
                    .Select(r => r.RoomNumber)
                    .ToListAsync();

                // Essential rooms to add
                var essentialRooms = new[]
                {
                    new Models.Room { RoomNumber = "C101", RoomType = "Consultation", Floor = "Floor 2", Building = "Building A", Description = "Cardiology consultation room", Capacity = 1, HourlyRate = 100, IsAvailable = true, IsActive = true, DepartmentId = cardiologyDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "N201", RoomType = "Consultation", Floor = "Floor 3", Building = "Building A", Description = "Neurology consultation room", Capacity = 1, HourlyRate = 120, IsAvailable = true, IsActive = true, DepartmentId = neurologyDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "O101", RoomType = "Consultation", Floor = "Floor 1", Building = "Building B", Description = "Orthopedics consultation room", Capacity = 1, HourlyRate = 110, IsAvailable = true, IsActive = true, DepartmentId = orthopedicsDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "P201", RoomType = "Consultation", Floor = "Floor 2", Building = "Building B", Description = "Pediatrics consultation room", Capacity = 1, HourlyRate = 90, IsAvailable = true, IsActive = true, DepartmentId = pediatricsDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "E001", RoomType = "Emergency", Floor = "Ground Floor", Building = "Building A", Description = "Emergency room", Capacity = 4, HourlyRate = 200, IsAvailable = true, IsActive = true, DepartmentId = emergencyDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "ICU-001", RoomType = "ICU", Floor = "Floor 4", Building = "Building A", Description = "Intensive Care Unit room 1", Capacity = 2, HourlyRate = 500, IsAvailable = true, IsActive = true, DepartmentId = emergencyDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "SURG-001", RoomType = "Surgery", Floor = "Floor 1", Building = "Building B", Description = "Surgery room 1", Capacity = 1, HourlyRate = 800, IsAvailable = true, IsActive = true, DepartmentId = orthopedicsDeptId, CreatedAt = DateTime.UtcNow },
                    new Models.Room { RoomNumber = "WARD-101", RoomType = "Ward", Floor = "Floor 3", Building = "Building B", Description = "General ward room 1", Capacity = 4, HourlyRate = 150, IsAvailable = true, IsActive = true, DepartmentId = cardiologyDeptId, CreatedAt = DateTime.UtcNow }
                };

                var roomsToAdd = essentialRooms
                    .Where(r => !existingRoomNumbers.Contains(r.RoomNumber) && r.DepartmentId > 0)
                    .ToList();

                if (roomsToAdd.Any())
                {
                    _context.Rooms.AddRange(roomsToAdd);
                    await _context.SaveChangesAsync();
                    roomsAdded = roomsToAdd.Count;
                }

                // Check existing medicine names
                var existingMedicineNames = await _context.Medicines
                    .Where(m => !m.IsDeleted)
                    .Select(m => m.Name)
                    .ToListAsync();

                // Essential medicines to add
                var essentialMedicines = new[]
                {
                    new Models.Medicine { Name = "Paracetamol", GenericName = "Acetaminophen", DosageForm = "Tablet", Strength = "500mg", Manufacturer = "PharmaCorp", Description = "Pain reliever and fever reducer", Indications = "Headache, fever, muscle pain", Contraindications = "Liver disease, alcohol use", SideEffects = "Nausea, rash", DosageInstructions = "1-2 tablets every 4-6 hours", Price = 5.00m, StockQuantity = 1000, MinimumStockLevel = 100, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "PC2024001", RequiresPrescription = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Amoxicillin", GenericName = "Amoxicillin", DosageForm = "Capsule", Strength = "250mg", Manufacturer = "MediCorp", Description = "Antibiotic for bacterial infections", Indications = "Respiratory infections, skin infections", Contraindications = "Penicillin allergy", SideEffects = "Diarrhea, nausea, rash", DosageInstructions = "1 capsule every 8 hours for 7 days", Price = 15.00m, StockQuantity = 500, MinimumStockLevel = 50, Unit = "capsule", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "MC2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Ibuprofen", GenericName = "Ibuprofen", DosageForm = "Tablet", Strength = "400mg", Manufacturer = "PharmaCorp", Description = "Anti-inflammatory and pain reliever", Indications = "Pain, inflammation, fever", Contraindications = "Stomach ulcers, kidney disease", SideEffects = "Stomach upset, dizziness", DosageInstructions = "1 tablet every 6-8 hours", Price = 8.00m, StockQuantity = 800, MinimumStockLevel = 100, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "IB2024001", RequiresPrescription = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Aspirin", GenericName = "Acetylsalicylic acid", DosageForm = "Tablet", Strength = "100mg", Manufacturer = "MediCorp", Description = "Blood thinner and pain reliever", Indications = "Heart attack prevention, pain relief", Contraindications = "Bleeding disorders, children under 12", SideEffects = "Stomach irritation, bleeding", DosageInstructions = "1 tablet daily for heart protection", Price = 3.00m, StockQuantity = 1200, MinimumStockLevel = 150, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "AS2024001", RequiresPrescription = false, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Omeprazole", GenericName = "Omeprazole", DosageForm = "Capsule", Strength = "20mg", Manufacturer = "PharmaCorp", Description = "Proton pump inhibitor for acid reflux", Indications = "GERD, stomach ulcers", Contraindications = "Severe liver disease", SideEffects = "Headache, diarrhea", DosageInstructions = "1 capsule daily before breakfast", Price = 25.00m, StockQuantity = 600, MinimumStockLevel = 80, Unit = "capsule", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "OM2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Metformin", GenericName = "Metformin", DosageForm = "Tablet", Strength = "500mg", Manufacturer = "MediCorp", Description = "Diabetes medication", Indications = "Type 2 diabetes", Contraindications = "Kidney disease, lactic acidosis", SideEffects = "Nausea, diarrhea", DosageInstructions = "1 tablet twice daily with meals", Price = 12.00m, StockQuantity = 700, MinimumStockLevel = 100, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "MT2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Amlodipine", GenericName = "Amlodipine", DosageForm = "Tablet", Strength = "5mg", Manufacturer = "PharmaCorp", Description = "Blood pressure medication", Indications = "Hypertension, angina", Contraindications = "Severe hypotension", SideEffects = "Swelling, dizziness", DosageInstructions = "1 tablet daily", Price = 18.00m, StockQuantity = 550, MinimumStockLevel = 70, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "AM2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Atorvastatin", GenericName = "Atorvastatin", DosageForm = "Tablet", Strength = "20mg", Manufacturer = "MediCorp", Description = "Cholesterol lowering medication", Indications = "High cholesterol, heart disease prevention", Contraindications = "Liver disease, pregnancy", SideEffects = "Muscle pain, liver problems", DosageInstructions = "1 tablet daily at bedtime", Price = 30.00m, StockQuantity = 500, MinimumStockLevel = 60, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "AT2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Losartan", GenericName = "Losartan", DosageForm = "Tablet", Strength = "50mg", Manufacturer = "PharmaCorp", Description = "Blood pressure and kidney protection", Indications = "Hypertension, diabetic kidney disease", Contraindications = "Pregnancy, severe kidney disease", SideEffects = "Dizziness, fatigue", DosageInstructions = "1 tablet daily", Price = 22.00m, StockQuantity = 600, MinimumStockLevel = 75, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "LO2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Models.Medicine { Name = "Levothyroxine", GenericName = "Levothyroxine", DosageForm = "Tablet", Strength = "50mcg", Manufacturer = "MediCorp", Description = "Thyroid hormone replacement", Indications = "Hypothyroidism", Contraindications = "Hyperthyroidism, heart disease", SideEffects = "Palpitations, weight loss", DosageInstructions = "1 tablet daily on empty stomach", Price = 10.00m, StockQuantity = 650, MinimumStockLevel = 80, Unit = "tablet", ExpiryDate = DateTime.UtcNow.AddYears(2), BatchNumber = "LE2024001", RequiresPrescription = true, IsActive = true, CreatedAt = DateTime.UtcNow }
                };

                var medicinesToAdd = essentialMedicines
                    .Where(m => !existingMedicineNames.Contains(m.Name))
                    .ToList();

                if (medicinesToAdd.Any())
                {
                    _context.Medicines.AddRange(medicinesToAdd);
                    await _context.SaveChangesAsync();
                    medicinesAdded = medicinesToAdd.Count;
                }

                return Ok(new
                {
                    message = "Essential data seeded successfully",
                    roomsAdded,
                    medicinesAdded,
                    totalRooms = await _context.Rooms.CountAsync(r => !r.IsDeleted),
                    totalMedicines = await _context.Medicines.CountAsync(m => !m.IsDeleted)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding essential data");
                return StatusCode(500, new { message = "An error occurred while seeding data", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current counts of rooms and medicines
        /// </summary>
        [HttpGet("counts")]
        [AllowAnonymous]
        public async Task<ActionResult> GetDataCounts()
        {
            try
            {
                var counts = new
                {
                    totalRooms = await _context.Rooms.CountAsync(r => !r.IsDeleted),
                    totalMedicines = await _context.Medicines.CountAsync(m => !m.IsDeleted),
                    totalDepartments = await _context.Departments.CountAsync(d => !d.IsDeleted)
                };

                return Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting data counts");
                return StatusCode(500, "An error occurred while getting data counts");
            }
        }
    }
}

