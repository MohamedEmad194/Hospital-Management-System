using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms");
                return StatusCode(500, "An error occurred while retrieving rooms");
            }
        }

        /// <summary>
        /// Get room by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                    return NotFound($"Room with ID {id} not found");

                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room with ID {RoomId}", id);
                return StatusCode(500, "An error occurred while retrieving the room");
            }
        }

        /// <summary>
        /// Get rooms by department
        /// </summary>
        [HttpGet("department/{departmentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByDepartment(int departmentId)
        {
            try
            {
                var rooms = await _roomService.GetRoomsByDepartmentAsync(departmentId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms for department {DepartmentId}", departmentId);
                return StatusCode(500, "An error occurred while retrieving rooms");
            }
        }

        /// <summary>
        /// Get available rooms
        /// </summary>
        [HttpGet("available")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms()
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available rooms");
                return StatusCode(500, "An error occurred while retrieving available rooms");
            }
        }

        /// <summary>
        /// Create a new room
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto createRoomDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = await _roomService.CreateRoomAsync(createRoomDto);
                return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(500, "An error occurred while creating the room");
            }
        }

        /// <summary>
        /// Update an existing room
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RoomDto>> UpdateRoom(int id, UpdateRoomDto updateRoomDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var room = await _roomService.UpdateRoomAsync(id, updateRoomDto);
                if (room == null)
                    return NotFound($"Room with ID {id} not found");

                return Ok(room);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with ID {RoomId}", id);
                return StatusCode(500, "An error occurred while updating the room");
            }
        }

        /// <summary>
        /// Delete a room
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(id);
                if (!result)
                    return NotFound($"Room with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room with ID {RoomId}", id);
                return StatusCode(500, "An error occurred while deleting the room");
            }
        }

        /// <summary>
        /// Check if room exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> RoomExists(int id)
        {
            try
            {
                var exists = await _roomService.RoomExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if room exists with ID {RoomId}", id);
                return StatusCode(500, "An error occurred while checking room existence");
            }
        }
    }
}
