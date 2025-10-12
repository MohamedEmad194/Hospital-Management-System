using Microsoft.EntityFrameworkCore;
using Hospital_Management_System.Data;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Models;
using AutoMapper;

namespace Hospital_Management_System.Services
{
    public class RoomService : IRoomService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public RoomService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Department)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            return room == null ? null : _mapper.Map<RoomDto>(room);
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsByDepartmentAsync(int departmentId)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => r.DepartmentId == departmentId && !r.IsDeleted)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted && r.IsAvailable && r.IsActive)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            // Verify department exists
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == createRoomDto.DepartmentId && d.IsActive);

            if (department == null)
                throw new InvalidOperationException("Department not found or inactive.");

            // Check if room number already exists
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomNumber == createRoomDto.RoomNumber && !r.IsDeleted);

            if (existingRoom != null)
                throw new InvalidOperationException("Room with this number already exists.");

            var room = _mapper.Map<Room>(createRoomDto);
            room.CreatedAt = DateTime.UtcNow;

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            // Load department for response
            await _context.Entry(room)
                .Reference(r => r.Department)
                .LoadAsync();

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<RoomDto?> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto)
        {
            var room = await _context.Rooms
                .Include(r => r.Department)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (room == null)
                return null;

            // Check room number uniqueness if room number is being updated
            if (!string.IsNullOrEmpty(updateRoomDto.RoomNumber) && 
                updateRoomDto.RoomNumber != room.RoomNumber)
            {
                var existingRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.RoomNumber == updateRoomDto.RoomNumber && !r.IsDeleted);

                if (existingRoom != null)
                    throw new InvalidOperationException("Room with this number already exists.");
            }

            // Verify department exists if department is being updated
            if (updateRoomDto.DepartmentId.HasValue)
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == updateRoomDto.DepartmentId.Value && d.IsActive);

                if (department == null)
                    throw new InvalidOperationException("Department not found or inactive.");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateRoomDto.RoomNumber))
                room.RoomNumber = updateRoomDto.RoomNumber;

            if (!string.IsNullOrEmpty(updateRoomDto.RoomType))
                room.RoomType = updateRoomDto.RoomType;

            if (updateRoomDto.Floor != null)
                room.Floor = updateRoomDto.Floor;

            if (updateRoomDto.Building != null)
                room.Building = updateRoomDto.Building;

            if (updateRoomDto.Description != null)
                room.Description = updateRoomDto.Description;

            if (updateRoomDto.Capacity.HasValue)
                room.Capacity = updateRoomDto.Capacity.Value;

            if (updateRoomDto.HourlyRate.HasValue)
                room.HourlyRate = updateRoomDto.HourlyRate.Value;

            if (updateRoomDto.IsAvailable.HasValue)
                room.IsAvailable = updateRoomDto.IsAvailable.Value;

            if (updateRoomDto.IsActive.HasValue)
                room.IsActive = updateRoomDto.IsActive.Value;

            if (updateRoomDto.DepartmentId.HasValue)
                room.DepartmentId = updateRoomDto.DepartmentId.Value;

            room.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<RoomDto>(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (room == null)
                return false;

            room.IsDeleted = true;
            room.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoomExistsAsync(int id)
        {
            return await _context.Rooms
                .AnyAsync(r => r.Id == id && !r.IsDeleted);
        }
    }
}
