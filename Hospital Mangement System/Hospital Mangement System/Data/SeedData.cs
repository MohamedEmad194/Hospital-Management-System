using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Nurse", "Receptionist", "Pharmacist", "User" };

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
            // Departments
            if (!await context.Departments.AnyAsync())
            {
                var departments = new[]
            {
                new Department
                {
                    Name = "Cardiology",
                    Description = "Heart and cardiovascular diseases treatment",
                    HeadOfDepartment = "Dr. Ahmed Hassan",
                    PhoneNumber = "1234567890",
                    Location = "Floor 2, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Neurology",
                    Description = "Brain and nervous system disorders treatment",
                    HeadOfDepartment = "Dr. Sarah Mohamed",
                    PhoneNumber = "1234567891",
                    Location = "Floor 3, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Orthopedics",
                    Description = "Bone, joint, and muscle disorders treatment",
                    HeadOfDepartment = "Dr. Omar Ali",
                    PhoneNumber = "1234567892",
                    Location = "Floor 1, Building B",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Pediatrics",
                    Description = "Medical care for infants, children, and adolescents",
                    HeadOfDepartment = "Dr. Fatima Ahmed",
                    PhoneNumber = "1234567893",
                    Location = "Floor 2, Building B",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Name = "Emergency",
                    Description = "Emergency medical care and trauma treatment",
                    HeadOfDepartment = "Dr. Khalid Ibrahim",
                    PhoneNumber = "1234567894",
                    Location = "Ground Floor, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
                };

                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }

            // Rooms
            if (!await context.Rooms.AnyAsync())
            {
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
                    DepartmentId = 1,
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
                    DepartmentId = 2,
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
                    DepartmentId = 3,
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
                    DepartmentId = 4,
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
                    DepartmentId = 5,
                    CreatedAt = DateTime.UtcNow
                }
                };

                context.Rooms.AddRange(rooms);
                await context.SaveChangesAsync();
            }

            // Medicines
            if (!await context.Medicines.AnyAsync())
            {
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
                    StockQuantity = 800,
                    MinimumStockLevel = 80,
                    Unit = "tablet",
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    BatchNumber = "HP2024001",
                    RequiresPrescription = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
                };

                context.Medicines.AddRange(medicines);
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
                        NationalId = "1111111111",
                        Email = "ahmed.hassan@hospital.com",
                        PhoneNumber = "01000000001",
                        DateOfBirth = new DateTime(1980, 3, 20),
                        Gender = "Male",
                        Address = "Building A",
                        LicenseNumber = "LIC-CARD-001",
                        Specialization = "Cardiology",
                        SubSpecialization = "Interventional Cardiology",
                        YearsOfExperience = 15,
                        Education = "MD, Cardiology",
                        Certifications = "Board Certified",
                        Languages = "Arabic, English",
                        ConsultationFee = 250,
                        WorkingHoursStart = new TimeSpan(9,0,0),
                        WorkingHoursEnd = new TimeSpan(17,0,0),
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
                        PhoneNumber = "01000000002",
                        DateOfBirth = new DateTime(1982, 6, 12),
                        Gender = "Female",
                        Address = "Building A",
                        LicenseNumber = "LIC-NEURO-001",
                        Specialization = "Neurology",
                        SubSpecialization = "Epileptology",
                        YearsOfExperience = 13,
                        Education = "MD, Neurology",
                        Certifications = "Board Certified",
                        Languages = "Arabic, English",
                        ConsultationFee = 300,
                        WorkingHoursStart = new TimeSpan(10,0,0),
                        WorkingHoursEnd = new TimeSpan(18,0,0),
                        DepartmentId = 2,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Doctor
                    {
                        FirstName = "Omar",
                        LastName = "Ali",
                        NationalId = "3333333333",
                        Email = "omar.ali@hospital.com",
                        PhoneNumber = "01000000003",
                        DateOfBirth = new DateTime(1985, 11, 5),
                        Gender = "Male",
                        Address = "Building B",
                        LicenseNumber = "LIC-ORTHO-001",
                        Specialization = "Orthopedics",
                        YearsOfExperience = 10,
                        Education = "MS, Orthopedics",
                        Languages = "Arabic, English",
                        ConsultationFee = 220,
                        WorkingHoursStart = new TimeSpan(8,30,0),
                        WorkingHoursEnd = new TimeSpan(16,30,0),
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
                        FirstName = "Mohamed",
                        LastName = "Youssef",
                        NationalId = "9876543210",
                        Email = "m.youssef@example.com",
                        PhoneNumber = "01111111111",
                        DateOfBirth = new DateTime(1985,5,15),
                        Gender = "Male",
                        Address = "Cairo, Egypt",
                        EmergencyContactName = "A. Youssef",
                        EmergencyContactPhone = "01200000000",
                        InsuranceProvider = "HealthCo",
                        InsuranceNumber = "INS-001",
                        MedicalHistory = "No chronic disease",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Patient
                    {
                        FirstName = "Salma",
                        LastName = "Khaled",
                        NationalId = "8765432109",
                        Email = "salma.khaled@example.com",
                        PhoneNumber = "01122222222",
                        DateOfBirth = new DateTime(1992,3,10),
                        Gender = "Female",
                        Address = "Giza, Egypt",
                        EmergencyContactName = "K. Khaled",
                        EmergencyContactPhone = "01211111111",
                        InsuranceProvider = "MedicAid",
                        InsuranceNumber = "INS-002",
                        Allergies = "Penicillin",
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
                var appts = new[]
                {
                    new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date.AddDays(1),
                        AppointmentTime = new TimeSpan(10,0,0),
                        Status = "Scheduled",
                        Reason = "Chest pain",
                        Notes = "Patient reports intermittent pain",
                        ConsultationFee = 250,
                        PatientId = 1,
                        DoctorId = 1,
                        RoomId = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Appointment
                    {
                        AppointmentDate = DateTime.UtcNow.Date.AddDays(-1),
                        AppointmentTime = new TimeSpan(11,30,0),
                        Status = "Completed",
                        Reason = "Headache",
                        Diagnosis = "Migraine",
                        Treatment = "Pain management",
                        ConsultationFee = 300,
                        PatientId = 2,
                        DoctorId = 2,
                        RoomId = 2,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                context.Appointments.AddRange(appts);
                await context.SaveChangesAsync();
            }

            // Medical Records
            if (!await context.MedicalRecords.AnyAsync())
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
                    PatientId = 2,
                    DoctorId = 2,
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            // Prescriptions
            if (!await context.Prescriptions.AnyAsync())
            {
                var prescription = new Prescription
                {
                    PrescriptionDate = DateTime.UtcNow.Date,
                    ValidUntil = DateTime.UtcNow.Date.AddDays(7),
                    Instructions = "Take with food",
                    Notes = "As needed",
                    PatientId = 2,
                    DoctorId = 2,
                    CreatedAt = DateTime.UtcNow,
                    PrescriptionItems = new List<PrescriptionItem>
                    {
                        new PrescriptionItem
                        {
                            MedicineName = "Paracetamol",
                            Dosage = "500mg",
                            Frequency = "Twice daily",
                            Duration = "7 days",
                            Instructions = "Take with food",
                            Quantity = 14,
                            UnitPrice = 5m,
                            MedicineId = 1
                        }
                    }
                };
                context.Prescriptions.Add(prescription);
                await context.SaveChangesAsync();
            }

            // Bills
            if (!await context.Bills.AnyAsync())
            {
                var bill = new Bill
                {
                    BillDate = DateTime.UtcNow.Date,
                    DueDate = DateTime.UtcNow.Date.AddDays(7),
                    Notes = "Consultation and medicine",
                    InsuranceProvider = "HealthCo",
                    InsuranceNumber = "INS-001",
                    InsuranceCoverage = 80,
                    PatientId = 2,
                    CreatedAt = DateTime.UtcNow,
                    BillItems = new List<BillItem>
                    {
                        new BillItem
                        {
                            Description = "Consultation Fee",
                            Category = "Consultation",
                            Quantity = 1,
                            UnitPrice = 300m,
                            Notes = "Neurology consultation"
                        },
                        new BillItem
                        {
                            Description = "Paracetamol 500mg",
                            Category = "Medicine",
                            Quantity = 14,
                            UnitPrice = 5m,
                            Notes = "Pain relief"
                        }
                    }
                };
                context.Bills.Add(bill);
                await context.SaveChangesAsync();
            }

            // Schedules
            if (!await context.Schedules.AnyAsync())
            {
                var schedules = new[]
                {
                    new Schedule
                    {
                        DayOfWeek = DayOfWeek.Sunday,
                        StartTime = new TimeSpan(9,0,0),
                        EndTime = new TimeSpan(13,0,0),
                        DoctorId = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Schedule
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(14,0,0),
                        EndTime = new TimeSpan(18,0,0),
                        DoctorId = 2,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                context.Schedules.AddRange(schedules);
                await context.SaveChangesAsync();
            }
        }
    }
}
