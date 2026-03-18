using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public static async Task SeedAdminUserAsync(UserManager<User> userManager)
        {
            var adminEmail = "admin@hospital.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
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

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
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
                    Description = "متخصص في علاج أمراض القلب والأوعية الدموية، تشخيص وعلاج حالات القلب، فحوصات القلب، قسطرة القلب، وجراحات القلب المفتوح",
                    HeadOfDepartment = "د. أحمد حسن",
                    PhoneNumber = "02-1234-5670",
                    Location = "الطابق الثاني - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Neurology",
                    Description = "متخصص في علاج أمراض الجهاز العصبي والدماغ، تشخيص وعلاج الصرع، السكتة الدماغية، أمراض الأعصاب، والصداع المزمن",
                    HeadOfDepartment = "د. سارة محمد",
                    PhoneNumber = "02-1234-5671",
                    Location = "الطابق الثالث - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Orthopedics",
                    Description = "متخصص في علاج أمراض العظام والمفاصل، جراحات العظام، استبدال المفاصل، كسور العظام، وإصابات الرياضة",
                    HeadOfDepartment = "د. عمر علي",
                    PhoneNumber = "02-1234-5672",
                    Location = "الطابق الأول - المبنى الجراحي (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pediatrics",
                    Description = "رعاية طبية متخصصة للأطفال والرضع، تشخيص وعلاج أمراض الأطفال، التطعيمات، ومتابعة نمو الأطفال",
                    HeadOfDepartment = "د. فاطمة أحمد",
                    PhoneNumber = "02-1234-5673",
                    Location = "الطابق الثاني - المبنى الجراحي (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Emergency",
                    Description = "قسم الطوارئ والرعاية الطبية العاجلة، استقبال حالات الطوارئ على مدار 24 ساعة، علاج الحوادث، والإصابات الخطيرة",
                    HeadOfDepartment = "د. خالد إبراهيم",
                    PhoneNumber = "02-1234-5674",
                    Location = "الطابق الأرضي - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Oncology",
                    Description = "متخصص في تشخيص وعلاج الأورام والسرطان، العلاج الكيميائي، العلاج الإشعاعي، وجراحات الأورام",
                    HeadOfDepartment = "د. منى السيد",
                    PhoneNumber = "02-1234-5675",
                    Location = "الطابق الرابع - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Radiology",
                    Description = "خدمات التشخيص بالإشعة، الأشعة السينية، التصوير بالرنين المغناطيسي (MRI)، التصوير المقطعي (CT)، والموجات فوق الصوتية",
                    HeadOfDepartment = "د. هشام فاروق",
                    PhoneNumber = "02-1234-5676",
                    Location = "البدروم - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Dermatology",
                    Description = "متخصص في علاج أمراض الجلد والشعر والأظافر، علاج البشرة، الأمراض الجلدية المزمنة، وإجراءات التجميل الطبي",
                    HeadOfDepartment = "د. رانيا مصطفى",
                    PhoneNumber = "02-1234-5677",
                    Location = "الطابق الأول - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Gastroenterology",
                    Description = "متخصص في علاج أمراض الجهاز الهضمي، تنظير المعدة والأمعاء، علاج أمراض الكبد، والمرارة، والبنكرياس",
                    HeadOfDepartment = "د. محمد فريد",
                    PhoneNumber = "02-1234-5678",
                    Location = "الطابق الثاني - المبنى الجراحي (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pulmonology",
                    Description = "متخصص في علاج أمراض الجهاز التنفسي والرئة، الربو، التهاب الشعب الهوائية، أمراض الرئة المزمنة، وفحوصات وظائف الرئة",
                    HeadOfDepartment = "د. أميرة صلاح",
                    PhoneNumber = "02-1234-5679",
                    Location = "الطابق الثالث - المبنى الجراحي (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Endocrinology",
                    Description = "متخصص في علاج أمراض الغدد الصماء والهرمونات، السكري، أمراض الغدة الدرقية، السمنة، واضطرابات النمو",
                    HeadOfDepartment = "د. طارق نبيل",
                    PhoneNumber = "02-1234-5680",
                    Location = "الطابق الرابع - المبنى الجراحي (B)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Nephrology",
                    Description = "متخصص في علاج أمراض الكلى والمسالك البولية، الفشل الكلوي، زراعة الكلى، غسيل الكلى، وأمراض الكلى المزمنة",
                    HeadOfDepartment = "د. ليلى كامل",
                    PhoneNumber = "02-1234-5681",
                    Location = "الطابق الخامس - المبنى الرئيسي (A)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Ophthalmology",
                    Description = "متخصص في علاج أمراض العيون والرؤية، جراحات العيون، تصحيح النظر، علاج المياه البيضاء والزرقاء، وفحوصات العيون الشاملة",
                    HeadOfDepartment = "د. كريم شكري",
                    PhoneNumber = "02-1234-5682",
                    Location = "الطابق الأول - المبنى الطبي (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "ENT",
                    Description = "متخصص في علاج أمراض الأنف والأذن والحنجرة، جراحات الأذن، مشاكل السمع، التهاب الجيوب الأنفية، وجراحات الحنجرة",
                    HeadOfDepartment = "د. نهى سمير",
                    PhoneNumber = "02-1234-5683",
                    Location = "الطابق الثاني - المبنى الطبي (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Urology",
                    Description = "متخصص في علاج أمراض الجهاز البولي والجهاز التناسلي الذكري، حصوات الكلى، تضخم البروستاتا، وجراحات المسالك البولية",
                    HeadOfDepartment = "د. وائل عطية",
                    PhoneNumber = "02-1234-5684",
                    Location = "الطابق الثالث - المبنى الطبي (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Gynecology",
                    Description = "رعاية صحية متخصصة للنساء، أمراض النساء، الفحوصات الدورية، الولادة، والعقم",
                    HeadOfDepartment = "د. ريهام الزهراء",
                    PhoneNumber = "02-1234-5685",
                    Location = "الطابق الرابع - المبنى الطبي (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Psychiatry",
                    Description = "متخصص في علاج الأمراض النفسية والصحة العقلية، الاكتئاب، القلق، الاضطرابات النفسية، والاستشارات النفسية",
                    HeadOfDepartment = "د. سمير أبو الفتوح",
                    PhoneNumber = "02-1234-5686",
                    Location = "الطابق الخامس - المبنى الطبي (C)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Anesthesiology",
                    Description = "خدمات التخدير وإدارة الألم، تخدير الجراحات، وحدة العناية المركزة، وإدارة الألم المزمن",
                    HeadOfDepartment = "د. هاني عبد المنعم",
                    PhoneNumber = "02-1234-5687",
                    Location = "الطابق الأول - المبنى الجراحي (D)",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pathology",
                    Description = "خدمات المختبرات الطبية والتشخيص المخبري، تحاليل الدم، تحاليل الأنسجة، علم الأمراض، والفحوصات المخبرية الشاملة",
                    HeadOfDepartment = "د. مجدة الشربيني",
                    PhoneNumber = "02-1234-5688",
                    Location = "البدروم - المبنى الجراحي (B)",
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
                        FirstName = "أحمد",
                        LastName = "حسن",
                        NationalId = "2111111111",
                        Email = "ahmed.hassan@hospital.com",
                        PhoneNumber = "01001234567",
                        DateOfBirth = new DateTime(1980, 3, 20),
                        Gender = "Male",
                        Address = "الطابق الثاني - المبنى الرئيسي (A)",
                        LicenseNumber = "LIC-CARD-001",
                        Specialization = "Cardiology",
                        SubSpecialization = "قسطرة القلب التداخلية",
                        YearsOfExperience = 15,
                        Education = "دكتوراه",
                        Certifications = "معتمد من المجلس الطبي",
                        Languages = "العربية، الإنجليزية",
                        ConsultationFee = 250,
                        WorkingHoursStart = new TimeSpan(9, 0, 0),
                        WorkingHoursEnd = new TimeSpan(17, 0, 0),
                        DepartmentId = 1,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Doctor
                    {
                        FirstName = "سارة",
                        LastName = "محمد",
                        NationalId = "2222222222",
                        Email = "sarah.mohamed@hospital.com",
                        PhoneNumber = "01002345678",
                        DateOfBirth = new DateTime(1982, 6, 12),
                        Gender = "Female",
                        Address = "الطابق الثالث - المبنى الرئيسي (A)",
                        LicenseNumber = "LIC-NEURO-001",
                        Specialization = "Neurology",
                        SubSpecialization = "طب الصرع",
                        YearsOfExperience = 13,
                        Education = "دكتوراه",
                        Certifications = "زمالة طبية",
                        Languages = "العربية، الإنجليزية",
                        ConsultationFee = 300,
                        WorkingHoursStart = new TimeSpan(10, 0, 0),
                        WorkingHoursEnd = new TimeSpan(18, 0, 0),
                        DepartmentId = 2,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Doctor
                    {
                        FirstName = "عمر",
                        LastName = "علي",
                        NationalId = "2333333333",
                        Email = "omar.ali@hospital.com",
                        PhoneNumber = "01003456789",
                        DateOfBirth = new DateTime(1985, 11, 5),
                        Gender = "Male",
                        Address = "الطابق الأول - المبنى الجراحي (B)",
                        LicenseNumber = "LIC-ORTHO-001",
                        Specialization = "Orthopedics",
                        SubSpecialization = "جراحة العظام والمفاصل",
                        YearsOfExperience = 10,
                        Education = "ماجستير",
                        Certifications = "شهادة تخصص",
                        Languages = "العربية، الإنجليزية",
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

            // Patients
            if (!await context.Patients.AnyAsync())
            {
                var patients = new[]
                {
                    new Patient
                    {
                        FirstName = "محمد",
                        LastName = "يوسف",
                        NationalId = "2987654321",
                        Email = "mohamed.youssef@example.com",
                        PhoneNumber = "01123456789",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        Gender = "Male",
                        Address = "شارع النيل، القاهرة، مصر",
                        EmergencyContactName = "أحمد يوسف",
                        EmergencyContactPhone = "01234567890",
                        InsuranceProvider = "شركة التأمين الصحي",
                        InsuranceNumber = "INS-0001",
                        MedicalHistory = "لا توجد أمراض مزمنة",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "سلمى",
                        LastName = "خالد",
                        NationalId = "2876543210",
                        Email = "salma.khaled@example.com",
                        PhoneNumber = "01198765432",
                        DateOfBirth = new DateTime(1992, 3, 10),
                        Gender = "Female",
                        Address = "شارع التحرير، الجيزة، مصر",
                        EmergencyContactName = "خالد خالد",
                        EmergencyContactPhone = "01298765432",
                        InsuranceProvider = "التأمين الصحي الشامل",
                        InsuranceNumber = "INS-0002",
                        Allergies = "بنسلين",
                        MedicalHistory = "لا توجد أمراض مزمنة",
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
                        FirstName = "حسن",
                        LastName = "محمود",
                        NationalId = "2765432109",
                        Email = "hassan.mahmoud@example.com",
                        PhoneNumber = "01134567890",
                        DateOfBirth = new DateTime(1978, 9, 1),
                        Gender = "Male",
                        Address = "شارع كورنيش النيل، الإسكندرية، مصر",
                        EmergencyContactName = "أخ - حسن",
                        EmergencyContactPhone = "01234567891",
                        InsuranceProvider = "تأمين الصحة العامة",
                        InsuranceNumber = "INS-0003",
                        MedicalHistory = "ضغط الدم",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "نورا",
                        LastName = "عادل",
                        NationalId = "2654321098",
                        Email = "nora.adel@example.com",
                        PhoneNumber = "01145678901",
                        DateOfBirth = new DateTime(1995, 1, 22),
                        Gender = "Female",
                        Address = "شارع المعادي، القاهرة، مصر",
                        EmergencyContactName = "أخت - نورا",
                        EmergencyContactPhone = "01245678902",
                        InsuranceProvider = "شركة التأمين الصحي",
                        InsuranceNumber = "INS-0004",
                        Allergies = "أيبوبروفين",
                        MedicalHistory = "لا توجد أمراض مزمنة",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "يسرى",
                        LastName = "فؤاد",
                        NationalId = "2543210987",
                        Email = "yasmine.fouad@example.com",
                        PhoneNumber = "01156789012",
                        DateOfBirth = new DateTime(2001, 7, 30),
                        Gender = "Female",
                        Address = "شارع الهرم، الجيزة، مصر",
                        EmergencyContactName = "أم - يسرى",
                        EmergencyContactPhone = "01256789013",
                        InsuranceProvider = null,
                        InsuranceNumber = null,
                        MedicalHistory = "لا توجد أمراض مزمنة",
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
            int targetDoctorsPerDepartment = 5;
            int targetPatients = 120;
            int targetAppointments = 150;

            // Ensure more doctors per department (with realistic Arabic names)
            var maleFirstNames = new[] { "أحمد", "عمر", "خالد", "حسن", "مصطفى", "يوسف", "حسام", "إبراهيم", "طارق", "محمود", "فاروق", "سامي" };
            var femaleFirstNames = new[] { "سارة", "منى", "رانيا", "فاطمة", "نور", "يسرى", "هدى", "ليلى", "أميرة", "نادية", "مريم", "ريهام" };
            var lastNames = new[] { "حسن", "إبراهيم", "محمود", "السيد", "مصطفى", "عبدالرحمن", "خالد", "علي", "فاروق", "عطية", "شكري", "كامل" };

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
                    var email = $"{first.ToLower()}.{last.ToLower()}.{dept.Id}{i}@hospital.com";

                    var educationLevels = new[] { "دكتوراه", "ماجستير", "بكالوريوس" };
                    var certifications = new[] { "معتمد من المجلس الطبي", "زمالة طبية", "شهادة تخصص", "بورد طبي" };
                    
                    newDoctors.Add(new Doctor
                    {
                        FirstName = first,
                        LastName = last,
                        NationalId = $"2{(dept.Id * 1000000 + i * 10000) % 1000000000:000000000}",
                        Email = $"{first.ToLower().Replace(" ", "")}.{last.ToLower().Replace(" ", "")}.{dept.Id}{i}@hospital.com",
                        PhoneNumber = $"010{dept.Id}{(i % 1000000):0000000}",
                        DateOfBirth = new DateTime(1965, 1, 1).AddDays((dept.Id * 50) + i * 30),
                        Gender = isMale ? "Male" : "Female",
                        Address = dept.Location,
                        LicenseNumber = $"LIC-{dept.Name.ToUpper().Replace(" ", string.Empty)}-{i:000}",
                        Specialization = dept.Name,
                        SubSpecialization = i % 3 == 0 ? "تخصص فرعي" : null,
                        YearsOfExperience = 5 + (i % 25),
                        Education = educationLevels[i % educationLevels.Length],
                        Certifications = certifications[i % certifications.Length],
                        Languages = i % 2 == 0 ? "العربية، الإنجليزية" : "العربية، الإنجليزية، الفرنسية",
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

            // Ensure more patients - Generate realistic Arabic names
            int currentPatients = await context.Patients.CountAsync();
            if (currentPatients < targetPatients)
            {
                var patientMaleFirstNames = new[] { "محمد", "أحمد", "علي", "خالد", "محمود", "يوسف", "حسام", "إبراهيم", "عمر", "طارق", "مصطفى", "عبدالله", "حسن", "سامي", "فاروق" };
                var patientFemaleFirstNames = new[] { "سارة", "فاطمة", "مريم", "نورا", "ريم", "يسرى", "هدى", "ليلى", "سلمى", "نادية", "أميرة", "رانيا", "منى", "هند", "أسماء" };
                var patientLastNames = new[] { "محمد", "علي", "حسن", "إبراهيم", "محمود", "السيد", "مصطفى", "عبدالرحمن", "خالد", "يوسف", "فاروق", "عطية", "شكري", "كامل", "صلاح" };
                var cities = new[] { "القاهرة", "الجيزة", "الإسكندرية", "المنصورة", "طنطا", "أسيوط", "سوهاج", "الأقصر", "أسوان", "بورسعيد", "الإسماعيلية", "السويس" };
                var streets = new[] { "شارع النيل", "شارع التحرير", "شارع كورنيش النيل", "شارع الهرم", "شارع المعادي", "شارع مصر الجديدة", "شارع الزمالك", "شارع المعادي" };
                
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
                    var address = $"{street}، {city}، مصر";
                    
                    var insuranceProviders = new[] { "شركة التأمين الصحي", "التأمين الصحي الشامل", "تأمين الصحة العامة", "تأمين القطاع الخاص", null };
                    var medicalHistories = new[] { "سكري", "ضغط الدم", "أمراض القلب", "أمراض الكلى", "أمراض الرئة", "لا توجد أمراض مزمنة", null };
                    var allergies = new[] { "بنسلين", "أسبرين", "أيبوبروفين", "لا توجد حساسية", null };
                    
                    addPatients.Add(new Patient
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        NationalId = $"2{(i % 1000000000):000000000}",
                        Email = $"{firstName.ToLower().Replace(" ", "")}.{lastName.ToLower().Replace(" ", "")}{i}@example.com",
                        PhoneNumber = $"01{(i % 10)}{(i % 10000000):00000000}",
                        DateOfBirth = new DateTime(1950, 1, 1).AddDays(i * 100 % 20000),
                        Gender = isMale ? "Male" : "Female",
                        Address = address,
                        EmergencyContactName = isMale ? $"أخ - {firstName}" : $"أخت - {firstName}",
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
            if (existingPrescriptions < 80)
            {
                var apptInfos = await context.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new { a.Id, a.PatientId, a.DoctorId, a.Status })
                    .Take(120)
                    .ToListAsync();

                var meds = await context.Medicines.Select(m => new { m.Id, m.Name, m.Price }).ToListAsync();
                if (meds.Count == 0)
                {
                    // No medicines available, skip prescription seeding
                }
                else
                {
                    var prxList = new List<Prescription>();
                    int limit = Math.Min(80 - existingPrescriptions, apptInfos.Count / 2);
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
            if (existingBills < 100)
            {
                var completedOrConfirmedAppts = await context.Appointments
                    .Where(a => a.Status == "Completed" || a.Status == "Confirmed")
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new { a.Id, a.PatientId, a.ConsultationFee })
                    .Take(120)
                    .ToListAsync();

                var billList = new List<Bill>();
                int toMake = Math.Min(100 - existingBills, completedOrConfirmedAppts.Count);
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

            // Schedules
            if (!await context.Schedules.AnyAsync())
            {
                var docIdsForSched = await context.Doctors.Select(d => d.Id).OrderBy(id => id).Take(2).ToListAsync();
                if (docIdsForSched.Count >= 1)
                {
                    var schedules = new List<Schedule>
                    {
                        new Schedule
                        {
                            DayOfWeek = DayOfWeek.Sunday,
                            StartTime = new TimeSpan(9,0,0),
                            EndTime = new TimeSpan(13,0,0),
                            DoctorId = docIdsForSched[0],
                            CreatedAt = DateTime.UtcNow
                        }
                    };
                    if (docIdsForSched.Count >= 2)
                    {
                        schedules.Add(new Schedule
                        {
                            DayOfWeek = DayOfWeek.Monday,
                            StartTime = new TimeSpan(14,0,0),
                            EndTime = new TimeSpan(18,0,0),
                            DoctorId = docIdsForSched[1],
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    context.Schedules.AddRange(schedules);
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
    }
}
