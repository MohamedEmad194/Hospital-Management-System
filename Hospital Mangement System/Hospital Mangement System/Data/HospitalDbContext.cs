using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Models;

namespace Hospital_Management_System.Data
{
    public class HospitalDbContext : IdentityDbContext<User>
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<NursingUnit> NursingUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            ConfigureRelationships(builder);

            // Configure indexes
            ConfigureIndexes(builder);

            // Configure constraints
            ConfigureConstraints(builder);

            // Configure decimal precision
            ConfigureDecimalPrecision(builder);

            // Seed data
            SeedData(builder);
        }

        private void ConfigureRelationships(ModelBuilder builder)
        {
            // Patient relationships
            builder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Patient>()
                .HasMany(p => p.MedicalRecords)
                .WithOne(mr => mr.Patient)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Patient>()
                .HasMany(p => p.Prescriptions)
                .WithOne(p => p.Patient)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Patient>()
                .HasMany(p => p.Bills)
                .WithOne(b => b.Patient)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor relationships
            builder.Entity<Doctor>()
                .HasOne(d => d.Department)
                .WithMany(dept => dept.Doctors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Doctor>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Doctor>()
                .HasMany(d => d.MedicalRecords)
                .WithOne(mr => mr.Doctor)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Doctor>()
                .HasMany(d => d.Prescriptions)
                .WithOne(p => p.Doctor)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Doctor>()
                .HasMany(d => d.Schedules)
                .WithOne(s => s.Doctor)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Department relationships
            builder.Entity<Department>()
                .HasMany(d => d.Rooms)
                .WithOne(r => r.Department)
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Department>()
                .HasMany(d => d.StaffMembers)
                .WithOne(s => s.Department)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room relationships
            builder.Entity<Room>()
                .HasMany(r => r.Appointments)
                .WithOne(a => a.Room)
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prescription relationships
            builder.Entity<Prescription>()
                .HasMany(p => p.PrescriptionItems)
                .WithOne(pi => pi.Prescription)
                .HasForeignKey(pi => pi.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Medicine relationships
            builder.Entity<Medicine>()
                .HasMany(m => m.PrescriptionItems)
                .WithOne(pi => pi.Medicine)
                .HasForeignKey(pi => pi.MedicineId)
                .OnDelete(DeleteBehavior.SetNull);

            // Bill relationships
            builder.Entity<Bill>()
                .HasMany(b => b.BillItems)
                .WithOne(bi => bi.Bill)
                .HasForeignKey(bi => bi.BillId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureIndexes(ModelBuilder builder)
        {
            // Patient indexes
            builder.Entity<Patient>()
                .HasIndex(p => p.NationalId)
                .IsUnique();

            builder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

            // Doctor indexes
            builder.Entity<Doctor>()
                .HasIndex(d => d.NationalId)
                .IsUnique();

            builder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            builder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Staff indexes
            builder.Entity<Staff>()
                .HasIndex(s => s.NationalId)
                .IsUnique();

            builder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // Room indexes
            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            // Bill indexes
            builder.Entity<Bill>()
                .HasIndex(b => b.BillNumber)
                .IsUnique();

            // Medicine indexes
            builder.Entity<Medicine>()
                .HasIndex(m => m.Name);

            // NursingUnit indexes
            builder.Entity<NursingUnit>()
                .HasIndex(n => n.UnitId)
                .IsUnique();
        }

        private void ConfigureConstraints(ModelBuilder builder)
        {
            // Add check constraints using the new syntax
            builder.Entity<Patient>()
                .ToTable(t => t.HasCheckConstraint("CK_Patient_Gender", "Gender IN ('Male', 'Female', 'Other')"));

            builder.Entity<Doctor>()
                .ToTable(t => t.HasCheckConstraint("CK_Doctor_Gender", "Gender IN ('Male', 'Female', 'Other')"));

            builder.Entity<Staff>()
                .ToTable(t => t.HasCheckConstraint("CK_Staff_Gender", "Gender IN ('Male', 'Female', 'Other')"));

            builder.Entity<Appointment>()
                .ToTable(t => t.HasCheckConstraint("CK_Appointment_Status", 
                    "Status IN ('Scheduled', 'Confirmed', 'Completed', 'Cancelled', 'NoShow')"));

            builder.Entity<Bill>()
                .ToTable(t => t.HasCheckConstraint("CK_Bill_Status", 
                    "Status IN ('Pending', 'Paid', 'Overdue', 'Cancelled')"));

            builder.Entity<Prescription>()
                .ToTable(t => t.HasCheckConstraint("CK_Prescription_Status", 
                    "Status IN ('Active', 'Dispensed', 'Expired', 'Cancelled')"));
        }

        private void ConfigureDecimalPrecision(ModelBuilder builder)
        {
            // Configure decimal precision for money-related fields
            builder.Entity<Appointment>()
                .Property(a => a.ConsultationFee)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.SubTotal)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.TaxAmount)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.PaidAmount)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.RemainingAmount)
                .HasPrecision(18, 2);

            builder.Entity<Bill>()
                .Property(b => b.InsuranceCoverage)
                .HasPrecision(18, 2);

            builder.Entity<BillItem>()
                .Property(bi => bi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<BillItem>()
                .Property(bi => bi.TotalPrice)
                .HasPrecision(18, 2);

            builder.Entity<Staff>()
                .Property(s => s.Salary)
                .HasPrecision(18, 2);

            builder.Entity<Room>()
                .Property(r => r.HourlyRate)
                .HasPrecision(18, 2);

            builder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(18, 2);

            builder.Entity<Medicine>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            builder.Entity<PrescriptionItem>()
                .Property(pi => pi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<PrescriptionItem>()
                .Property(pi => pi.TotalPrice)
                .HasPrecision(18, 2);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Departments
            builder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Cardiology",
                    Description = "Heart and cardiovascular diseases",
                    HeadOfDepartment = "Dr. Ahmed Hassan",
                    PhoneNumber = "1234567890",
                    Location = "Floor 2, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Id = 2,
                    Name = "Neurology",
                    Description = "Brain and nervous system disorders",
                    HeadOfDepartment = "Dr. Sarah Mohamed",
                    PhoneNumber = "1234567891",
                    Location = "Floor 3, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Id = 3,
                    Name = "Orthopedics",
                    Description = "Bone, joint, and muscle disorders",
                    HeadOfDepartment = "Dr. Omar Ali",
                    PhoneNumber = "1234567892",
                    Location = "Floor 1, Building B",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Department
                {
                    Id = 4,
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
                    Id = 5,
                    Name = "Emergency",
                    Description = "Emergency medical care",
                    HeadOfDepartment = "Dr. Khalid Ibrahim",
                    PhoneNumber = "1234567894",
                    Location = "Ground Floor, Building A",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed Rooms
            builder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
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
                    Id = 4,
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
                    Id = 5,
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
            );

            // Seed Medicines
            builder.Entity<Medicine>().HasData(
                new Medicine
                {
                    Id = 1,
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
                    Id = 2,
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
                }
            );
        }
    }
}
