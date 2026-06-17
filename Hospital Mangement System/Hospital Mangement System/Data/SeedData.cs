using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Configuration;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Nurse", "Receptionist", "Pharmacist", "Patient", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUserAsync(
            UserManager<User> userManager,
            IConfiguration configuration,
            Microsoft.Extensions.Logging.ILogger logger,
            bool isDevelopment)
        {
            var adminEmail = "admin@hospital.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
                return;

            var password = SeedPasswordProvider.ResolveForProvisioning(
                configuration, "Admin", logger, allowGeneratedInDevelopment: true, isDevelopment);
            if (string.IsNullOrEmpty(password))
                return;

            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                NationalId = "0000000000",
                PhoneNumber = "1234567890",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                logger.LogWarning("Admin seed failed: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        public static async Task SeedSampleDataAsync(HospitalDbContext context)
        {
            // Departments - Add missing departments
            var existingDeptNames = await context.Departments.Select(d => d.Name).ToListAsync();
            var departments = new[]
            {
                new Department
                {
                    Name = "Cardiology",
                    Description = "Specialized in diagnosing and treating cardiovascular diseases, heart catheterization, open-heart surgery and advanced cardiac care.",
                    HeadOfDepartment = "Dr. Ahmed Hassan",
                    PhoneNumber = "02-1234-5670",
                    Location = "Second Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Neurology",
                    Description = "Specialized in neurological and brain disorders including epilepsy, stroke, chronic headache and nerve diseases.",
                    HeadOfDepartment = "Dr. Sarah Mohamed",
                    PhoneNumber = "02-1234-5671",
                    Location = "Third Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Orthopedics",
                    Description = "Bone, joint and musculoskeletal care including joint replacement, fracture care and sports injuries.",
                    HeadOfDepartment = "Dr. Omar Ali",
                    PhoneNumber = "02-1234-5672",
                    Location = "First Floor - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pediatrics",
                    Description = "Specialized medical care for infants and children, including vaccinations, growth monitoring and pediatric disease management.",
                    HeadOfDepartment = "Dr. Fatima Ahmed",
                    PhoneNumber = "02-1234-5673",
                    Location = "Second Floor - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Emergency",
                    Description = "24/7 emergency and critical care services for accidents, trauma and acute medical conditions.",
                    HeadOfDepartment = "Dr. Khaled Ibrahim",
                    PhoneNumber = "02-1234-5674",
                    Location = "Ground Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Oncology",
                    Description = "Diagnosis and treatment of tumors and cancer including chemotherapy, radiotherapy and surgical oncology.",
                    HeadOfDepartment = "Dr. Mona Elsayed",
                    PhoneNumber = "02-1234-5675",
                    Location = "Fourth Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Radiology",
                    Description = "Diagnostic imaging services including X-ray, MRI, CT scan and ultrasound.",
                    HeadOfDepartment = "Dr. Hesham Farouk",
                    PhoneNumber = "02-1234-5676",
                    Location = "Basement - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Dermatology",
                    Description = "Skin, hair and nail care including treatment of chronic skin conditions and medical cosmetic procedures.",
                    HeadOfDepartment = "Dr. Rania Mostafa",
                    PhoneNumber = "02-1234-5677",
                    Location = "First Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Gastroenterology",
                    Description = "Digestive system care including endoscopy, hepatology, gallbladder and pancreatic disorders.",
                    HeadOfDepartment = "Dr. Mohamed Farid",
                    PhoneNumber = "02-1234-5678",
                    Location = "Second Floor - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pulmonology",
                    Description = "Respiratory and lung care including asthma, bronchitis, chronic lung diseases and pulmonary function testing.",
                    HeadOfDepartment = "Dr. Amira Salah",
                    PhoneNumber = "02-1234-5679",
                    Location = "Third Floor - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Endocrinology",
                    Description = "Endocrine and hormonal disorders including diabetes, thyroid disease, obesity and growth disorders.",
                    HeadOfDepartment = "Dr. Tarek Nabil",
                    PhoneNumber = "02-1234-5680",
                    Location = "Fourth Floor - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Nephrology",
                    Description = "Kidney and urinary tract care including renal failure, kidney transplant and dialysis services.",
                    HeadOfDepartment = "Dr. Laila Kamel",
                    PhoneNumber = "02-1234-5681",
                    Location = "Fifth Floor - Main Building (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Ophthalmology",
                    Description = "Eye and vision care including eye surgery, vision correction, cataract and glaucoma treatment.",
                    HeadOfDepartment = "Dr. Karim Shokry",
                    PhoneNumber = "02-1234-5682",
                    Location = "First Floor - Medical Building (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "ENT",
                    Description = "Ear, nose and throat care including hearing problems, sinusitis and laryngeal surgery.",
                    HeadOfDepartment = "Dr. Noha Samir",
                    PhoneNumber = "02-1234-5683",
                    Location = "Second Floor - Medical Building (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Urology",
                    Description = "Urinary system and male reproductive care including kidney stones, prostate enlargement and urological surgery.",
                    HeadOfDepartment = "Dr. Wael Attia",
                    PhoneNumber = "02-1234-5684",
                    Location = "Third Floor - Medical Building (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Gynecology",
                    Description = "Specialized women's healthcare including routine exams, childbirth and infertility treatment.",
                    HeadOfDepartment = "Dr. Reham Elzahraa",
                    PhoneNumber = "02-1234-5685",
                    Location = "Fourth Floor - Medical Building (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Psychiatry",
                    Description = "Mental health care including depression, anxiety, psychiatric disorders and counseling services.",
                    HeadOfDepartment = "Dr. Samir Aboelfotouh",
                    PhoneNumber = "02-1234-5686",
                    Location = "Fifth Floor - Medical Building (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Anesthesiology",
                    Description = "Anesthesia and pain management including surgical anesthesia, ICU support and chronic pain treatment.",
                    HeadOfDepartment = "Dr. Hany Abdelmoneim",
                    PhoneNumber = "02-1234-5687",
                    Location = "First Floor - Surgical Building (D)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pathology",
                    Description = "Medical laboratory and diagnostic services including blood, tissue, histopathology and comprehensive lab testing.",
                    HeadOfDepartment = "Dr. Magda Elsherbini",
                    PhoneNumber = "02-1234-5688",
                    Location = "Basement - Surgical Building (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
                };

            // Add only departments that don't exist
            var departmentsToAdd = departments.Where(d => !existingDeptNames.Contains(d.Name)).ToList();
            if (departmentsToAdd.Any())
            {
                context.Departments.AddRange(departmentsToAdd);
                await context.SaveChangesAsync();
            }

            // Get department IDs for room assignment
            var deptIds = await context.Departments.ToDictionaryAsync(d => d.Name, d => d.Id);

            // Rooms - Add missing rooms
            var existingRoomNumbers = await context.Rooms.Select(r => r.RoomNumber).ToListAsync();
            var rooms = new[]
            {
                new Room
                {
                    RoomNumber = "C101",
                    RoomType = "Consultation",
                    Floor = "Floor 2",
                    Building = "Building A",
                    Description = "Cardiology consultation room",
                    Capacity = 1,
                    HourlyRate = 100,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Cardiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "N201",
                    RoomType = "Consultation",
                    Floor = "Floor 3",
                    Building = "Building A",
                    Description = "Neurology consultation room",
                    Capacity = 1,
                    HourlyRate = 120,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Neurology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "O101",
                    RoomType = "Consultation",
                    Floor = "Floor 1",
                    Building = "Building B",
                    Description = "Orthopedics consultation room",
                    Capacity = 1,
                    HourlyRate = 110,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Orthopedics", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "P201",
                    RoomType = "Consultation",
                    Floor = "Floor 2",
                    Building = "Building B",
                    Description = "Pediatrics consultation room",
                    Capacity = 1,
                    HourlyRate = 90,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Pediatrics", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "E001",
                    RoomType = "Emergency",
                    Floor = "Ground Floor",
                    Building = "Building A",
                    Description = "Emergency room",
                    Capacity = 4,
                    HourlyRate = 200,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Emergency", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "C102",
                    RoomType = "Consultation",
                    Floor = "Floor 2",
                    Building = "Building A",
                    Description = "Cardiology consultation room 2",
                    Capacity = 1,
                    HourlyRate = 100,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Cardiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "ICU-001",
                    RoomType = "ICU",
                    Floor = "Floor 5",
                    Building = "Building A",
                    Description = "Intensive Care Unit room 1",
                    Capacity = 1,
                    HourlyRate = 500,
                    IsAvailable = false,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Cardiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "ICU-002",
                    RoomType = "ICU",
                    Floor = "Floor 5",
                    Building = "Building A",
                    Description = "Intensive Care Unit room 2",
                    Capacity = 1,
                    HourlyRate = 500,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Emergency", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "SURG-001",
                    RoomType = "Surgery",
                    Floor = "Floor 3",
                    Building = "Building B",
                    Description = "Surgery room 1",
                    Capacity = 10,
                    HourlyRate = 800,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Orthopedics", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "SURG-002",
                    RoomType = "Surgery",
                    Floor = "Floor 3",
                    Building = "Building B",
                    Description = "Surgery room 2",
                    Capacity = 10,
                    HourlyRate = 800,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Orthopedics", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "WARD-101",
                    RoomType = "Ward",
                    Floor = "Floor 4",
                    Building = "Building A",
                    Description = "General ward room 1",
                    Capacity = 4,
                    HourlyRate = 150,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Cardiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "WARD-102",
                    RoomType = "Ward",
                    Floor = "Floor 4",
                    Building = "Building A",
                    Description = "General ward room 2",
                    Capacity = 4,
                    HourlyRate = 150,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Neurology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "WARD-103",
                    RoomType = "Ward",
                    Floor = "Floor 4",
                    Building = "Building A",
                    Description = "General ward room 3",
                    Capacity = 4,
                    HourlyRate = 150,
                    IsAvailable = false,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Pediatrics", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "G101",
                    RoomType = "Consultation",
                    Floor = "Floor 2",
                    Building = "Building B",
                    Description = "Gastroenterology consultation room",
                    Capacity = 1,
                    HourlyRate = 130,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Gastroenterology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "PUL-201",
                    RoomType = "Consultation",
                    Floor = "Floor 3",
                    Building = "Building B",
                    Description = "Pulmonology consultation room",
                    Capacity = 1,
                    HourlyRate = 140,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Pulmonology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "END-301",
                    RoomType = "Consultation",
                    Floor = "Floor 4",
                    Building = "Building B",
                    Description = "Endocrinology consultation room",
                    Capacity = 1,
                    HourlyRate = 135,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Endocrinology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "NEP-401",
                    RoomType = "Consultation",
                    Floor = "Floor 5",
                    Building = "Building A",
                    Description = "Nephrology consultation room",
                    Capacity = 1,
                    HourlyRate = 145,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Nephrology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "OPH-101",
                    RoomType = "Consultation",
                    Floor = "Floor 1",
                    Building = "Building C",
                    Description = "Ophthalmology consultation room",
                    Capacity = 1,
                    HourlyRate = 160,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Ophthalmology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "ENT-201",
                    RoomType = "Consultation",
                    Floor = "Floor 2",
                    Building = "Building C",
                    Description = "ENT consultation room",
                    Capacity = 1,
                    HourlyRate = 125,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("ENT", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "URO-301",
                    RoomType = "Consultation",
                    Floor = "Floor 3",
                    Building = "Building C",
                    Description = "Urology consultation room",
                    Capacity = 1,
                    HourlyRate = 140,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Urology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "GYN-401",
                    RoomType = "Consultation",
                    Floor = "Floor 4",
                    Building = "Building C",
                    Description = "Gynecology consultation room",
                    Capacity = 1,
                    HourlyRate = 150,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Gynecology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "PSY-501",
                    RoomType = "Consultation",
                    Floor = "Floor 5",
                    Building = "Building C",
                    Description = "Psychiatry consultation room",
                    Capacity = 1,
                    HourlyRate = 180,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Psychiatry", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "E002",
                    RoomType = "Emergency",
                    Floor = "Ground Floor",
                    Building = "Building A",
                    Description = "Emergency room 2",
                    Capacity = 4,
                    HourlyRate = 200,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Emergency", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "E003",
                    RoomType = "Emergency",
                    Floor = "Ground Floor",
                    Building = "Building A",
                    Description = "Emergency room 3",
                    Capacity = 4,
                    HourlyRate = 200,
                    IsAvailable = false,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Emergency", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "RAD-001",
                    RoomType = "Radiology",
                    Floor = "Basement",
                    Building = "Building A",
                    Description = "X-Ray room",
                    Capacity = 1,
                    HourlyRate = 300,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Radiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "RAD-002",
                    RoomType = "Radiology",
                    Floor = "Basement",
                    Building = "Building A",
                    Description = "MRI room",
                    Capacity = 1,
                    HourlyRate = 500,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Radiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "RAD-003",
                    RoomType = "Radiology",
                    Floor = "Basement",
                    Building = "Building A",
                    Description = "CT Scan room",
                    Capacity = 1,
                    HourlyRate = 450,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Radiology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "ONC-401",
                    RoomType = "Consultation",
                    Floor = "Floor 4",
                    Building = "Building A",
                    Description = "Oncology consultation room",
                    Capacity = 1,
                    HourlyRate = 250,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Oncology", 0),
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    RoomNumber = "DER-101",
                    RoomType = "Consultation",
                    Floor = "Floor 1",
                    Building = "Building A",
                    Description = "Dermatology consultation room",
                    Capacity = 1,
                    HourlyRate = 115,
                    IsAvailable = true,
                    IsActive = true,
                    DepartmentId = deptIds.GetValueOrDefault("Dermatology", 0),
                    CreatedAt = DateTime.UtcNow
                }
                };

            // Add only rooms that don't exist
            var roomsToAdd = rooms.Where(r => !existingRoomNumbers.Contains(r.RoomNumber) && r.DepartmentId > 0).ToList();
            if (roomsToAdd.Any())
            {
                context.Rooms.AddRange(roomsToAdd);
                await context.SaveChangesAsync();
            }

            // Medicines - Add missing medicines
            var existingMedicineNames = await context.Medicines.Select(m => m.Name).ToListAsync();
            var medicines = new[]
            {
                new Medicine
                {
                    Name = "Paracetamol",
                    GenericName = "Acetaminophen",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    Manufacturer = "PharmaCorp",
                    Description = "Pain reliever and fever reducer",
                    Indications = "Headache, fever, muscle pain",
                    Contraindications = "Liver disease, alcohol use",
                    SideEffects = "Nausea, rash",
                    DosageInstructions = "1-2 tablets every 4-6 hours",
                    Price = 5.00m,
                    StockQuantity = 1000,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "PC2024001",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Amoxicillin",
                    GenericName = "Amoxicillin",
                    DosageForm = "Capsule",
                    Strength = "250mg",
                    Manufacturer = "MediCorp",
                    Description = "Antibiotic for bacterial infections",
                    Indications = "Respiratory infections, skin infections",
                    Contraindications = "Penicillin allergy",
                    SideEffects = "Diarrhea, nausea, rash",
                    DosageInstructions = "1 capsule every 8 hours for 7 days",
                    Price = 15.00m,
                    StockQuantity = 500,
                    MinimumStockLevel = 50,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "MC2024001",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Ibuprofen",
                    GenericName = "Ibuprofen",
                    DosageForm = "Tablet",
                    Strength = "400mg",
                    Manufacturer = "HealthPharma",
                    Description = "Anti-inflammatory and pain reliever",
                    Indications = "Inflammation, pain, fever",
                    Contraindications = "Stomach ulcers, heart disease",
                    SideEffects = "Stomach upset, dizziness",
                    DosageInstructions = "1 tablet every 6-8 hours",
                    Price = 8.00m,
                    StockQuantity = 60, // low stock
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "HP2024001",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Metformin",
                    GenericName = "Metformin Hydrochloride",
                    DosageForm = "Tablet",
                    Strength = "850mg",
                    Manufacturer = "GlucoPharm",
                    Description = "Antidiabetic",
                    Indications = "Type 2 Diabetes",
                    Price = 12.50m,
                    StockQuantity = 45, // low stock
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    BatchNumber = "GF2024002",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Lisinopril",
                    GenericName = "Lisinopril",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    Manufacturer = "CardioMed",
                    Description = "ACE inhibitor",
                    Indications = "Hypertension",
                    Price = 9.75m,
                    StockQuantity = 30, // low stock
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    BatchNumber = "CM2024007",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Omeprazole",
                    GenericName = "Omeprazole",
                    DosageForm = "Capsule",
                    Strength = "20mg",
                    Manufacturer = "GastroHeal",
                    Description = "Proton pump inhibitor",
                    Indications = "Acid reflux",
                    Price = 11.00m,
                    StockQuantity = 300,
                    MinimumStockLevel = 100,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "GH2024009",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Atorvastatin",
                    GenericName = "Atorvastatin",
                    DosageForm = "Tablet",
                    Strength = "20mg",
                    Manufacturer = "LipoPharm",
                    Description = "Statin",
                    Indications = "High cholesterol",
                    Price = 14.20m,
                    StockQuantity = 75, // low stock
                    MinimumStockLevel = 150,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "LP2024012",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Azithromycin",
                    GenericName = "Azithromycin",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    Manufacturer = "MacroMed",
                    Description = "Antibiotic",
                    Indications = "Respiratory infections",
                    Price = 22.00m,
                    StockQuantity = 40, // low stock
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    BatchNumber = "MM2024015",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Cetirizine",
                    GenericName = "Cetirizine Hydrochloride",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    Manufacturer = "AllerGo",
                    Description = "Antihistamine",
                    Indications = "Allergies",
                    Price = 6.00m,
                    StockQuantity = 200,
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "AG2024016",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Salbutamol Inhaler",
                    GenericName = "Albuterol",
                    DosageForm = "Inhaler",
                    Strength = "100mcg/dose",
                    Manufacturer = "Respira",
                    Description = "Bronchodilator",
                    Indications = "Asthma",
                    Price = 55.00m,
                    StockQuantity = 20, // low stock
                    MinimumStockLevel = 60,
                    Unit = "device",
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    BatchNumber = "RS2024017",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Insulin Glargine",
                    GenericName = "Insulin Glargine",
                    DosageForm = "Injection",
                    Strength = "100 IU/ml",
                    Manufacturer = "GlucoCare",
                    Description = "Long-acting insulin",
                    Indications = "Diabetes",
                    Price = 120.00m,
                    StockQuantity = 15, // low stock
                    MinimumStockLevel = 50,
                    Unit = "vial",
                    ExpiryDate = DateTime.UtcNow.AddMonths(18),
                    BatchNumber = "GC2024018",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Losartan",
                    GenericName = "Losartan Potassium",
                    DosageForm = "Tablet",
                    Strength = "50mg",
                    Manufacturer = "CardioMed",
                    Description = "ARB",
                    Indications = "Hypertension",
                    Price = 10.00m,
                    StockQuantity = 70, // low stock
                    MinimumStockLevel = 120,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CM2024019",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Vitamin D3",
                    GenericName = "Cholecalciferol",
                    DosageForm = "Capsule",
                    Strength = "1000 IU",
                    Manufacturer = "NutriLife",
                    Description = "Supplement",
                    Indications = "Deficiency",
                    Price = 5.50m,
                    StockQuantity = 500,
                    MinimumStockLevel = 100,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "NL2024020",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Warfarin",
                    GenericName = "Warfarin Sodium",
                    DosageForm = "Tablet",
                    Strength = "5mg",
                    Manufacturer = "Coagulo",
                    Description = "Anticoagulant",
                    Indications = "Thrombosis prevention",
                    Price = 18.00m,
                    StockQuantity = 25, // low stock
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CG2024021",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Aspirin",
                    GenericName = "Acetylsalicylic Acid",
                    DosageForm = "Tablet",
                    Strength = "100mg",
                    Manufacturer = "CardioPharm",
                    Description = "Antiplatelet and pain reliever",
                    Indications = "Heart attack prevention, pain relief",
                    Contraindications = "Peptic ulcers, bleeding disorders",
                    SideEffects = "Stomach irritation, bleeding",
                    DosageInstructions = "1 tablet daily with food",
                    Price = 3.50m,
                    StockQuantity = 800,
                    MinimumStockLevel = 200,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CP2024022",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Ciprofloxacin",
                    GenericName = "Ciprofloxacin",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    Manufacturer = "AntibioCorp",
                    Description = "Broad-spectrum antibiotic",
                    Indications = "Urinary tract infections, respiratory infections",
                    Contraindications = "Children, pregnancy",
                    SideEffects = "Nausea, diarrhea, tendonitis",
                    DosageInstructions = "1 tablet twice daily for 7-10 days",
                    Price = 25.00m,
                    StockQuantity = 350,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "AC2024023",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Levothyroxine",
                    GenericName = "Levothyroxine Sodium",
                    DosageForm = "Tablet",
                    Strength = "50mcg",
                    Manufacturer = "ThyroMed",
                    Description = "Thyroid hormone replacement",
                    Indications = "Hypothyroidism",
                    Contraindications = "Hyperthyroidism, heart disease",
                    SideEffects = "Palpitations, weight loss",
                    DosageInstructions = "1 tablet daily on empty stomach",
                    Price = 8.50m,
                    StockQuantity = 400,
                    MinimumStockLevel = 150,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "TM2024024",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Amlodipine",
                    GenericName = "Amlodipine Besylate",
                    DosageForm = "Tablet",
                    Strength = "5mg",
                    Manufacturer = "CardioMed",
                    Description = "Calcium channel blocker",
                    Indications = "Hypertension, angina",
                    Contraindications = "Severe hypotension",
                    SideEffects = "Swelling, dizziness, fatigue",
                    DosageInstructions = "1 tablet daily",
                    Price = 7.00m,
                    StockQuantity = 280,
                    MinimumStockLevel = 120,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CM2024025",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Metronidazole",
                    GenericName = "Metronidazole",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    Manufacturer = "AntibioCorp",
                    Description = "Antibiotic and antiprotozoal",
                    Indications = "Bacterial infections, parasitic infections",
                    Contraindications = "First trimester pregnancy, alcohol use",
                    SideEffects = "Nausea, metallic taste, dark urine",
                    DosageInstructions = "1 tablet three times daily for 7 days",
                    Price = 12.00m,
                    StockQuantity = 220,
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "AC2024026",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Simvastatin",
                    GenericName = "Simvastatin",
                    DosageForm = "Tablet",
                    Strength = "20mg",
                    Manufacturer = "LipoPharm",
                    Description = "HMG-CoA reductase inhibitor",
                    Indications = "High cholesterol, cardiovascular disease prevention",
                    Contraindications = "Liver disease, pregnancy",
                    SideEffects = "Muscle pain, liver problems",
                    DosageInstructions = "1 tablet daily in evening",
                    Price = 11.50m,
                    StockQuantity = 180,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "LP2024027",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Furosemide",
                    GenericName = "Furosemide",
                    DosageForm = "Tablet",
                    Strength = "40mg",
                    Manufacturer = "DiuroMed",
                    Description = "Loop diuretic",
                    Indications = "Edema, hypertension, heart failure",
                    Contraindications = "Anuria, severe electrolyte imbalance",
                    SideEffects = "Dehydration, electrolyte imbalance, dizziness",
                    DosageInstructions = "1 tablet daily or as directed",
                    Price = 4.50m,
                    StockQuantity = 320,
                    MinimumStockLevel = 120,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "DM2024028",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Diclofenac",
                    GenericName = "Diclofenac Sodium",
                    DosageForm = "Tablet",
                    Strength = "50mg",
                    Manufacturer = "PainRelief",
                    Description = "NSAID pain reliever",
                    Indications = "Pain, inflammation, arthritis",
                    Contraindications = "Peptic ulcers, kidney disease, heart disease",
                    SideEffects = "Stomach upset, kidney problems",
                    DosageInstructions = "1 tablet 2-3 times daily with food",
                    Price = 9.00m,
                    StockQuantity = 450,
                    MinimumStockLevel = 150,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "PR2024029",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Cephalexin",
                    GenericName = "Cephalexin",
                    DosageForm = "Capsule",
                    Strength = "500mg",
                    Manufacturer = "MediCorp",
                    Description = "Cephalosporin antibiotic",
                    Indications = "Bacterial infections, skin infections",
                    Contraindications = "Penicillin allergy",
                    SideEffects = "Diarrhea, nausea, rash",
                    DosageInstructions = "1 capsule every 6 hours for 7-14 days",
                    Price = 18.00m,
                    StockQuantity = 380,
                    MinimumStockLevel = 100,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "MC2024030",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Metoclopramide",
                    GenericName = "Metoclopramide",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    Manufacturer = "GastroHeal",
                    Description = "Anti-emetic and prokinetic",
                    Indications = "Nausea, vomiting, gastroparesis",
                    Contraindications = "Gastrointestinal obstruction",
                    SideEffects = "Drowsiness, restlessness, extrapyramidal symptoms",
                    DosageInstructions = "1 tablet 3-4 times daily before meals",
                    Price = 5.00m,
                    StockQuantity = 260,
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "GH2024031",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Ranitidine",
                    GenericName = "Ranitidine Hydrochloride",
                    DosageForm = "Tablet",
                    Strength = "150mg",
                    Manufacturer = "GastroHeal",
                    Description = "H2 receptor antagonist",
                    Indications = "Peptic ulcers, GERD",
                    Contraindications = "Hypersensitivity",
                    SideEffects = "Headache, dizziness, constipation",
                    DosageInstructions = "1 tablet twice daily",
                    Price = 6.50m,
                    StockQuantity = 340,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "GH2024032",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Hydrochlorothiazide",
                    GenericName = "Hydrochlorothiazide",
                    DosageForm = "Tablet",
                    Strength = "25mg",
                    Manufacturer = "CardioMed",
                    Description = "Thiazide diuretic",
                    Indications = "Hypertension, edema",
                    Contraindications = "Anuria, severe renal impairment",
                    SideEffects = "Dehydration, electrolyte imbalance, photosensitivity",
                    DosageInstructions = "1 tablet daily in morning",
                    Price = 3.00m,
                    StockQuantity = 290,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CM2024033",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Montelukast",
                    GenericName = "Montelukast Sodium",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    Manufacturer = "Respira",
                    Description = "Leukotriene receptor antagonist",
                    Indications = "Asthma, allergic rhinitis",
                    Contraindications = "Hypersensitivity",
                    SideEffects = "Headache, nausea, sleep disturbances",
                    DosageInstructions = "1 tablet daily in evening",
                    Price = 35.00m,
                    StockQuantity = 150,
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "RS2024034",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Loratadine",
                    GenericName = "Loratadine",
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    Manufacturer = "AllerGo",
                    Description = "Antihistamine",
                    Indications = "Allergic rhinitis, urticaria",
                    Contraindications = "Hypersensitivity",
                    SideEffects = "Drowsiness (rare), dry mouth",
                    DosageInstructions = "1 tablet daily",
                    Price = 7.50m,
                    StockQuantity = 420,
                    MinimumStockLevel = 150,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "AG2024035",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Calcium Carbonate",
                    GenericName = "Calcium Carbonate",
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    Manufacturer = "NutriLife",
                    Description = "Calcium supplement",
                    Indications = "Calcium deficiency, osteoporosis prevention",
                    Contraindications = "Hypercalcemia, kidney stones",
                    SideEffects = "Constipation, gas",
                    DosageInstructions = "1-2 tablets daily with meals",
                    Price = 4.00m,
                    StockQuantity = 600,
                    MinimumStockLevel = 200,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "NL2024036",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Ferrous Sulfate",
                    GenericName = "Ferrous Sulfate",
                    DosageForm = "Tablet",
                    Strength = "325mg",
                    Manufacturer = "NutriLife",
                    Description = "Iron supplement",
                    Indications = "Iron deficiency anemia",
                    Contraindications = "Hemochromatosis, hemosiderosis",
                    SideEffects = "Constipation, dark stools, nausea",
                    DosageInstructions = "1 tablet daily with vitamin C",
                    Price = 5.50m,
                    StockQuantity = 380,
                    MinimumStockLevel = 120,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "NL2024037",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Prednisolone",
                    GenericName = "Prednisolone",
                    DosageForm = "Tablet",
                    Strength = "5mg",
                    Manufacturer = "SteroidPharm",
                    Description = "Corticosteroid",
                    Indications = "Inflammation, autoimmune disorders, allergies",
                    Contraindications = "Systemic fungal infections",
                    SideEffects = "Weight gain, mood changes, increased infection risk",
                    DosageInstructions = "As directed by physician",
                    Price = 8.00m,
                    StockQuantity = 240,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "SP2024038",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Atenolol",
                    GenericName = "Atenolol",
                    DosageForm = "Tablet",
                    Strength = "50mg",
                    Manufacturer = "CardioMed",
                    Description = "Beta-blocker",
                    Indications = "Hypertension, angina, arrhythmia",
                    Contraindications = "Heart failure, asthma, bradycardia",
                    SideEffects = "Fatigue, cold hands, dizziness",
                    DosageInstructions = "1 tablet daily",
                    Price = 6.00m,
                    StockQuantity = 310,
                    MinimumStockLevel = 120,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "CM2024039",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Fluconazole",
                    GenericName = "Fluconazole",
                    DosageForm = "Capsule",
                    Strength = "150mg",
                    Manufacturer = "FungiMed",
                    Description = "Antifungal medication",
                    Indications = "Fungal infections, candidiasis",
                    Contraindications = "Hypersensitivity, pregnancy",
                    SideEffects = "Nausea, headache, rash",
                    DosageInstructions = "1 capsule daily for 7-14 days",
                    Price = 28.00m,
                    StockQuantity = 180,
                    MinimumStockLevel = 80,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "FM2024040",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Diphenhydramine",
                    GenericName = "Diphenhydramine",
                    DosageForm = "Tablet",
                    Strength = "25mg",
                    Manufacturer = "AllerGo",
                    Description = "Antihistamine and sedative",
                    Indications = "Allergies, insomnia, motion sickness",
                    Contraindications = "Glaucoma, urinary retention",
                    SideEffects = "Drowsiness, dry mouth, dizziness",
                    DosageInstructions = "1 tablet every 4-6 hours as needed",
                    Price = 4.50m,
                    StockQuantity = 500,
                    MinimumStockLevel = 150,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "AG2024041",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Gliclazide",
                    GenericName = "Gliclazide",
                    DosageForm = "Tablet",
                    Strength = "80mg",
                    Manufacturer = "GlucoPharm",
                    Description = "Sulfonylurea antidiabetic",
                    Indications = "Type 2 Diabetes",
                    Contraindications = "Type 1 diabetes, severe renal impairment",
                    SideEffects = "Hypoglycemia, weight gain",
                    DosageInstructions = "1-2 tablets daily before meals",
                    Price = 15.00m,
                    StockQuantity = 200,
                    MinimumStockLevel = 100,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "GF2024042",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Ciprofloxacin Eye Drops",
                    GenericName = "Ciprofloxacin",
                    DosageForm = "Eye Drops",
                    Strength = "0.3%",
                    Manufacturer = "OphthaMed",
                    Description = "Antibiotic eye drops",
                    Indications = "Bacterial eye infections, conjunctivitis",
                    Contraindications = "Hypersensitivity",
                    SideEffects = "Eye irritation, burning sensation",
                    DosageInstructions = "1-2 drops every 2-4 hours",
                    Price = 32.00m,
                    StockQuantity = 120,
                    MinimumStockLevel = 50,
                    Unit = "bottle",
                    ExpiryDate = DateTime.UtcNow.AddMonths(18),
                    BatchNumber = "OM2024043",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Tramadol",
                    GenericName = "Tramadol Hydrochloride",
                    DosageForm = "Capsule",
                    Strength = "50mg",
                    Manufacturer = "PainRelief",
                    Description = "Opioid analgesic",
                    Indications = "Moderate to severe pain",
                    Contraindications = "Respiratory depression, acute intoxication",
                    SideEffects = "Dizziness, nausea, constipation, drowsiness",
                    DosageInstructions = "1-2 capsules every 4-6 hours",
                    Price = 20.00m,
                    StockQuantity = 90,
                    MinimumStockLevel = 60,
                    Unit = "capsule",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "PR2024044",
                    RequiresPrescription = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    Name = "Folic Acid",
                    GenericName = "Folic Acid",
                    DosageForm = "Tablet",
                    Strength = "5mg",
                    Manufacturer = "NutriLife",
                    Description = "B vitamin supplement",
                    Indications = "Folate deficiency, pregnancy, anemia",
                    Contraindications = "Pernicious anemia",
                    SideEffects = "Rare - allergic reactions",
                    DosageInstructions = "1 tablet daily",
                    Price = 3.00m,
                    StockQuantity = 550,
                    MinimumStockLevel = 200,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "NL2024045",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
                };

            // Add only medicines that don't exist
            var medicinesToAdd = medicines.Where(m => !existingMedicineNames.Contains(m.Name)).ToList();
            if (medicinesToAdd.Any())
            {
                context.Medicines.AddRange(medicinesToAdd);
                await context.SaveChangesAsync();
            }

            // Doctors
            if (!await context.Doctors.AnyAsync())
            {
                var doctors = new[]
                {
                    new Doctor
                    {
                        FirstName = "Ahmed",
                        LastName = "Hassan",
                        NationalId = "2111111111",
                        Email = "ahmed.hassan@hospital.com",
                        PhoneNumber = "01001234567",
                        DateOfBirth = new DateTime(1980, 3, 20),
                        Gender = "Male",
                        Address = "Second Floor - Main Building (A)",
                        LicenseNumber = "LIC-CARD-001",
                        Specialization = "Cardiology",
                        SubSpecialization = "Interventional Cardiac Catheterization",
                        YearsOfExperience = 15,
                        Education = "PhD",
                        Certifications = "Board Certified by the Medical Council",
                        Languages = "Arabic, English",
                        ConsultationFee = 250,
                        WorkingHoursStart = new TimeSpan(9, 0, 0),
                        WorkingHoursEnd = new TimeSpan(17, 0, 0),
                        DepartmentId = 1,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Doctor
                    {
                        FirstName = "Sarah",
                        LastName = "Mohamed",
                        NationalId = "2222222222",
                        Email = "sarah.mohamed@hospital.com",
                        PhoneNumber = "01002345678",
                        DateOfBirth = new DateTime(1982, 6, 12),
                        Gender = "Female",
                        Address = "Third Floor - Main Building (A)",
                        LicenseNumber = "LIC-NEURO-001",
                        Specialization = "Neurology",
                        SubSpecialization = "Epileptology",
                        YearsOfExperience = 13,
                        Education = "PhD",
                        Certifications = "Medical Fellowship",
                        Languages = "Arabic, English",
                        ConsultationFee = 300,
                        WorkingHoursStart = new TimeSpan(10, 0, 0),
                        WorkingHoursEnd = new TimeSpan(18, 0, 0),
                        DepartmentId = 2,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Doctor
                    {
                        FirstName = "Omar",
                        LastName = "Ali",
                        NationalId = "2333333333",
                        Email = "omar.ali@hospital.com",
                        PhoneNumber = "01003456789",
                        DateOfBirth = new DateTime(1985, 11, 5),
                        Gender = "Male",
                        Address = "First Floor - Surgical Building (B)",
                        LicenseNumber = "LIC-ORTHO-001",
                        Specialization = "Orthopedics",
                        SubSpecialization = "Bone & Joint Surgery",
                        YearsOfExperience = 10,
                        Education = "Master's",
                        Certifications = "Specialty Certification",
                        Languages = "Arabic, English",
                        ConsultationFee = 220,
                        WorkingHoursStart = new TimeSpan(8, 30, 0),
                        WorkingHoursEnd = new TimeSpan(16, 30, 0),
                        DepartmentId = 3,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };
                context.Doctors.AddRange(doctors);
                await context.SaveChangesAsync();
            }

            // Staff (Nurses, Receptionists, Lab Technicians, etc.)
            if (!await context.Staff.AnyAsync())
            {
                var staffList = new[]
                {
                    new Staff
                    {
                        FirstName = "Mariam",
                        LastName = "Ibrahim",
                        NationalId = "2444444444",
                        Email = "mariam.ibrahim@hospital.com",
                        PhoneNumber = "01004567890",
                        DateOfBirth = new DateTime(1990, 4, 18),
                        Gender = "Female",
                        Address = "Nasr City, Cairo, Egypt",
                        Position = "Head Nurse",
                        EmployeeId = "EMP-NUR-001",
                        HireDate = new DateTime(2018, 6, 1),
                        Salary = 8000,
                        Qualification = "Bachelor of Nursing",
                        Skills = "Patient care, IV insertion, Emergency response",
                        WorkingHoursStart = new TimeSpan(8, 0, 0),
                        WorkingHoursEnd = new TimeSpan(16, 0, 0),
                        DepartmentId = 1,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Staff
                    {
                        FirstName = "Karim",
                        LastName = "Saeed",
                        NationalId = "2555555555",
                        Email = "karim.saeed@hospital.com",
                        PhoneNumber = "01005678901",
                        DateOfBirth = new DateTime(1988, 9, 25),
                        Gender = "Male",
                        Address = "Heliopolis, Cairo, Egypt",
                        Position = "Receptionist",
                        EmployeeId = "EMP-REC-001",
                        HireDate = new DateTime(2019, 3, 15),
                        Salary = 5500,
                        Qualification = "Bachelor of Business Administration",
                        Skills = "Customer service, Scheduling, Data entry",
                        WorkingHoursStart = new TimeSpan(9, 0, 0),
                        WorkingHoursEnd = new TimeSpan(17, 0, 0),
                        DepartmentId = 5,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Staff
                    {
                        FirstName = "Layla",
                        LastName = "Rashad",
                        NationalId = "2666666666",
                        Email = "layla.rashad@hospital.com",
                        PhoneNumber = "01006789012",
                        DateOfBirth = new DateTime(1992, 12, 3),
                        Gender = "Female",
                        Address = "Maadi, Cairo, Egypt",
                        Position = "Lab Technician",
                        EmployeeId = "EMP-LAB-001",
                        HireDate = new DateTime(2020, 1, 10),
                        Salary = 6500,
                        Qualification = "Bachelor of Medical Laboratory Sciences",
                        Skills = "Blood analysis, Microscopy, Lab equipment operation",
                        WorkingHoursStart = new TimeSpan(8, 0, 0),
                        WorkingHoursEnd = new TimeSpan(16, 0, 0),
                        DepartmentId = 7,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Staff
                    {
                        FirstName = "Tarek",
                        LastName = "Nabil",
                        NationalId = "2777777777",
                        Email = "tarek.nabil@hospital.com",
                        PhoneNumber = "01007890123",
                        DateOfBirth = new DateTime(1985, 7, 14),
                        Gender = "Male",
                        Address = "Zamalek, Cairo, Egypt",
                        Position = "Pharmacist",
                        EmployeeId = "EMP-PHA-001",
                        HireDate = new DateTime(2017, 9, 1),
                        Salary = 9000,
                        Qualification = "Bachelor of Pharmacy",
                        Skills = "Drug dispensing, Patient counseling, Inventory management",
                        WorkingHoursStart = new TimeSpan(9, 0, 0),
                        WorkingHoursEnd = new TimeSpan(17, 0, 0),
                        DepartmentId = 5,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };
                context.Staff.AddRange(staffList);
                await context.SaveChangesAsync();
            }

            // Patients
            if (!await context.Patients.AnyAsync())
            {
                var patients = new[]
                {
                    new Patient
                    {
                        FirstName = "Mohamed",
                        LastName = "Youssef",
                        NationalId = "2987654321",
                        Email = "mohamed.youssef@hospital.com",
                        PhoneNumber = "01123456789",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        Gender = "Male",
                        Address = "Nile Street, Cairo, Egypt",
                        EmergencyContactName = "Ahmed Youssef",
                        EmergencyContactPhone = "01234567890",
                        InsuranceProvider = "National Health Insurance",
                        InsuranceNumber = "INS-0001",
                        MedicalHistory = "No chronic conditions",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "Salma",
                        LastName = "Khaled",
                        NationalId = "2876543210",
                        Email = "salma.khaled@hospital.com",
                        PhoneNumber = "01198765432",
                        DateOfBirth = new DateTime(1992, 3, 10),
                        Gender = "Female",
                        Address = "Tahrir Street, Giza, Egypt",
                        EmergencyContactName = "Khaled Mahmoud",
                        EmergencyContactPhone = "01298765432",
                        InsuranceProvider = "Comprehensive Health Insurance",
                        InsuranceNumber = "INS-0002",
                        Allergies = "Penicillin",
                        MedicalHistory = "No chronic conditions",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };
                context.Patients.AddRange(patients);
                await context.SaveChangesAsync();
            }

            // Appointments
            if (!await context.Appointments.AnyAsync())
            {
                var patientIdsSeed = await context.Patients.Select(p => p.Id).OrderBy(id => id).ToListAsync();
                var doctorIdsSeed = await context.Doctors.Select(d => d.Id).OrderBy(id => id).ToListAsync();
                var roomIdsSeed = await context.Rooms.Select(r => r.Id).OrderBy(id => id).ToListAsync();

                if (patientIdsSeed.Count >= 1 && doctorIdsSeed.Count >= 1)
                {
                    var firstRoom = roomIdsSeed.FirstOrDefault();
                    var appt1 = new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date.AddDays(1),
                        AppointmentTime = new TimeSpan(10,0,0),
                        Status = "Scheduled",
                        Reason = "Chest pain",
                        Notes = "Patient reports intermittent pain",
                        ConsultationFee = 250,
                        PatientId = patientIdsSeed[0],
                        DoctorId = doctorIdsSeed[0],
                        RoomId = firstRoom == 0 ? null : firstRoom,
                        CreatedAt = DateTime.UtcNow
                    };

                    Appointment? appt2 = null;
                    if (patientIdsSeed.Count >= 2 && doctorIdsSeed.Count >= 2)
                    {
                        var room2 = roomIdsSeed.Skip(1).FirstOrDefault();
                        appt2 = new Appointment
                        {
                            AppointmentDate = DateTime.UtcNow.Date.AddDays(-1),
                            AppointmentTime = new TimeSpan(11,30,0),
                            Status = "Completed",
                            Reason = "Headache",
                            Diagnosis = "Migraine",
                            Treatment = "Pain management",
                            ConsultationFee = 300,
                            PatientId = patientIdsSeed[1],
                            DoctorId = doctorIdsSeed[1],
                            RoomId = room2 == 0 ? firstRoom : room2,
                            CreatedAt = DateTime.UtcNow
                        };
                    }

                    var toInsert = new List<Appointment> { appt1 };
                    if (appt2 != null) toInsert.Add(appt2);
                    context.Appointments.AddRange(toInsert);
                    await context.SaveChangesAsync();
                }
            }

            // Medical Records
            if (!await context.MedicalRecords.AnyAsync())
            {
                var anyPatientId = await context.Patients.Select(p => p.Id).OrderBy(id => id).FirstOrDefaultAsync();
                var anyDoctorId = await context.Doctors.Select(d => d.Id).OrderBy(id => id).FirstOrDefaultAsync();
                if (anyPatientId != 0 && anyDoctorId != 0)
                {
                    context.MedicalRecords.Add(new MedicalRecord
                    {
                        RecordDate = DateTime.UtcNow.Date.AddDays(-1),
                        RecordType = "Consultation",
                        Symptoms = "Headache",
                        Diagnosis = "Migraine",
                        Treatment = "Rest and analgesics",
                        Notes = "Follow up in 2 weeks",
                        VitalSigns = "BP 120/80, HR 72",
                        BloodPressure = "120/80",
                        Temperature = "36.8°C",
                        HeartRate = "72",
                        Weight = "70kg",
                        Height = "170cm",
                        PatientId = anyPatientId,
                        DoctorId = anyDoctorId,
                        CreatedAt = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Prescriptions
            if (!await context.Prescriptions.AnyAsync())
            {
                var firstPatientId = await context.Patients.Select(p => p.Id).OrderBy(id => id).FirstOrDefaultAsync();
                var firstDoctorId = await context.Doctors.Select(d => d.Id).OrderBy(id => id).FirstOrDefaultAsync();
                var firstMed = await context.Medicines.Select(m => new { m.Id, m.Name, m.Price }).OrderBy(m => m.Id).FirstOrDefaultAsync();
                if (firstPatientId != 0 && firstDoctorId != 0 && firstMed != null)
                {
                    var prescription = new Prescription
                    {
                        PrescriptionDate = DateTime.UtcNow.Date,
                        ValidUntil = DateTime.UtcNow.Date.AddDays(7),
                        Instructions = "Take with food",
                        Notes = "As needed",
                        PatientId = firstPatientId,
                        DoctorId = firstDoctorId,
                        CreatedAt = DateTime.UtcNow,
                        PrescriptionItems = new List<PrescriptionItem>
                        {
                            new PrescriptionItem
                            {
                                MedicineName = firstMed.Name,
                                Dosage = "500mg",
                                Frequency = "Twice daily",
                                Duration = "7 days",
                                Instructions = "Take with food",
                                Quantity = 14,
                                UnitPrice = firstMed.Price,
                                MedicineId = firstMed.Id
                            }
                        }
                    };
                    context.Prescriptions.Add(prescription);
                    await context.SaveChangesAsync();
                }
            }

            // Bills
            if (!await context.Bills.AnyAsync())
            {
                var anyPatient = await context.Patients.Select(p => p.Id).OrderBy(id => id).FirstOrDefaultAsync();
                if (anyPatient != 0)
                {
                    var bill = new Bill
                    {
                        BillNumber = $"BILL-{DateTime.UtcNow:yyyyMMdd}-0001",
                        BillDate = DateTime.UtcNow.Date,
                        DueDate = DateTime.UtcNow.Date.AddDays(7),
                        Notes = "Consultation and medicine",
                        InsuranceProvider = "HealthCo",
                        InsuranceNumber = "INS-001",
                        InsuranceCoverage = 80,
                        PatientId = anyPatient,
                        CreatedAt = DateTime.UtcNow,
                        BillItems = new List<BillItem>
                        {
                            new BillItem
                            {
                                Description = "Consultation Fee",
                                Category = "Consultation",
                                Quantity = 1,
                                UnitPrice = 300m,
                                TotalPrice = 300m,
                                Notes = "Neurology consultation"
                            },
                            new BillItem
                            {
                                Description = "Paracetamol 500mg",
                                Category = "Medicine",
                                Quantity = 14,
                                UnitPrice = 5m,
                                TotalPrice = 14 * 5m,
                                Notes = "Pain relief"
                            }
                        }
                    };
                    // Compute totals
                    bill.SubTotal = bill.BillItems.Sum(i => i.TotalPrice);
                    bill.TaxAmount = Math.Round(bill.SubTotal * 0.14m, 2); // 14% VAT example
                    bill.DiscountAmount = 0m;
                    bill.TotalAmount = bill.SubTotal + bill.TaxAmount - bill.DiscountAmount;
                    bill.PaidAmount = 0m;
                    bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;

                    context.Bills.Add(bill);
                    await context.SaveChangesAsync();
                }
            }

            // Add a few more patients and appointments for a richer dataset
            if (await context.Patients.CountAsync() < 5)
            {
                var extraPatients = new List<Patient>
                {
                    new Patient
                    {
                        FirstName = "Hassan",
                        LastName = "Mahmoud",
                        NationalId = "2765432109",
                        Email = "hassan.mahmoud@hospital.com",
                        PhoneNumber = "01134567890",
                        DateOfBirth = new DateTime(1978, 9, 1),
                        Gender = "Male",
                        Address = "Corniche El Nil Street, Alexandria, Egypt",
                        EmergencyContactName = "Brother - Hassan",
                        EmergencyContactPhone = "01234567891",
                        InsuranceProvider = "Public Health Insurance",
                        InsuranceNumber = "INS-0003",
                        MedicalHistory = "Hypertension",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "Nora",
                        LastName = "Adel",
                        NationalId = "2654321098",
                        Email = "nora.adel@hospital.com",
                        PhoneNumber = "01145678901",
                        DateOfBirth = new DateTime(1995, 1, 22),
                        Gender = "Female",
                        Address = "Maadi Street, Cairo, Egypt",
                        EmergencyContactName = "Sister - Nora",
                        EmergencyContactPhone = "01245678902",
                        InsuranceProvider = "National Health Insurance",
                        InsuranceNumber = "INS-0004",
                        Allergies = "Ibuprofen",
                        MedicalHistory = "No chronic conditions",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "Yasmine",
                        LastName = "Fouad",
                        NationalId = "2543210987",
                        Email = "yasmine.fouad@hospital.com",
                        PhoneNumber = "01156789012",
                        DateOfBirth = new DateTime(2001, 7, 30),
                        Gender = "Female",
                        Address = "Pyramid Street, Giza, Egypt",
                        EmergencyContactName = "Mother - Yasmine",
                        EmergencyContactPhone = "01256789013",
                        InsuranceProvider = null,
                        InsuranceNumber = null,
                        MedicalHistory = "No chronic conditions",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };
                context.Patients.AddRange(extraPatients);
                await context.SaveChangesAsync();
            }

            if (await context.Appointments.CountAsync() < 5)
            {
                var appts2 = new List<Appointment>
                {
                    new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date.AddDays(2),
                        AppointmentTime = new TimeSpan(9,30,0),
                        Status = "Scheduled",
                        Reason = "Follow up",
                        ConsultationFee = 250,
                        PatientId = 3,
                        DoctorId = 1,
                        RoomId = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date.AddDays(3),
                        AppointmentTime = new TimeSpan(15,0,0),
                        Status = "Scheduled",
                        Reason = "Knee pain",
                        ConsultationFee = 220,
                        PatientId = 4,
                        DoctorId = 3,
                        RoomId = 3,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date,
                        AppointmentTime = new TimeSpan(12,0,0),
                        Status = "Confirmed",
                        Reason = "Fever",
                        ConsultationFee = 90,
                        PatientId = 5,
                        DoctorId = 4, // fallback if not present, adjust to existing doctor
                        RoomId = 4,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                // Ensure referenced doctor/patient IDs exist
                var maxDoctorId = await context.Doctors.MaxAsync(d => (int?)d.Id) ?? 0;
                var maxPatientId = await context.Patients.MaxAsync(p => (int?)p.Id) ?? 0;
                foreach (var a in appts2)
                {
                    if (a.DoctorId > maxDoctorId) a.DoctorId = 1;
                    if (a.PatientId > maxPatientId) a.PatientId = 1;
                }

                context.Appointments.AddRange(appts2);
                await context.SaveChangesAsync();
            }

            // Bulk generate dataset: more doctors, patients, appointments, prescriptions, bills
            // Target sizes
            int targetDoctorsPerDepartment = 8;
            int targetPatients = 300;
            int targetAppointments = 500;

            // Ensure more doctors per department (with English names)
            var maleFirstNames = new[] { "Ahmed", "Omar", "Khaled", "Hassan", "Mostafa", "Youssef", "Hossam", "Ibrahim", "Tarek", "Mahmoud", "Farouk", "Sami" };
            var femaleFirstNames = new[] { "Sarah", "Mona", "Rania", "Fatima", "Nour", "Yasmine", "Hoda", "Laila", "Amira", "Nadia", "Mariam", "Reham" };
            var lastNames = new[] { "Hassan", "Ibrahim", "Mahmoud", "Elsayed", "Mostafa", "Abdelrahman", "Khaled", "Ali", "Farouk", "Attia", "Shokry", "Kamel" };

            var allDepartments = await context.Departments.AsNoTracking().ToListAsync();
            foreach (var dept in allDepartments)
            {
                int existingDoctorsInDept = await context.Doctors.CountAsync(d => d.DepartmentId == dept.Id);
                var newDoctors = new List<Doctor>();
                for (int i = existingDoctorsInDept + 1; i <= targetDoctorsPerDepartment; i++)
                {
                    bool isMale = i % 2 == 0;
                    var first = isMale ? maleFirstNames[i % maleFirstNames.Length] : femaleFirstNames[i % femaleFirstNames.Length];
                    var last = lastNames[(i + dept.Id) % lastNames.Length];
                    var educationLevels = new[] { "PhD", "Master's", "Bachelor's" };
                    var certifications = new[] { "Board Certified by the Medical Council", "Medical Fellowship", "Specialty Certification", "Medical Board" };

                    newDoctors.Add(new Doctor
                    {
                        FirstName = first,
                        LastName = last,
                        NationalId = $"2{(dept.Id * 1000000 + i * 10000) % 1000000000:000000000}",
                        Email = SampleEmailHelper.DoctorEmailForSeed(dept.Id, i),
                        PhoneNumber = $"010{dept.Id}{(i % 1000000):0000000}",
                        DateOfBirth = new DateTime(1965, 1, 1).AddDays((dept.Id * 50) + i * 30),
                        Gender = isMale ? "Male" : "Female",
                        Address = dept.Location,
                        LicenseNumber = $"LIC-{dept.Name.ToUpper().Replace(" ", string.Empty)}-{i:000}",
                        Specialization = dept.Name,
                        SubSpecialization = i % 3 == 0 ? "Subspecialty" : null,
                        YearsOfExperience = 5 + (i % 25),
                        Education = educationLevels[i % educationLevels.Length],
                        Certifications = certifications[i % certifications.Length],
                        Languages = i % 2 == 0 ? "Arabic, English" : "Arabic, English, French",
                        ConsultationFee = 150 + (i * 15),
                        WorkingHoursStart = new TimeSpan(9 + (i % 2), 0, 0),
                        WorkingHoursEnd = new TimeSpan(17 + (i % 2), 0, 0),
                        DepartmentId = dept.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-(i * 10)),
                        IsActive = true
                    });
                }
                if (newDoctors.Count > 0)
                {
                    context.Doctors.AddRange(newDoctors);
                    await context.SaveChangesAsync();
                }
            }

            // Ensure more patients - Generate realistic English names
            int currentPatients = await context.Patients.CountAsync();
            if (currentPatients < targetPatients)
            {
                var patientMaleFirstNames = new[] { "Mohamed", "Ahmed", "Ali", "Khaled", "Mahmoud", "Youssef", "Hossam", "Ibrahim", "Omar", "Tarek", "Mostafa", "Abdullah", "Hassan", "Sami", "Farouk" };
                var patientFemaleFirstNames = new[] { "Sarah", "Fatima", "Mariam", "Nora", "Reem", "Yasmine", "Hoda", "Laila", "Salma", "Nadia", "Amira", "Rania", "Mona", "Hend", "Asmaa" };
                var patientLastNames = new[] { "Mohamed", "Ali", "Hassan", "Ibrahim", "Mahmoud", "Elsayed", "Mostafa", "Abdelrahman", "Khaled", "Youssef", "Farouk", "Attia", "Shokry", "Kamel", "Salah" };
                var cities = new[] { "Cairo", "Giza", "Alexandria", "Mansoura", "Tanta", "Assiut", "Sohag", "Luxor", "Aswan", "Port Said", "Ismailia", "Suez" };
                var streets = new[] { "Nile Street", "Tahrir Street", "Corniche El Nil Street", "Pyramid Street", "Maadi Street", "Heliopolis Street", "Zamalek Street", "Mohandessin Street" };

                var addPatients = new List<Patient>();
                for (int i = currentPatients + 1; i <= targetPatients; i++)
                {
                    bool isMale = i % 2 == 0;
                    var firstName = isMale
                        ? patientMaleFirstNames[i % patientMaleFirstNames.Length]
                        : patientFemaleFirstNames[i % patientFemaleFirstNames.Length];
                    var lastName = patientLastNames[(i * 3) % patientLastNames.Length];
                    var city = cities[i % cities.Length];
                    var street = streets[i % streets.Length];
                    var address = $"{street}, {city}, Egypt";

                    var insuranceProviders = new[] { "National Health Insurance", "Comprehensive Health Insurance", "Public Health Insurance", "Private Sector Insurance", null };
                    var medicalHistories = new[] { "Diabetes", "Hypertension", "Heart Disease", "Kidney Disease", "Lung Disease", "No chronic conditions", null };
                    var allergies = new[] { "Penicillin", "Aspirin", "Ibuprofen", "No known allergies", null };

                    addPatients.Add(new Patient
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        NationalId = $"2{(i % 1000000000):000000000}",
                        Email = SampleEmailHelper.PatientEmailForSeed(i),
                        PhoneNumber = $"01{(i % 10)}{(i % 10000000):00000000}",
                        DateOfBirth = new DateTime(1950, 1, 1).AddDays(i * 100 % 20000),
                        Gender = isMale ? "Male" : "Female",
                        Address = address,
                        EmergencyContactName = isMale ? $"Brother - {firstName}" : $"Sister - {firstName}",
                        EmergencyContactPhone = $"01{(i % 10)}{(i % 10000000 + 10000000):00000000}",
                        InsuranceProvider = insuranceProviders[i % insuranceProviders.Length],
                        InsuranceNumber = insuranceProviders[i % insuranceProviders.Length] != null ? $"INS-{i:00000}" : null,
                        MedicalHistory = medicalHistories[i % medicalHistories.Length],
                        Allergies = allergies[i % allergies.Length],
                        CreatedAt = DateTime.UtcNow.AddDays(-(i % 365)),
                        IsActive = true
                    });
                }
                context.Patients.AddRange(addPatients);
                await context.SaveChangesAsync();
            }

            // Appointments bulk
            int currentAppointments = await context.Appointments.CountAsync();
            if (currentAppointments < targetAppointments)
            {
                var doctorIds = await context.Doctors.Select(d => d.Id).ToListAsync();
                var patientIds = await context.Patients.Select(p => p.Id).ToListAsync();
                var roomsByDept = await context.Rooms
                    .Select(r => new { r.Id, r.DepartmentId })
                    .ToListAsync();

                var doctorsDept = await context.Doctors
                    .Select(d => new { d.Id, d.DepartmentId })
                    .ToListAsync();

                var deptToRoom = roomsByDept
                    .GroupBy(r => r.DepartmentId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Id).FirstOrDefault());

                var rngBase = DateTime.UtcNow.Date;
                var toAdd = new List<Appointment>();
                int toCreate = targetAppointments - currentAppointments;
                for (int i = 0; i < toCreate; i++)
                {
                    int doctorId = doctorIds[i % doctorIds.Count];
                    int patientId = patientIds[i % patientIds.Count];
                    var dDept = doctorsDept.First(x => x.Id == doctorId).DepartmentId;
                    int? roomId = deptToRoom.TryGetValue(dDept, out var rid) ? rid : (int?)null;

                    var dayOffset = (i % 60) - 30; // spread across past/future 30 days
                    var hour = 9 + (i % 8); // 9..16

                    string status = (i % 10 == 0) ? "Cancelled" : (i % 7 == 0) ? "Completed" : (i % 5 == 0) ? "Confirmed" : "Scheduled";
                    toAdd.Add(new Appointment
                    {
                        AppointmentDate = rngBase.AddDays(dayOffset),
                        AppointmentTime = new TimeSpan(hour, (i % 2) * 30, 0),
                        Status = status,
                        Reason = (i % 3 == 0) ? "Checkup" : (i % 3 == 1) ? "Pain" : "Follow up",
                        ConsultationFee = 150 + (i % 6) * 25,
                        PatientId = patientId,
                        DoctorId = doctorId,
                        RoomId = roomId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                context.Appointments.AddRange(toAdd);
                await context.SaveChangesAsync();
            }

            // Prescriptions bulk (for a subset of appointments)
            int existingPrescriptions = await context.Prescriptions.CountAsync();
            if (existingPrescriptions < 200)
            {
                var apptInfos = await context.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new { a.Id, a.PatientId, a.DoctorId, a.Status })
                    .Take(400)
                    .ToListAsync();

                var meds = await context.Medicines.Select(m => new { m.Id, m.Name, m.Price }).ToListAsync();
                if (meds.Count == 0)
                {
                    // No medicines available, skip prescription seeding
                }
                else
                {
                    var prxList = new List<Prescription>();
                    int limit = Math.Min(200 - existingPrescriptions, apptInfos.Count / 2);
                    for (int i = 0; i < limit; i++)
                    {
                        var ap = apptInfos[i * 2];
                        var med = meds[i % meds.Count];
                        prxList.Add(new Prescription
                        {
                            PrescriptionDate = DateTime.UtcNow.Date.AddDays(-(i % 20)),
                            ValidUntil = DateTime.UtcNow.Date.AddDays(7 - (i % 5)),
                            Instructions = (i % 2 == 0) ? "Take after meals" : "Take before sleep",
                            Notes = null,
                            PatientId = ap.PatientId,
                            DoctorId = ap.DoctorId,
                            CreatedAt = DateTime.UtcNow,
                            PrescriptionItems = new List<PrescriptionItem>
                            {
                                new PrescriptionItem
                                {
                                    MedicineName = meds[i % meds.Count].Name,
                                    Dosage = (i % 3 == 0) ? "500mg" : "250mg",
                                    Frequency = (i % 2 == 0) ? "Twice daily" : "Once daily",
                                    Duration = (i % 2 == 0) ? "7 days" : "5 days",
                                    Instructions = "With water",
                                    Quantity = 10 + (i % 10),
                                    UnitPrice = med.Price,
                                    MedicineId = med.Id
                                }
                            }
                        });
                    }
                    context.Prescriptions.AddRange(prxList);
                    await context.SaveChangesAsync();
                }
            }

            // Bills bulk (for a subset of appointments)
            int existingBills = await context.Bills.CountAsync();
            if (existingBills < 250)
            {
                var completedOrConfirmedAppts = await context.Appointments
                    .Where(a => a.Status == "Completed" || a.Status == "Confirmed")
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new { a.Id, a.PatientId, a.ConsultationFee })
                    .Take(400)
                    .ToListAsync();

                var billList = new List<Bill>();
                int toMake = Math.Min(250 - existingBills, completedOrConfirmedAppts.Count);
                for (int i = 0; i < toMake; i++)
                {
                    var ap = completedOrConfirmedAppts[i];
                    decimal consult = ap.ConsultationFee ?? 200m;
                    var items = new List<BillItem>
                    {
                        new BillItem { Description = "Consultation Fee", Category = "Consultation", Quantity = 1, UnitPrice = consult, TotalPrice = consult },
                        new BillItem { Description = "Service Charge", Category = "Service", Quantity = 1, UnitPrice = 25m, TotalPrice = 25m }
                    };
                    var bill = new Bill
                    {
                        BillNumber = $"BILL-{DateTime.UtcNow:yyyyMMdd}-{existingBills + i + 2:0000}",
                        BillDate = DateTime.UtcNow.Date.AddDays(-(i % 15)),
                        DueDate = DateTime.UtcNow.Date.AddDays(7 - (i % 7)),
                        Status = (i % 6 == 0) ? "Paid" : (i % 5 == 0) ? "Overdue" : "Pending",
                        Notes = null,
                        InsuranceProvider = (i % 5 == 0) ? "HealthCo" : null,
                        InsuranceNumber = (i % 5 == 0) ? $"INS-{1000 + i}" : null,
                        InsuranceCoverage = (i % 5 == 0) ? 80 : null,
                        PatientId = ap.PatientId,
                        CreatedAt = DateTime.UtcNow,
                        BillItems = items
                    };
                    bill.SubTotal = items.Sum(x => x.TotalPrice);
                    bill.TaxAmount = Math.Round(bill.SubTotal * 0.14m, 2);
                    bill.DiscountAmount = 0m;
                    bill.TotalAmount = bill.SubTotal + bill.TaxAmount;
                    bill.PaidAmount = (bill.Status == "Paid") ? bill.TotalAmount : 0m;
                    bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;
                    billList.Add(bill);
                }
                context.Bills.AddRange(billList);
                await context.SaveChangesAsync();
            }

            // Schedules - assign a weekly schedule for every doctor that doesn't have one yet
            var docsWithoutSchedule = await context.Doctors
                .Where(d => !context.Schedules.Any(s => s.DoctorId == d.Id))
                .Select(d => d.Id)
                .ToListAsync();
            if (docsWithoutSchedule.Count > 0)
            {
                var weeklyDays = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday };
                var schedulesToAdd = new List<Schedule>();
                foreach (var docId in docsWithoutSchedule)
                {
                    // Each doctor gets 3 weekday shifts
                    for (int s = 0; s < 3; s++)
                    {
                        var day = weeklyDays[(docId + s) % weeklyDays.Length];
                        var startHour = 8 + ((docId + s) % 4); // 8..11 start
                        schedulesToAdd.Add(new Schedule
                        {
                            DayOfWeek = day,
                            StartTime = new TimeSpan(startHour, 0, 0),
                            EndTime = new TimeSpan(startHour + 4, 0, 0),
                            DoctorId = docId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                context.Schedules.AddRange(schedulesToAdd);
                await context.SaveChangesAsync();
            }

            // Bulk Medical Records - seed up to 200 consultation records from past appointments
            int existingMedicalRecords = await context.MedicalRecords.CountAsync();
            if (existingMedicalRecords < 200)
            {
                var pastAppointments = await context.Appointments
                    .Where(a => a.Status == "Completed")
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new { a.PatientId, a.DoctorId, a.AppointmentDate })
                    .Take(400)
                    .ToListAsync();

                if (pastAppointments.Count > 0)
                {
                    var symptomBank = new[]
                    {
                        "Headache", "Fever", "Chest pain", "Cough", "Shortness of breath",
                        "Abdominal pain", "Joint pain", "Fatigue", "Dizziness", "Skin rash"
                    };
                    var diagnosisBank = new[]
                    {
                        "Migraine", "Viral infection", "Hypertension stage 1", "Bronchitis",
                        "Asthma exacerbation", "Gastritis", "Osteoarthritis", "Vitamin D deficiency",
                        "Vertigo", "Allergic dermatitis"
                    };
                    var treatmentBank = new[]
                    {
                        "Rest and analgesics", "Hydration and antipyretics", "Lifestyle changes and follow-up",
                        "Bronchodilator therapy", "Inhaler regimen", "Diet adjustment and PPI",
                        "Physical therapy", "Oral supplementation", "Vestibular rehabilitation", "Topical steroid"
                    };
                    var bpBank = new[] { "120/80", "118/76", "135/85", "128/82", "142/90", "110/70" };
                    var tempBank = new[] { "36.6°C", "36.8°C", "37.1°C", "37.4°C", "38.0°C" };
                    var hrBank = new[] { "68", "72", "76", "82", "88", "94" };

                    var mrList = new List<MedicalRecord>();
                    int toCreate = Math.Min(200 - existingMedicalRecords, pastAppointments.Count);
                    for (int i = 0; i < toCreate; i++)
                    {
                        var ap = pastAppointments[i];
                        mrList.Add(new MedicalRecord
                        {
                            RecordDate = ap.AppointmentDate,
                            RecordType = "Consultation",
                            Symptoms = symptomBank[i % symptomBank.Length],
                            Diagnosis = diagnosisBank[i % diagnosisBank.Length],
                            Treatment = treatmentBank[i % treatmentBank.Length],
                            Notes = (i % 3 == 0) ? "Follow up in 2 weeks" : "Follow up in 1 month",
                            VitalSigns = $"BP {bpBank[i % bpBank.Length]}, HR {hrBank[i % hrBank.Length]}",
                            BloodPressure = bpBank[i % bpBank.Length],
                            Temperature = tempBank[i % tempBank.Length],
                            HeartRate = hrBank[i % hrBank.Length],
                            Weight = $"{60 + (i % 40)}kg",
                            Height = $"{155 + (i % 40)}cm",
                            PatientId = ap.PatientId,
                            DoctorId = ap.DoctorId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    context.MedicalRecords.AddRange(mrList);
                    await context.SaveChangesAsync();
                }
            }

            // Seed Features
            if (!await context.Features.AnyAsync())
            {
                var features = new[]
                {
                    new Feature
                    {
                        Icon = "⚡",
                        TitleEn = "Quick Care",
                        TitleAr = "رعاية سريعة",
                        DescriptionEn = "Fast and efficient medical services",
                        DescriptionAr = "خدمات طبية سريعة وفعالة",
                        Color = "#FFD700",
                        DisplayOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Feature
                    {
                        Icon = "🎯",
                        TitleEn = "High Precision",
                        TitleAr = "دقة عالية",
                        DescriptionEn = "Accurate diagnosis and specialized treatment",
                        DescriptionAr = "تشخيص دقيق وعلاج متخصص",
                        Color = "#00CED1",
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Feature
                    {
                        Icon = "💚",
                        TitleEn = "Comprehensive Care",
                        TitleAr = "رعاية شاملة",
                        DescriptionEn = "We provide you with the best healthcare",
                        DescriptionAr = "نوفر لك أفضل رعاية صحية",
                        Color = "#32CD32",
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Features.AddRange(features);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Fixes doctor/patient emails that contain Arabic or invalid characters so login works with ASCII addresses.
        /// Also updates linked AspNetUsers Email and UserName when present.
        /// </summary>
        public static async Task<FixLoginEmailsResult> FixLoginEmailsAsync(
            HospitalDbContext context,
            UserManager<User> userManager)
        {
            var result = new FixLoginEmailsResult();

            var doctors = await context.Doctors.Where(d => !d.IsDeleted).ToListAsync();
            foreach (var doctor in doctors)
            {
                if (SampleEmailHelper.IsValidLoginEmail(doctor.Email))
                    continue;

                var newEmail = SampleEmailHelper.DoctorEmail(doctor.Id);
                if (await context.Doctors.AnyAsync(d => d.Id != doctor.Id && d.Email == newEmail && !d.IsDeleted))
                    newEmail = $"dr.{doctor.Id}.{doctor.DepartmentId}@{SampleEmailHelper.Domain}";

                result.DoctorsUpdated++;
                await UpdateEntityAndUserEmailAsync(context, userManager, doctor.UserId, doctor.Email, newEmail,
                    (e) => doctor.Email = e);
            }

            var patients = await context.Patients.Where(p => !p.IsDeleted).ToListAsync();
            foreach (var patient in patients)
            {
                if (SampleEmailHelper.IsValidLoginEmail(patient.Email))
                    continue;

                var newEmail = SampleEmailHelper.PatientEmail(patient.Id);
                if (await context.Patients.AnyAsync(p => p.Id != patient.Id && p.Email == newEmail && !p.IsDeleted))
                    newEmail = $"patient.{patient.Id}.alt@{SampleEmailHelper.Domain}";

                result.PatientsUpdated++;
                await UpdateEntityAndUserEmailAsync(context, userManager, patient.UserId, patient.Email, newEmail,
                    (e) => patient.Email = e);
            }

            await context.SaveChangesAsync();
            return result;
        }

        private static async Task UpdateEntityAndUserEmailAsync(
            HospitalDbContext context,
            UserManager<User> userManager,
            string? userId,
            string oldEmail,
            string newEmail,
            Action<string> setEntityEmail)
        {
            setEntityEmail(newEmail);

            if (string.IsNullOrEmpty(userId))
                return;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return;

            if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                return;

            var existing = await userManager.FindByEmailAsync(newEmail);
            if (existing != null && existing.Id != user.Id)
                return;

            await userManager.SetEmailAsync(user, newEmail);
            await userManager.SetUserNameAsync(user, newEmail);
        }
    }

    public class FixLoginEmailsResult
    {
        public int DoctorsUpdated { get; set; }
        public int PatientsUpdated { get; set; }
    }
}
