using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;
using NodaTime;
using Microsoft.AspNetCore.Authorization;

namespace AIBookingSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoomDTO>> ListRooms()
        {
            var rooms = _roomService.ListRooms();
            if (rooms == null || rooms.Count() == 0)
            {
                string message = "No room is found.";
                _logger.LogError(message);
                return NotFound(message);
            }

            return Ok(rooms);
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoomDTO>> GetRoombyID(int id)
        {
            string message;
            if (id > 0) 
            {
                var room = _roomService.GetRoombyID(id);
                if (room == null)
                {
                    message = $"Room with ID, {id}, not found.";
                    _logger.LogError(message);
                    return NotFound(message);

                }
                return Ok(room);
            }

            message = "Please provide valid Id for getting a room.";
            _logger.LogError(message);
            return NotFound(message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<RoomDTO> CreateRoom([FromBody] RoomCreateDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 
            
            string message="";

            if (createDto.Floor < 0)
            {
                message = "Please provide a valid location/floor.";
            }
            else
            {
                if (createDto.Capacity <= 0)
                {
                    message = "Capacity must be bigger than 0.";
                }
                else 
                {
                    if (createDto.Name == ""){
                        message = "Please provide a name of the room.";
                    }
                    else 
                    {
                        if (createDto.Description == "")
                        {
                            message = "Please provide description of the room.";
                        }
                        else
                        {
                            var newRoom = _roomService.CreateRoom(createDto);
                            if (newRoom == null)
                            {
                                message = "Room is not created successfully.";
                            }
                            else {

                                return CreatedAtAction(nameof(GetRoombyID), new { id = newRoom.Id }, newRoom);   
                                
                            }
                        }
                    }
                }
            }
            _logger.LogError(message);        
            return BadRequest (message);
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<RoomDTO>> FindAvailableRoomsbyDateTime(DateTimeOffset from, DateTimeOffset to)
        {
            string message = "";

            if (from > to)
            {
                message = "To date must be later than From date.";
            }
            else
            {
                if (from < DateTimeOffset.UtcNow)
                {
                    message = "You can only book rooms for future.";
                }
                else 
                {
                    if (from.Date != to.Date)
                    {
                        message = "You cannot book room across 2 days. Please split the booking into 2.";
                    }
                    else 
                    {
                        var rooms = _roomService.FindAvailableRoomsbyDateTime(from, to);
                        if (rooms == null || rooms.Count()== 0)
                        {
                            message = "No available room is found.";
                            _logger.LogError(message);
                            return NotFound(message);
                        }

                        return Ok(rooms);
                    }
                }
            }
            return BadRequest(message);
        }
    }
}