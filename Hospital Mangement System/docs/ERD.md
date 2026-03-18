# Entity Relationship Diagram (ERD)
## Hospital Management System Database Schema

### Overview
This document describes the complete Entity Relationship Diagram for the Hospital Management System database, including all entities, their attributes, and relationships.

---

## Entity Definitions

### 1. BaseEntity (Abstract)
Base class for all entities providing common fields.

**Attributes:**
- `Id` (int, PK) - Primary key
- `CreatedAt` (DateTime) - Creation timestamp
- `UpdatedAt` (DateTime?) - Last update timestamp
- `IsDeleted` (bool) - Soft delete flag

---

### 2. User
Extends IdentityUser for authentication and authorization.

**Attributes:**
- `Id` (string, PK) - Identity primary key
- `FirstName` (string, required, max 100)
- `LastName` (string, required, max 100)
- `NationalId` (string, max 20)
- `DateOfBirth` (DateTime?)
- `Gender` (string, max 10)
- `Address` (string, max 500)
- `PhoneNumber2` (string, max 20)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `IsActive` (bool)
- Identity fields: `Email`, `UserName`, `PasswordHash`, etc.

**Relationships:**
- One-to-Many with Patient (optional, navigation only)
- One-to-Many with Doctor (optional, navigation only)
- One-to-Many with Staff (optional, navigation only)

---

### 3. Patient
Represents patients in the hospital system.

**Attributes:**
- `Id` (int, PK)
- `FirstName` (string, required, max 100)
- `LastName` (string, required, max 100)
- `NationalId` (string, required, max 20, **Unique Index**)
- `Email` (string, required, max 256, **Unique Index**)
- `PhoneNumber` (string, required, max 20)
- `PhoneNumber2` (string, max 20)
- `DateOfBirth` (DateTime)
- `Gender` (string, max 10) - Check constraint: 'Male', 'Female', 'Other'
- `Address` (string, max 500)
- `EmergencyContactName` (string, max 100)
- `EmergencyContactPhone` (string, max 20)
- `InsuranceProvider` (string, max 100)
- `InsuranceNumber` (string, max 50)
- `MedicalHistory` (string, max 500)
- `Allergies` (string, max 500)
- `CurrentMedications` (string, max 500)
- `IsActive` (bool)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- One-to-Many: `Appointments` (FK: PatientId, Delete: Restrict)
- One-to-Many: `MedicalRecords` (FK: PatientId, Delete: Restrict)
- One-to-Many: `Prescriptions` (FK: PatientId, Delete: Restrict)
- One-to-Many: `Bills` (FK: PatientId, Delete: Restrict)

---

### 4. Doctor
Represents doctors working in the hospital.

**Attributes:**
- `Id` (int, PK)
- `FirstName` (string, required, max 100)
- `LastName` (string, required, max 100)
- `NationalId` (string, required, max 20, **Unique Index**)
- `Email` (string, required, max 256, **Unique Index**)
- `PhoneNumber` (string, required, max 20)
- `PhoneNumber2` (string, max 20)
- `DateOfBirth` (DateTime)
- `Gender` (string, max 10) - Check constraint: 'Male', 'Female', 'Other'
- `Address` (string, max 500)
- `LicenseNumber` (string, required, max 50, **Unique Index**)
- `Specialization` (string, required, max 100)
- `SubSpecialization` (string, max 100)
- `YearsOfExperience` (int)
- `Education` (string, max 1000)
- `Certifications` (string, max 1000)
- `Languages` (string, max 500)
- `ConsultationFee` (decimal(18,2))
- `WorkingHoursStart` (TimeSpan)
- `WorkingHoursEnd` (TimeSpan)
- `IsAvailable` (bool)
- `IsActive` (bool)
- `DepartmentId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Department` (FK: DepartmentId, Delete: Restrict)
- One-to-Many: `Appointments` (FK: DoctorId, Delete: Restrict)
- One-to-Many: `MedicalRecords` (FK: DoctorId, Delete: Restrict)
- One-to-Many: `Prescriptions` (FK: DoctorId, Delete: Restrict)
- One-to-Many: `Schedules` (FK: DoctorId, Delete: Cascade)

