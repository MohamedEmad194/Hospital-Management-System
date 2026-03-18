using AutoMapper;
using Hospital_Management_System.Models;
using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Patient mappings
            CreateMap<Patient, PatientDto>();
            CreateMap<CreatePatientDto, Patient>();
            CreateMap<UpdatePatientDto, Patient>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Doctor mappings
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));
            CreateMap<CreateDoctorDto, Doctor>();
            CreateMap<UpdateDoctorDto, Doctor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Department mappings
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Appointment mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => 
                    src.Patient != null && !string.IsNullOrEmpty(src.Patient.FirstName) && !string.IsNullOrEmpty(src.Patient.LastName)
                        ? $"{src.Patient.FirstName} {src.Patient.LastName}" 
                        : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => 
                    src.Doctor != null && !string.IsNullOrEmpty(src.Doctor.FirstName) && !string.IsNullOrEmpty(src.Doctor.LastName)
                        ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" 
                        : null))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => 
                    src.Room != null && !string.IsNullOrEmpty(src.Room.RoomNumber)
                        ? src.Room.RoomNumber 
                        : null));
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Medical Record mappings
            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null));
            CreateMap<CreateMedicalRecordDto, MedicalRecord>();
            CreateMap<UpdateMedicalRecordDto, MedicalRecord>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Medicine mappings
            CreateMap<Medicine, MedicineDto>();
            CreateMap<CreateMedicineDto, Medicine>();
            CreateMap<UpdateMedicineDto, Medicine>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Bill mappings
            CreateMap<Bill, BillDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => 
                    src.Patient != null && !string.IsNullOrEmpty(src.Patient.FirstName) && !string.IsNullOrEmpty(src.Patient.LastName)
                        ? $"{src.Patient.FirstName} {src.Patient.LastName}" 
                        : null))
                .ForMember(dest => dest.PatientEmail, opt => opt.MapFrom(src => 
                    src.Patient != null ? src.Patient.Email : null));
            CreateMap<CreateBillDto, Bill>();
            CreateMap<UpdateBillDto, Bill>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<BillItem, BillItemDto>();
            CreateMap<CreateBillItemDto, BillItem>();

            // Room mappings
            CreateMap<Room, RoomDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null));
            CreateMap<CreateRoomDto, Room>();
            CreateMap<UpdateRoomDto, Room>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Schedule mappings
            CreateMap<Schedule, ScheduleDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null));
            CreateMap<CreateScheduleDto, Schedule>();
            CreateMap<UpdateScheduleDto, Schedule>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Prescription mappings
            CreateMap<Prescription, PrescriptionDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null));
            CreateMap<CreatePrescriptionDto, Prescription>();
            CreateMap<UpdatePrescriptionDto, Prescription>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PrescriptionItem, PrescriptionItemDto>()
                .ForMember(dest => dest.MedicineGenericName, opt => opt.MapFrom(src => src.Medicine != null ? src.Medicine.GenericName : null));
            CreateMap<CreatePrescriptionItemDto, PrescriptionItem>();

            // User mappings
            CreateMap<User, UserDto>();

            // NursingUnit mappings
            CreateMap<NursingUnit, NursingUnitDto>();
            CreateMap<CreateNursingUnitDto, NursingUnit>();
            CreateMap<UpdateNursingUnitDto, NursingUnit>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
