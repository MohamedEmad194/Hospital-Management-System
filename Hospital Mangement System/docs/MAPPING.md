# Entity to DTO Mapping Documentation
## AutoMapper Configuration Reference

### Overview
This document describes all the mappings between Entity models and DTOs (Data Transfer Objects) configured in the AutoMapper profile. These mappings are used to transform data between the database layer and the API layer.

**Location:** `Hospital Mangement System/Hospital Mangement System/Mappings/AutoMapperProfile.cs`

---

## Mapping Configuration

### 1. Patient Mappings

#### Patient → PatientDto
**Source:** `Patient` Entity  
**Destination:** `PatientDto`

**Mapping:** Direct property mapping
- All properties are mapped automatically
- Includes BaseEntity properties (Id, CreatedAt, UpdatedAt, IsDeleted)

**Usage:**
```csharp
var patientDto = _mapper.Map<PatientDto>(patient);
```

---

#### CreatePatientDto → Patient
**Source:** `CreatePatientDto`  
**Destination:** `Patient` Entity

**Mapping:** Direct property mapping
- Maps all properties from DTO to Entity
- BaseEntity fields (CreatedAt, etc.) are set by the system

**Usage:**
```csharp
var patient = _mapper.Map<Patient>(createPatientDto);
_context.Patients.Add(patient);
```

---

#### UpdatePatientDto → Patient
**Source:** `UpdatePatientDto`  
**Destination:** `Patient` Entity

**Mapping:** Conditional mapping
- Only non-null properties are mapped
- Uses `ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))`
- Allows partial updates

**Usage:**
```csharp
_mapper.Map(updatePatientDto, existingPatient);
```

---

### 2. Doctor Mappings

#### Doctor → DoctorDto
**Source:** `Doctor` Entity  
**Destination:** `DoctorDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mapping:** `DepartmentName` is computed from `Doctor.Department.Name`
  ```csharp
  .ForMember(dest => dest.DepartmentName, 
      opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
  ```

**Usage:**
```csharp
var doctorDto = _mapper.Map<DoctorDto>(doctor);
// doctorDto.DepartmentName will be populated if Department is loaded
```

---

#### CreateDoctorDto → Doctor
**Source:** `CreateDoctorDto`  
**Destination:** `Doctor` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var doctor = _mapper.Map<Doctor>(createDoctorDto);
_context.Doctors.Add(doctor);
```

---

#### UpdateDoctorDto → Doctor
**Source:** `UpdateDoctorDto`  
**Destination:** `Doctor` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateDoctorDto, existingDoctor);
```

---

### 3. Department Mappings

#### Department → DepartmentDto
**Source:** `Department` Entity  
**Destination:** `DepartmentDto`

**Mapping:** Direct property mapping

**Usage:**
```csharp
var departmentDto = _mapper.Map<DepartmentDto>(department);
```

---

#### CreateDepartmentDto → Department
**Source:** `CreateDepartmentDto`  
**Destination:** `Department` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var department = _mapper.Map<Department>(createDepartmentDto);
_context.Departments.Add(department);
```

---

#### UpdateDepartmentDto → Department
**Source:** `UpdateDepartmentDto`  
**Destination:** `Department` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateDepartmentDto, existingDepartment);
```

---

### 4. Appointment Mappings

#### Appointment → AppointmentDto
**Source:** `Appointment` Entity  
**Destination:** `AppointmentDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mappings:**
  - `PatientName`: Computed as `$"{Patient.FirstName} {Patient.LastName}"`
  - `DoctorName`: Computed as `$"{Doctor.FirstName} {Doctor.LastName}"`
  - `RoomNumber`: Computed as `Room.RoomNumber`

**Computed Properties:**
```csharp
.ForMember(dest => dest.PatientName, 
    opt => opt.MapFrom(src => src.Patient != null ? 
        $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
.ForMember(dest => dest.DoctorName, 
    opt => opt.MapFrom(src => src.Doctor != null ? 
        $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null))
.ForMember(dest => dest.RoomNumber, 
    opt => opt.MapFrom(src => src.Room != null ? src.Room.RoomNumber : null))
```