---

### 5. Department
Represents hospital departments.

**Attributes:**
- `Id` (int, PK)
- `Name` (string, required, max 100)
- `Description` (string, max 500)
- `HeadOfDepartment` (string, max 100)
- `PhoneNumber` (string, max 20)
- `Location` (string, max 100)
- `IsActive` (bool)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- One-to-Many: `Doctors` (FK: DepartmentId, Delete: Restrict)
- One-to-Many: `Rooms` (FK: DepartmentId, Delete: Restrict)
- One-to-Many: `StaffMembers` (FK: DepartmentId, Delete: Restrict)

---

### 6. Staff
Represents non-doctor staff members.

**Attributes:**
- `Id` (int, PK)
- `FirstName` (string, required, max 100)
- `LastName` (string, required, max 100)
- `NationalId` (string, required, max 20, **Unique Index**)
- `Email` (string, required, max 256, **Unique Index**)
- `PhoneNumber` (string, required, max 20)
- `PhoneNumber2` (string, max 20)
- `DateOfBirth` (DateTime)
- `Gender` (string, max 10) - Check constraint: 'Male', 'Female', 'Other'
- `Address` (string, max 500)
- `Position` (string, required, max 100)
- `EmployeeId` (string, max 50)
- `HireDate` (DateTime)
- `Salary` (decimal(18,2))
- `Qualification` (string, max 100)
- `Skills` (string, max 500)
- `WorkingHoursStart` (TimeSpan)
- `WorkingHoursEnd` (TimeSpan)
- `IsActive` (bool)
- `DepartmentId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Department` (FK: DepartmentId, Delete: Restrict)

---

### 7. Appointment
Represents patient appointments with doctors.

