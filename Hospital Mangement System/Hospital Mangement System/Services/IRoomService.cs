using Hospital_Management_System.DTOs;

namespace Hospital_Management_System.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<IEnumerable<RoomDto>> GetRoomsByDepartmentAsync(int departmentId);
        Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync();
        Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
        Task<RoomDto?> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> RoomExistsAsync(int id);
    }
}