**Usage:**
```csharp
var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
// Requires eager loading: .Include(a => a.Patient).Include(a => a.Doctor).Include(a => a.Room)
```

---

#### CreateAppointmentDto → Appointment
**Source:** `CreateAppointmentDto`  
**Destination:** `Appointment` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var appointment = _mapper.Map<Appointment>(createAppointmentDto);
_context.Appointments.Add(appointment);
```

---

#### UpdateAppointmentDto → Appointment
**Source:** `UpdateAppointmentDto`  
**Destination:** `Appointment` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateAppointmentDto, existingAppointment);
```

---

### 5. MedicalRecord Mappings

#### MedicalRecord → MedicalRecordDto
**Source:** `MedicalRecord` Entity  
**Destination:** `MedicalRecordDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mappings:**
  - `PatientName`: Computed as `$"{Patient.FirstName} {Patient.LastName}"`
  - `DoctorName`: Computed as `$"{Doctor.FirstName} {Doctor.LastName}"`

**Computed Properties:**
```csharp
.ForMember(dest => dest.PatientName, 
    opt => opt.MapFrom(src => src.Patient != null ? 
        $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
.ForMember(dest => dest.DoctorName, 
    opt => opt.MapFrom(src => src.Doctor != null ? 
        $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null))
```

**Usage:**
```csharp
var medicalRecordDto = _mapper.Map<MedicalRecordDto>(medicalRecord);
// Requires eager loading: .Include(mr => mr.Patient).Include(mr => mr.Doctor)
```

---

#### CreateMedicalRecordDto → MedicalRecord
**Source:** `CreateMedicalRecordDto`  
**Destination:** `MedicalRecord` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var medicalRecord = _mapper.Map<MedicalRecord>(createMedicalRecordDto);
_context.MedicalRecords.Add(medicalRecord);
```

---

#### UpdateMedicalRecordDto → MedicalRecord
**Source:** `UpdateMedicalRecordDto`  
**Destination:** `MedicalRecord` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateMedicalRecordDto, existingMedicalRecord);
```

---

### 6. Medicine Mappings

#### Medicine → MedicineDto
**Source:** `Medicine` Entity  
**Destination:** `MedicineDto`

**Mapping:** Direct property mapping

**Usage:**
```csharp
var medicineDto = _mapper.Map<MedicineDto>(medicine);
```

---

#### CreateMedicineDto → Medicine
**Source:** `CreateMedicineDto`  
**Destination:** `Medicine` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var medicine = _mapper.Map<Medicine>(createMedicineDto);
_context.Medicines.Add(medicine);
```

---

#### UpdateMedicineDto → Medicine
**Source:** `UpdateMedicineDto`  
**Destination:** `Medicine` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateMedicineDto, existingMedicine);
```

---

### 7. Bill Mappings

#### Bill → BillDto
**Source:** `Bill` Entity  
**Destination:** `BillDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mapping:** `PatientName` computed as `$"{Patient.FirstName} {Patient.LastName}"`

**Computed Property:**
```csharp
.ForMember(dest => dest.PatientName, 
    opt => opt.MapFrom(src => src.Patient != null ? 
        $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
```

**Usage:**
```csharp
var billDto = _mapper.Map<BillDto>(bill);
// Requires eager loading: .Include(b => b.Patient)
```

---

#### CreateBillDto → Bill
**Source:** `CreateBillDto`  
**Destination:** `Bill` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var bill = _mapper.Map<Bill>(createBillDto);
_context.Bills.Add(bill);
```

---

#### UpdateBillDto → Bill
**Source:** `UpdateBillDto`  
**Destination:** `Bill` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateBillDto, existingBill);
```

---

### 8. BillItem Mappings

#### BillItem → BillItemDto
**Source:** `BillItem` Entity  
**Destination:** `BillItemDto`