**Attributes:**
- `Id` (int, PK)
- `AppointmentDate` (DateTime, required)
- `AppointmentTime` (TimeSpan, required)
- `Status` (string, max 20) - Check constraint: 'Scheduled', 'Confirmed', 'Completed', 'Cancelled', 'NoShow'
- `Reason` (string, max 500)
- `Notes` (string, max 1000)
- `Diagnosis` (string, max 1000)
- `Treatment` (string, max 500)
- `ConsultationFee` (decimal(18,2)?)
- `IsFollowUp` (bool)
- `PatientId` (int, FK, required)
- `DoctorId` (int, FK, required)
- `RoomId` (int, FK, nullable)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Patient` (FK: PatientId, Delete: Restrict)
- Many-to-One: `Doctor` (FK: DoctorId, Delete: Restrict)
- Many-to-One: `Room` (FK: RoomId, Delete: SetNull)

---

### 8. Room
Represents hospital rooms.

**Attributes:**
- `Id` (int, PK)
- `RoomNumber` (string, required, max 20, **Unique Index**)
- `RoomType` (string, required, max 50) - e.g., 'Consultation', 'Surgery', 'ICU', 'Ward', 'Emergency'
- `Floor` (string, max 100)
- `Building` (string, max 100)
- `Description` (string, max 500)
- `Capacity` (int)
- `HourlyRate` (decimal(18,2)?)
- `IsAvailable` (bool)
- `IsActive` (bool)
- `DepartmentId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Department` (FK: DepartmentId, Delete: Restrict)
- One-to-Many: `Appointments` (FK: RoomId, Delete: SetNull)

---

### 9. MedicalRecord
Represents patient medical records.

**Attributes:**
- `Id` (int, PK)
- `RecordDate` (DateTime, required)
- `RecordType` (string, required, max 100) - e.g., 'Consultation', 'Diagnosis', 'Treatment', 'Surgery', 'Lab'
- `Symptoms` (string, max 1000)
- `Diagnosis` (string, max 1000)
- `Treatment` (string, max 1000)
- `Prescription` (string, max 1000)
- `Notes` (string, max 1000)
- `LabResults` (string, max 1000)
- `ImagingResults` (string, max 1000)
- `VitalSigns` (string, max 100)
- `BloodPressure` (string, max 100)
- `Temperature` (string, max 100)
- `HeartRate` (string, max 100)
- `Weight` (string, max 100)
- `Height` (string, max 100)
- `PatientId` (int, FK, required)
- `DoctorId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Patient` (FK: PatientId, Delete: Restrict)
- Many-to-One: `Doctor` (FK: DoctorId, Delete: Restrict)

---

### 10. Prescription
Represents prescriptions issued to patients.

**Attributes:**
- `Id` (int, PK)
- `PrescriptionDate` (DateTime, required)
- `ValidUntil` (DateTime, required)
- `Instructions` (string, max 1000)
- `Notes` (string, max 500)
- `IsDispensed` (bool)
- `DispensedDate` (DateTime?)
- `Status` (string, max 100) - Check constraint: 'Active', 'Dispensed', 'Expired', 'Cancelled'
- `PatientId` (int, FK, required)
- `DoctorId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Patient` (FK: PatientId, Delete: Restrict)
- Many-to-One: `Doctor` (FK: DoctorId, Delete: Restrict)
- One-to-Many: `PrescriptionItems` (FK: PrescriptionId, Delete: Cascade)

---

### 11. PrescriptionItem
Represents individual medicines in a prescription.

**Attributes:**
- `Id` (int, PK)
- `MedicineName` (string, required, max 100)
- `Dosage` (string, max 100)
- `Frequency` (string, max 50) - e.g., 'Once daily', 'Twice daily'
- `Duration` (string, max 100) - e.g., '7 days', '2 weeks'
- `Instructions` (string, max 500)
- `Quantity` (int)
- `UnitPrice` (decimal(18,2))
- `TotalPrice` (decimal(18,2))
- `PrescriptionId` (int, FK, required)
- `MedicineId` (int, FK, nullable)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Prescription` (FK: PrescriptionId, Delete: Cascade)
- Many-to-One: `Medicine` (FK: MedicineId, Delete: SetNull)

---

### 12. Medicine
Represents medicines in the hospital inventory.

**Attributes:**
- `Id` (int, PK)
- `Name` (string, required, max 100, **Index**)
- `GenericName` (string, max 100)
- `DosageForm` (string, max 50) - e.g., 'Tablet', 'Syrup', 'Injection'
- `Strength` (string, max 100)
- `Manufacturer` (string, max 100)
- `Description` (string, max 500)
- `Indications` (string, max 1000)
- `Contraindications` (string, max 1000)
- `SideEffects` (string, max 1000)
- `DosageInstructions` (string, max 1000)
- `Price` (decimal(18,2))
- `StockQuantity` (int)
- `MinimumStockLevel` (int)
- `Unit` (string, max 20) - e.g., 'mg', 'ml', 'tablet'
- `ExpiryDate` (DateTime?)
- `BatchNumber` (string, max 50)
- `RequiresPrescription` (bool)
- `IsActive` (bool)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- One-to-Many: `PrescriptionItems` (FK: MedicineId, Delete: SetNull)

---

### 13. Bill
Represents bills for patient services.

**Attributes:**
- `Id` (int, PK)
- `BillNumber` (string, required, max 50, **Unique Index**)
- `BillDate` (DateTime, required)
- `DueDate` (DateTime, required)
- `Status` (string, max 20) - Check constraint: 'Pending', 'Paid', 'Overdue', 'Cancelled'
- `SubTotal` (decimal(18,2))
- `TaxAmount` (decimal(18,2))
- `DiscountAmount` (decimal(18,2))
- `TotalAmount` (decimal(18,2))
- `PaidAmount` (decimal(18,2))
- `RemainingAmount` (decimal(18,2))
- `PaymentMethod` (string, max 100)
- `PaymentDate` (DateTime?)
- `Notes` (string, max 500)
- `InsuranceProvider` (string, max 100)
- `InsuranceNumber` (string, max 50)
- `InsuranceCoverage` (decimal(18,2)?)
- `PatientId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Patient` (FK: PatientId, Delete: Restrict)
- One-to-Many: `BillItems` (FK: BillId, Delete: Cascade)

