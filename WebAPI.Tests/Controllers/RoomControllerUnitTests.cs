using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Xunit.v3;

namespace WebAPI.Tests.Controllers

{
    public class RoomControllerUnitTests
    {
        private readonly RoomController _roomController;
        private readonly Mock<IRoomService> _mockRoomService;
        private readonly Mock<ILogger<RoomController>> _mockLogger;

        public RoomControllerUnitTests()
        {
            _mockRoomService = new Mock<IRoomService>();
            _mockLogger = new Mock<ILogger<RoomController>>();
            _roomController = new RoomController(_mockRoomService.Object, _mockLogger.Object);
        }

        [Fact]
        public void ListRooms_FoundRooms_ReturnListOfRoomDTO()
        {
            List<RoomDTO> rooms = new List<RoomDTO>
            {
                new RoomDTO() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."},
                new RoomDTO() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."}
            };
            _mockRoomService.Setup(r => r.ListRooms())
                        .Returns(rooms);

            var result = _roomController.ListRooms();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDTO>>(okResult.Value);
            Assert.Equal(rooms.Count, returnedRooms.Count());
            if (returnedRooms.Count()>0)
            {
                Assert.Equal(rooms.First().Name, returnedRooms.First().Name);
                Assert.Equal(rooms.First().Floor, returnedRooms.First().Floor);
                Assert.Equal(rooms.First().Capacity, returnedRooms.First().Capacity);
                Assert.Equal(rooms.First().Description, returnedRooms.First().Description);
            }
        }

            [Fact]
        public void ListRooms_NoRoom_ReturnListOfRoomDTO()
        {
            _mockRoomService.Setup(r => r.ListRooms())
                        .Returns((List<RoomDTO>?)null);

            var result = _roomController.ListRooms();

        Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetRoombyID_ValidId_ReturnRoomDTO()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var roomDTO = new RoomDTO
                            {
                                Id = id,
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };

            roomDTO.Equipments = [new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            }];

            _mockRoomService.Setup(s => s.GetRoombyID(id))
                            .Returns(roomDTO);
            var result = _roomController.GetRoombyID(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRoom = Assert.IsType<RoomDTO>(okResult.Value);
            Assert.Equal(roomDTO.Name, ((RoomDTO)returnedRoom).Name);
            Assert.Equal(roomDTO.Floor, ((RoomDTO)returnedRoom).Floor);
            Assert.Equal(roomDTO.Capacity, ((RoomDTO)returnedRoom).Capacity);
            Assert.Equal(roomDTO.Description, ((RoomDTO)returnedRoom).Description);
        }

        [Fact]
        public void GetRoombyID_InvalidId_ReturnNotFound()
        {
            int id = -1;

            var result = _roomController.GetRoombyID(id);

            Assert.IsType<NotFoundObjectResult>(result.Result); 
        }

        [Fact]
        public void GetRoombyID_NonExistingId_ReturnNotFound()
        {
            int id = 999;

            _mockRoomService.Setup(s => s.GetRoombyID(id))
                            .Returns((RoomDTO?) null);
            var result = _roomController.GetRoombyID(id);

            Assert.IsType<NotFoundObjectResult>(result.Result); 
        }    

        [Fact]
        public void GetRoombyID_IdIsZero_ReturnNotFound()
        {
            int id = 0;

            var result = _roomController.GetRoombyID(id);

            Assert.IsType<NotFoundObjectResult>(result.Result); 
        }   

        [Fact]
        public void CreateRoom_ValidRoomCreateDTO_ReturnRoomDTO()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments = [equipmentDTO];

            var roomDTO = new RoomDTO
                            {
                                Id = id,
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };

            roomDTO.Equipments = [equipmentDTO];

            _mockRoomService.Setup(s => s.CreateRoom(roomCreateDTO))
                            .Returns(roomDTO);
            var result = _roomController.CreateRoom(roomCreateDTO);
            var returnedResult = Assert.IsType<CreatedAtActionResult>(result.Result); 
            Assert.NotNull(returnedResult);
            var returnedDTO = (returnedResult as CreatedAtActionResult).Value as RoomDTO;
            Assert.NotNull(returnedDTO);
            Assert.Equal(roomDTO.Name, returnedDTO.Name);
            Assert.Equal(roomDTO.Floor, returnedDTO.Floor);
            Assert.Equal(roomDTO.Capacity, returnedDTO.Capacity);
            Assert.Equal(roomDTO.Description, returnedDTO.Description);
            Assert.NotNull(returnedDTO.Equipments);
            Assert.Equal(roomDTO.Equipments.Count, returnedDTO.Equipments.Count);
            if (roomDTO.Equipments.Count>0)
            {
                Assert.Equal(roomDTO.Equipments.First().Name, returnedDTO.Equipments.First().Name);
            }
        }

        [Fact]
        public void CreateRoom_NullRoomCreateDTO_ReturnBadRequest()
        {
            var expected = "The RoomCreateDTO is null.";

            var result = _roomController.CreateRoom((RoomCreateDTO) null!);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_InvalidFloor_ReturnBadRequest()
        {
            int id = 1;
            string name = "Paris";
            int floor = -1;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Please provide a valid location/floor.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_InvalidCapacity_ReturnBadRequest()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = -1;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Capacity must be bigger than 0.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_EmptyName_ReturnBadRequest()
        {
            int id = 1;
            string name = "";
            int floor = 5;
            int capacity = 10;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Please provide a name of the room.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_NullName_ReturnBadRequest()
        {
            int id = 1;
            string name = null!;
            int floor = 5;
            int capacity = 10;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Please provide a name of the room.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_EmptyDescription_ReturnBadRequest()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 10;
            string description = "";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Please provide description of the room.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_NullDescription_ReturnBadRequest()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 10;
            string description = null!;
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var expected = "Please provide description of the room.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void CreateRoom_NullEquipments_ReturnRoomDTO()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments = null!;

            var roomDTO = new RoomDTO
                            {
                                Id = id,
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };

            roomDTO.Equipments = [];

            _mockRoomService.Setup(s => s.CreateRoom(roomCreateDTO))
                            .Returns(roomDTO);
            var result = _roomController.CreateRoom(roomCreateDTO);

            var returnedResult = Assert.IsType<CreatedAtActionResult>(result.Result); 
            var returnedDTO = Assert.IsType<RoomDTO>(returnedResult.Value);
            Assert.Equal(roomDTO.Name, returnedDTO.Name);
            Assert.Equal(roomDTO.Floor, returnedDTO.Floor);
            Assert.Equal(roomDTO.Capacity, returnedDTO.Capacity);
            Assert.Equal(roomDTO.Description, returnedDTO.Description);
            Assert.NotNull(returnedDTO.Equipments);
            Assert.Equal(roomDTO.Equipments.Count, returnedDTO.Equipments.Count);
        }

        [Fact]
        public void CreateRoom_RoomCreateionFailed_ReturnBadRequest()
        {
            int id = 1;
            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
            EquipmentDTO equipmentDTO = new EquipmentDTO{
            Name = "Telephone",
            RoomId = id
            };

            var roomCreateDTO = new RoomCreateDTO
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description,
                            };
            
            roomCreateDTO.Equipments.Add(equipmentDTO);

            var expected = "Room is not created successfully.";
            _mockRoomService.Setup(s => s.CreateRoom(roomCreateDTO))
                            .Returns((RoomDTO?) null);
            var result = _roomController.CreateRoom(roomCreateDTO);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }
    }
}