**Mapping:** Direct property mapping

**Usage:**
```csharp
var billItemDto = _mapper.Map<BillItemDto>(billItem);
```

---

#### CreateBillItemDto → BillItem
**Source:** `CreateBillItemDto`  
**Destination:** `BillItem` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var billItem = _mapper.Map<BillItem>(createBillItemDto);
_context.BillItems.Add(billItem);
```

---

### 9. Room Mappings

#### Room → RoomDto
**Source:** `Room` Entity  
**Destination:** `RoomDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mapping:** `DepartmentName` computed as `Room.Department.Name`

**Computed Property:**
```csharp
.ForMember(dest => dest.DepartmentName, 
    opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
```

**Usage:**
```csharp
var roomDto = _mapper.Map<RoomDto>(room);
// Requires eager loading: .Include(r => r.Department)
```

---

#### CreateRoomDto → Room
**Source:** `CreateRoomDto`  
**Destination:** `Room` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var room = _mapper.Map<Room>(createRoomDto);
_context.Rooms.Add(room);
```

---

#### UpdateRoomDto → Room
**Source:** `UpdateRoomDto`  
**Destination:** `Room` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateRoomDto, existingRoom);
```

---

### 10. Schedule Mappings

#### Schedule → ScheduleDto
**Source:** `Schedule` Entity  
**Destination:** `ScheduleDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mapping:** `DoctorName` computed as `$"{Doctor.FirstName} {Doctor.LastName}"`

**Computed Property:**
```csharp
.ForMember(dest => dest.DoctorName, 
    opt => opt.MapFrom(src => src.Doctor != null ? 
        $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null))
```

**Usage:**
```csharp
var scheduleDto = _mapper.Map<ScheduleDto>(schedule);
// Requires eager loading: .Include(s => s.Doctor)
```

---

#### CreateScheduleDto → Schedule
**Source:** `CreateScheduleDto`  
**Destination:** `Schedule` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var schedule = _mapper.Map<Schedule>(createScheduleDto);
_context.Schedules.Add(schedule);
```

---

#### UpdateScheduleDto → Schedule
**Source:** `UpdateScheduleDto`  
**Destination:** `Schedule` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updateScheduleDto, existingSchedule);
```

---

### 11. Prescription Mappings

#### Prescription → PrescriptionDto
**Source:** `Prescription` Entity  
**Destination:** `PrescriptionDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mappings:**
  - `PatientName`: Computed as `$"{Patient.FirstName} {Patient.LastName}"`
  - `DoctorName`: Computed as `$"{Doctor.FirstName} {Doctor.LastName}"`

**Computed Properties:**
```csharp
.ForMember(dest => dest.PatientName, 
    opt => opt.MapFrom(src => src.Patient != null ? 
        $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
.ForMember(dest => dest.DoctorName, 
    opt => opt.MapFrom(src => src.Doctor != null ? 
        $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null))
```

**Usage:**
```csharp
var prescriptionDto = _mapper.Map<PrescriptionDto>(prescription);
// Requires eager loading: .Include(p => p.Patient).Include(p => p.Doctor)
```

---

#### CreatePrescriptionDto → Prescription
**Source:** `CreatePrescriptionDto`  
**Destination:** `Prescription` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var prescription = _mapper.Map<Prescription>(createPrescriptionDto);
_context.Prescriptions.Add(prescription);
```

---

#### UpdatePrescriptionDto → Prescription
**Source:** `UpdatePrescriptionDto`  
**Destination:** `Prescription` Entity

**Mapping:** Conditional mapping (null values ignored)

**Usage:**
```csharp
_mapper.Map(updatePrescriptionDto, existingPrescription);
```

---

### 12. PrescriptionItem Mappings

#### PrescriptionItem → PrescriptionItemDto
**Source:** `PrescriptionItem` Entity  
**Destination:** `PrescriptionItemDto`

**Mapping:** Direct + computed properties
- Maps all properties directly
- **Additional mapping:** `MedicineGenericName` computed as `Medicine.GenericName`

**Computed Property:**
```csharp
.ForMember(dest => dest.MedicineGenericName, 
    opt => opt.MapFrom(src => src.Medicine != null ? src.Medicine.GenericName : null))