---

### 14. BillItem
Represents individual items in a bill.

**Attributes:**
- `Id` (int, PK)
- `Description` (string, required, max 200)
- `Category` (string, max 50) - e.g., 'Consultation', 'Medicine', 'Lab', 'Surgery'
- `Quantity` (int)
- `UnitPrice` (decimal(18,2))
- `TotalPrice` (decimal(18,2))
- `Notes` (string, max 100)
- `BillId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Bill` (FK: BillId, Delete: Cascade)

---

### 15. Schedule
Represents doctor schedules.

**Attributes:**
- `Id` (int, PK)
- `DayOfWeek` (DayOfWeek, required)
- `StartTime` (TimeSpan, required)
- `EndTime` (TimeSpan, required)
- `Notes` (string, max 100)
- `IsAvailable` (bool)
- `DoctorId` (int, FK, required)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- Many-to-One: `Doctor` (FK: DoctorId, Delete: Cascade)

---

### 16. Feature
Represents system features (for UI display).

**Attributes:**
- `Id` (int, PK)
- `Icon` (string, required, max 10)
- `TitleEn` (string, required, max 100)
- `TitleAr` (string, required, max 100)
- `DescriptionEn` (string, required, max 500)
- `DescriptionAr` (string, required, max 500)
- `Color` (string, max 20)
- `DisplayOrder` (int)
- `IsActive` (bool)
- BaseEntity fields: `CreatedAt`, `UpdatedAt`, `IsDeleted`

**Relationships:**
- None

---

## Entity Relationship Diagram (Text Representation)

```
┌──────────────┐
│    User      │ (Identity)
│ (Identity)   │
└──────────────┘
      │
      │ (1:N - Optional)
      ├─────────┐──────────┐
      │         │          │
┌─────▼────┐ ┌──▼─────┐ ┌─▼─────┐
│ Patient  │ │ Doctor │ │ Staff │
└────┬─────┘ └──┬─────┘ └───────┘
     │          │
     │          │
     │          ├──────────────┐
     │          │              │
     │     ┌────▼────────┐ ┌───▼────────┐
     │     │ Department  │ │  Schedule  │
     │     └────┬────────┘ └────────────┘
     │          │
     │          │
     │     ┌────▼─────┐
     │     │   Room   │
     │     └────┬─────┘
     │          │
     │          │
┌────▼──────────▼─────┐
│    Appointment      │
└─────────────────────┘

┌───────────┐      ┌─────────────────┐      ┌──────────────┐
│  Patient  │──────│ MedicalRecord   │──────│    Doctor    │
└───────────┘ (1:N)└─────────────────┘(1:N) └──────────────┘

┌───────────┐      ┌─────────────────┐      ┌──────────────┐
│  Patient  │──────│  Prescription   │──────│    Doctor    │
└───────────┘ (1:N)└────────┬────────┘(1:N) └──────────────┘
                            │
                            │ (1:N)
                            │
                   ┌────────▼────────┐      ┌──────────────┐
                   │PrescriptionItem │──────│   Medicine   │
                   └─────────────────┘(N:1) └──────────────┘

┌───────────┐      ┌─────────────────┐      ┌──────────────┐
│  Patient  │──────│      Bill       │      │              │
└───────────┘ (1:N)└────────┬────────┘      │              │
                            │               │              │
                            │ (1:N)         │              │
                            │               │              │
                   ┌────────▼────────┐      │              │
                   │   BillItem      │      │              │
                   └─────────────────┘      │              │
```

