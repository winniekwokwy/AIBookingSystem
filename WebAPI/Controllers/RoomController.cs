using Microsoft.AspNetCore.Mvc;
using AIBookingSystem.DTO;
using AIBookingSystem.Services;

namespace AIBookingSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
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
            if (rooms == null)
            {
                string message = "No room is found.";
                _logger.LogError(message);
                return NotFound(new { Message = message });
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
                    return NotFound(new { Message = message });

                }
                return Ok(room);
            }

            message = "Please provide valid Id for getting a room.";
            _logger.LogError(message);
            return NotFound(new { Message = message });
        }

        [HttpPost]
        public ActionResult<RoomDTO> CreateRoom([FromBody] RoomCreateDTO createDto)
        {
            string message="";
            if (createDto != null)
            {
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
                        if (createDto.Name == null || createDto.Name == ""){
                            message = "Please provide a name of the room.";
                        }
                        else 
                        {
                            if (createDto.Description == null || createDto.Description == "")
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
            }
            else 
            {
                message = "The RoomCreateDTO is null.";
            }
            _logger.LogError(message);        
            return BadRequest (message);
        }
    }
}