```

**Usage:**
```csharp
var prescriptionItemDto = _mapper.Map<PrescriptionItemDto>(prescriptionItem);
// Requires eager loading: .Include(pi => pi.Medicine)
```

---

#### CreatePrescriptionItemDto → PrescriptionItem
**Source:** `CreatePrescriptionItemDto`  
**Destination:** `PrescriptionItem` Entity

**Mapping:** Direct property mapping

**Usage:**
```csharp
var prescriptionItem = _mapper.Map<PrescriptionItem>(createPrescriptionItemDto);
_context.PrescriptionItems.Add(prescriptionItem);
```

---

### 13. User Mappings

#### User → UserDto
**Source:** `User` Entity (Identity)  
**Destination:** `UserDto`

**Mapping:** Direct property mapping
- Maps Identity properties and custom User properties

**Usage:**
```csharp
var userDto = _mapper.Map<UserDto>(user);
```

---

## Mapping Patterns

### 1. Direct Mapping
Most mappings use direct property-to-property mapping when property names match.

**Example:**
```csharp
CreateMap<Patient, PatientDto>();
```

### 2. Conditional Mapping (Update DTOs)
Update DTOs use conditional mapping to ignore null values, allowing partial updates.

**Example:**
```csharp
CreateMap<UpdatePatientDto, Patient>()
    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
```

### 3. Computed Properties
Some DTOs include computed properties from related entities.

**Example:**
```csharp
CreateMap<Doctor, DoctorDto>()
    .ForMember(dest => dest.DepartmentName, 
        opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));
```

### 4. Concatenated Properties
Some DTOs concatenate related entity properties for display.

**Example:**
```csharp
CreateMap<Appointment, AppointmentDto>()
    .ForMember(dest => dest.PatientName, 
        opt => opt.MapFrom(src => src.Patient != null ? 
            $"{src.Patient.FirstName} {src.Patient.LastName}" : null));
```

---

## Best Practices

### Eager Loading for Computed Properties
When mapping entities with computed properties from related entities, ensure eager loading:

**Good:**
```csharp
var appointment = await _context.Appointments
    .Include(a => a.Patient)
    .Include(a => a.Doctor)
    .Include(a => a.Room)
    .FirstOrDefaultAsync(a => a.Id == id);
    
var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
```

**Avoid:**
```csharp
var appointment = await _context.Appointments
    .FirstOrDefaultAsync(a => a.Id == id);
    
var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
// PatientName, DoctorName, RoomNumber will be null
```

### Null Safety
All computed property mappings include null checks:

```csharp
src.Patient != null ? src.Patient.Name : null
```

This prevents NullReferenceException if related entities are not loaded.

### Partial Updates
Use conditional mapping for update operations to allow partial updates:

```csharp
_mapper.Map(updateDto, existingEntity);
// Only non-null properties in updateDto will be mapped
```

---

## DTO Structure Summary

### Read DTOs (Entity → DTO)
- Include all entity properties
- May include computed properties from related entities
- Used for GET operations

### Create DTOs (DTO → Entity)
- Include only properties required for creation
- Exclude system-generated fields (Id, CreatedAt, etc.)
- Used for POST operations

### Update DTOs (DTO → Entity)
- Include only updatable properties
- All properties are optional (nullable)
- Used for PUT/PATCH operations

---

## Notes

- All mappings are configured in `AutoMapperProfile.cs`
- AutoMapper is registered in `Program.cs` during startup
- Mappings are automatically applied when using `_mapper.Map<TDestination>(TSource)`
- For reverse mappings, AutoMapper handles them automatically if property names match
- Computed properties require eager loading of related entities using `.Include()`