---

## Relationship Summary

### One-to-Many Relationships

1. **Department → Doctors** (1:N)
   - Department can have many Doctors
   - Delete: Restrict

2. **Department → Rooms** (1:N)
   - Department can have many Rooms
   - Delete: Restrict

3. **Department → Staff** (1:N)
   - Department can have many Staff members
   - Delete: Restrict

4. **Patient → Appointments** (1:N)
   - Patient can have many Appointments
   - Delete: Restrict

5. **Patient → MedicalRecords** (1:N)
   - Patient can have many Medical Records
   - Delete: Restrict

6. **Patient → Prescriptions** (1:N)
   - Patient can have many Prescriptions
   - Delete: Restrict

7. **Patient → Bills** (1:N)
   - Patient can have many Bills
   - Delete: Restrict

8. **Doctor → Appointments** (1:N)
   - Doctor can have many Appointments
   - Delete: Restrict

9. **Doctor → MedicalRecords** (1:N)
   - Doctor can have many Medical Records
   - Delete: Restrict

10. **Doctor → Prescriptions** (1:N)
    - Doctor can have many Prescriptions
    - Delete: Restrict

11. **Doctor → Schedules** (1:N)
    - Doctor can have many Schedules
    - Delete: Cascade

12. **Room → Appointments** (1:N)
    - Room can have many Appointments
    - Delete: SetNull

13. **Prescription → PrescriptionItems** (1:N)
    - Prescription can have many Prescription Items
    - Delete: Cascade

14. **Medicine → PrescriptionItems** (1:N)
    - Medicine can have many Prescription Items
    - Delete: SetNull

15. **Bill → BillItems** (1:N)
    - Bill can have many Bill Items
    - Delete: Cascade

---

## Indexes

### Unique Indexes
- `Patient.NationalId` (Unique)
- `Patient.Email` (Unique)
- `Doctor.NationalId` (Unique)
- `Doctor.Email` (Unique)
- `Doctor.LicenseNumber` (Unique)
- `Staff.NationalId` (Unique)
- `Staff.Email` (Unique)
- `Room.RoomNumber` (Unique)
- `Bill.BillNumber` (Unique)

### Regular Indexes
- `Medicine.Name` (Index)

---

## Check Constraints

1. **Patient.Gender**: IN ('Male', 'Female', 'Other')
2. **Doctor.Gender**: IN ('Male', 'Female', 'Other')
3. **Staff.Gender**: IN ('Male', 'Female', 'Other')
4. **Appointment.Status**: IN ('Scheduled', 'Confirmed', 'Completed', 'Cancelled', 'NoShow')
5. **Bill.Status**: IN ('Pending', 'Paid', 'Overdue', 'Cancelled')
6. **Prescription.Status**: IN ('Active', 'Dispensed', 'Expired', 'Cancelled')

---

## Decimal Precision

All monetary fields use `decimal(18,2)` precision:
- `Appointment.ConsultationFee`
- `Doctor.ConsultationFee`
- `Bill.SubTotal`, `TaxAmount`, `DiscountAmount`, `TotalAmount`, `PaidAmount`, `RemainingAmount`, `InsuranceCoverage`
- `BillItem.UnitPrice`, `TotalPrice`
- `Medicine.Price`
- `PrescriptionItem.UnitPrice`, `TotalPrice`
- `Room.HourlyRate`
- `Staff.Salary`

---

## Soft Delete Pattern

All entities inherit from `BaseEntity` which includes:
- `IsDeleted` flag for soft deletion
- `CreatedAt` timestamp
- `UpdatedAt` nullable timestamp

This allows for data recovery and audit trails without permanent deletion.

---

## Notes

- All foreign keys have appropriate navigation properties
- Delete behaviors are configured to maintain referential integrity
- Unique constraints ensure data consistency (e.g., NationalId, Email)
- Check constraints enforce valid enum values
- Decimal precision is standardized for financial calculations

