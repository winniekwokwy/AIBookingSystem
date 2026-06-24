using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Tests.Controllers;

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
        Assert.Equal(roomDTO, okResult.Value);  
    }

    [Fact]
    public void GetRoombyID_InvalidId_ReturnNull()
    {
        int id = -1;

        _mockRoomService.Setup(s => s.GetRoombyID(id))
                        .Returns((RoomDTO) null);
        var result = _roomController.GetRoombyID(id);

        Assert.IsType<NotFoundObjectResult>(result.Result); 
    }

    [Fact]
    public void GetRoombyID_NonExistingId_ReturnNull()
    {
        int id = 999;

        _mockRoomService.Setup(s => s.GetRoombyID(id))
                        .Returns((RoomDTO) null);
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
        
        roomCreateDTO.Equipments.Add(equipmentDTO);

        var roomDTO = new RoomDTO
                        {
                            Id = id,
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description,
                        };

        roomDTO.Equipments.Add(equipmentDTO);

        _mockRoomService.Setup(s => s.CreateRoom(roomCreateDTO))
                        .Returns(roomDTO);
        var result = _roomController.CreateRoom(roomCreateDTO);
        Assert.IsType<CreatedAtActionResult>(result.Result); 
        Assert.Equal(roomDTO, (result.Result as CreatedAtActionResult).Value);
    }

    [Fact]
    public void CreateRoom_NullRoomCreateDTO_ReturnBadRequest()
    {
        var expected = "The RoomCreateDTO is null.";
        _mockRoomService.Setup(s => s.CreateRoom((RoomCreateDTO) null))
                        .Returns((RoomDTO) null);
        var result = _roomController.CreateRoom((RoomCreateDTO) null);
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
        string name = null;
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
        string description = null;
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
                        .Returns((RoomDTO) null);
        var result = _roomController.CreateRoom(roomCreateDTO);
        var badRequstResult = result.Result as BadRequestObjectResult;
        Assert.IsType<BadRequestObjectResult>(badRequstResult);
        Assert.Equal(expected, badRequstResult.Value);
    }
}