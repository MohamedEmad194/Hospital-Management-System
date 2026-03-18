using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class NursingUnitService : INursingUnitService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public NursingUnitService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NursingUnitDto>> GetAllNursingUnitsAsync()
        {
            var units = await _context.NursingUnits
                .Where(n => !n.IsDeleted && n.IsActive)
                .OrderBy(n => n.Unit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NursingUnitDto>>(units);
        }

        public async Task<NursingUnitDto?> GetNursingUnitByIdAsync(int id)
        {
            var unit = await _context.NursingUnits
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            if (unit == null)
                return null;

            return _mapper.Map<NursingUnitDto>(unit);
        }

        public async Task<NursingUnitDto?> GetNursingUnitByUnitIdAsync(string unitId)
        {
            var unit = await _context.NursingUnits
                .FirstOrDefaultAsync(n => n.UnitId == unitId && !n.IsDeleted);

            if (unit == null)
                return null;

            return _mapper.Map<NursingUnitDto>(unit);
        }

        public async Task<IEnumerable<NursingUnitDto>> SearchNursingUnitsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllNursingUnitsAsync();

            var normalized = searchTerm.Trim().ToLower();
            var units = await _context.NursingUnits
                .Where(n => !n.IsDeleted && n.IsActive &&
                    (n.Unit.ToLower().Contains(normalized) ||
                     (n.Wing != null && n.Wing.ToLower().Contains(normalized)) ||
                     (n.Lead != null && n.Lead.ToLower().Contains(normalized)) ||
                     (n.Focus != null && n.Focus.ToLower().Contains(normalized)) ||
                     (n.UnitId != null && n.UnitId.ToLower().Contains(normalized)) ||
                     (n.Coverage != null && n.Coverage.ToLower().Contains(normalized)) ||
                     (n.Ratio != null && n.Ratio.ToLower().Contains(normalized))))
                .OrderBy(n => n.Unit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NursingUnitDto>>(units);
        }

        public async Task<NursingUnitDto> CreateNursingUnitAsync(CreateNursingUnitDto createDto)
        {
            var unit = _mapper.Map<NursingUnit>(createDto);
            unit.CreatedAt = DateTime.UtcNow;

            _context.NursingUnits.Add(unit);
            await _context.SaveChangesAsync();

            return _mapper.Map<NursingUnitDto>(unit);
        }

        public async Task<NursingUnitDto?> UpdateNursingUnitAsync(int id, UpdateNursingUnitDto updateDto)
        {
            var unit = await _context.NursingUnits
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            if (unit == null)
                return null;

            _mapper.Map(updateDto, unit);
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<NursingUnitDto>(unit);
        }

        public async Task<bool> DeleteNursingUnitAsync(int id)
        {
            var unit = await _context.NursingUnits
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            if (unit == null)
                return false;

            unit.IsDeleted = true;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NursingUnitExistsAsync(int id)
        {
            return await _context.NursingUnits
                .AnyAsync(n => n.Id == id && !n.IsDeleted);
        }

        public async Task<bool> SeedNursingUnitsAsync()
        {
            // Check if data already exists
            if (await _context.NursingUnits.AnyAsync())
                return false;

            // Dataset from CSV file - Nursing Units Data
            var units = new List<NursingUnit>
            {
                new NursingUnit 
                { 
                    UnitId = "ICU-01", 
                    Unit = "Intensive Care Unit", 
                    Wing = "North Tower, Level 5", 
                    Lead = "Sarah Youssef", 
                    Nurses = 18, 
                    Coverage = "24/7", 
                    Ratio = "1:2", 
                    Focus = "Ventilators; Sepsis bundles; Post-op cardiac", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "CCU-02", 
                    Unit = "Cardiac Care", 
                    Wing = "North Tower, Level 4", 
                    Lead = "Ahmed Kamal", 
                    Nurses = 14, 
                    Coverage = "24/7", 
                    Ratio = "1:3", 
                    Focus = "Post-CABG; Arrhythmia monitoring; Cath lab recovery", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "ER-03", 
                    Unit = "Emergency Response", 
                    Wing = "Ground Pavilion A", 
                    Lead = "Mona Helmy", 
                    Nurses = 26, 
                    Coverage = "24/7", 
                    Ratio = "Team-based", 
                    Focus = "Trauma; Triage; Code blue management", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "PED-04", 
                    Unit = "Pediatrics", 
                    Wing = "Family Center, Level 2", 
                    Lead = "Rania Saad", 
                    Nurses = 16, 
                    Coverage = "24/7", 
                    Ratio = "1:4", 
                    Focus = "Growth tracking; Pain distraction; Parental coaching", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "NICU-05", 
                    Unit = "Neonatal ICU", 
                    Wing = "Family Center, Level 3", 
                    Lead = "Hassan Emad", 
                    Nurses = 20, 
                    Coverage = "24/7", 
                    Ratio = "1:1-2", 
                    Focus = "Premature infants; Incubator care; Kangaroo programs", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "SURG-06", 
                    Unit = "Post-Surgical Recovery", 
                    Wing = "South Tower, Level 6", 
                    Lead = "Yara Hamed", 
                    Nurses = 22, 
                    Coverage = "24/7", 
                    Ratio = "1:3", 
                    Focus = "Pain control; Wound vacs; Early mobility", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "MED-07", 
                    Unit = "Medical Ward", 
                    Wing = "South Tower, Level 4", 
                    Lead = "Karim Talaat", 
                    Nurses = 24, 
                    Coverage = "24/7", 
                    Ratio = "1:5", 
                    Focus = "Chronic diseases; Diabetic education; IV therapy", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "ONC-08", 
                    Unit = "Oncology Day Care", 
                    Wing = "Cancer Center, Level 1", 
                    Lead = "Lina Fathi", 
                    Nurses = 12, 
                    Coverage = "06:00-00:00", 
                    Ratio = "1:3", 
                    Focus = "Chemo administration; Port care; Symptom coaching", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "HOME-09", 
                    Unit = "Home Care & Tele-Nursing", 
                    Wing = "Virtual Command", 
                    Lead = "Omar Safwat", 
                    Nurses = 15, 
                    Coverage = "07:00-23:00", 
                    Ratio = "1:15", 
                    Focus = "Remote vitals; Discharge follow-up; Medication reconciliation", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "REHAB-10", 
                    Unit = "Rehabilitation Support", 
                    Wing = "Wellness Pavilion, Level 2", 
                    Lead = "Dina Ghandour", 
                    Nurses = 10, 
                    Coverage = "06:00-22:00", 
                    Ratio = "1:4", 
                    Focus = "Neuro recovery; Mobility labs; Caregiver training", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "MAT-11", 
                    Unit = "Maternity & Postpartum", 
                    Wing = "Family Center, Level 1", 
                    Lead = "Heba Abdelrahman", 
                    Nurses = 18, 
                    Coverage = "24/7", 
                    Ratio = "1:3", 
                    Focus = "Lactation support; Newborn safety; Recovery coaching", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                },
                new NursingUnit 
                { 
                    UnitId = "EDU-12", 
                    Unit = "Clinical Education", 
                    Wing = "Knowledge Hub", 
                    Lead = "Nader Fouad", 
                    Nurses = 8, 
                    Coverage = "08:00-18:00", 
                    Ratio = "1:25 trainees", 
                    Focus = "Simulation labs; Competency exams; Research audits", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow 
                }
            };

            _context.NursingUnits.AddRange(units);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

