using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;
using AIBookingSystem.Models;

using Moq;
using System.Reflection;
using System.Net.Cache;

namespace WebAPI.Tests;

public class RoomServiceUnitTests
{
    private readonly RoomService _roomService;
    private readonly Mock<IRoomRepository> _mockRoomRepo;

    public RoomServiceUnitTests()
    {
        _mockRoomRepo = new Mock<IRoomRepository>();
        _roomService = new RoomService(_mockRoomRepo.Object);
    }

    [Fact]
    public void MapRoom2DTO_ValidRoom_ReturnRoomDTO()
    {
        int id = 1;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
        EquipmentDTO equipment = new EquipmentDTO{
           Name = "Telephone",
           RoomId = id
        };

        var room = new Room
                    {
                        Id = id,
                        Name = name,
                        Floor = floor,
                        Capacity = capacity,
                        Description = description
                    };
        room.Equipments = [new Equipment{
                            Name = "Telephone",
                            RoomId = room.Id
                            }];

        var roomDTO = new RoomDTO
                        {
                            Id = id,
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };

        roomDTO.Equipments = [equipment];   

        var result = _roomService.MapRoom2DTO(room);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
        Assert.Equal(room.Equipments.Count, result.Equipments.Count);
        if (room.Equipments.Count>0)
        {
            Assert.Equal(room.Equipments.First().Name, result.Equipments.First().Name);
        }
    }

    [Fact]
    public void MapRoom2DTO_NullRoom_ReturnNull()
    {

        var result = _roomService.MapRoom2DTO((Room) null);
        Assert.Null(result);
    }

    [Fact]
    public void MapRoom2DTO_ValidRoomWithNullEquipments_ReturnRoomDTO()
    {
        int id = 1;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

        var room = new Room
                    {
                        Id = id,
                        Name = name,
                        Floor = floor,
                        Capacity = capacity,
                        Description = description
                    };
        room.Equipments = null;

        var roomDTO = new RoomDTO
                        {
                            Id = id,
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };

        roomDTO.Equipments = [];   

        var result = _roomService.MapRoom2DTO(room);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
        Assert.Equal(0, result.Equipments.Count);
    }

    [Fact]
    public void GetRoombyID_ValidId_ReturnRoomDTO()
    {
        int id = 1;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

        var room = new Room
                        {
                            Id = id,
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };

        room.Equipments = [new Equipment{
           Name = "Telephone",
           RoomId = room.Id
        }]; 

        _mockRoomRepo.Setup(repo => repo.GetRoombyID(id))
                    .Returns(room);

        var result = _roomService.GetRoombyID(id);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
    }

    [Fact]
    public void GetRoombyID_InvalidId_ReturnNull()
    {
        int id = -1;

        var result = _roomService.GetRoombyID(id);
        Assert.Null(result);
    }

    [Fact]
    public void GetRoombyID_IdIsZero_ReturnNull()
    {
        int id = 0;

        var result = _roomService.GetRoombyID(id);
        Assert.Null(result);
    }

    [Fact]
    public void GetRoombyID_WhenRepoReturnNull_ReturnNull()
    {
        int id = 1;

        _mockRoomRepo.Setup(repo => repo.GetRoombyID(id))
                    .Returns((Room)null);

        var result = _roomService.GetRoombyID(id);
        Assert.Null(result);

    }

    [Fact]
    public void CreateRoom_ValidRoomCreateDTO_ReturnRoomDTO()
    {
        
        int id = 3;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";
        Equipment equipment = new Equipment{
                            Name = "Telephone",
                            RoomId = id
                            };

        var roomCreateDTO = new RoomCreateDTO
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        roomCreateDTO.Equipments = [new EquipmentDTO{
           Name = "Telephone"
        }];
        
        var room = new Room
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        room.Equipments = [equipment];

        _mockRoomRepo.Setup(repo => repo.CreateRoom(It.Is<Room>(r =>
                    r.Name == name &&
                    r.Floor == floor &&
                    r.Capacity == capacity &&
                    r.Description == description &&
                    r.Equipments != null &&
                    r.Equipments.Any(e => e.Name == "Telephone")
                    )))
                    .Returns(room);
        var result = _roomService.CreateRoom(roomCreateDTO);
        _mockRoomRepo.VerifyAll();
        Assert.NotNull(result);
        Assert.IsType<RoomDTO>(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
        Assert.Equal(room.Equipments.Count, result.Equipments.Count);
        if (room.Equipments.Count>0)
        {
            Assert.Equal(room.Equipments.First().Name, result.Equipments.First().Name);
        }
    }

    [Fact]
    public void CreateRoom_NullRoomCreateDTO_ReturnNull()
    {
        var result = _roomService.CreateRoom((RoomCreateDTO) null);

        Assert.Null(result);
    }

    [Fact]
    public void CreateRoom_ValidRoomCreateDTOwithEmptyEquipments_ReturnRoomDTO()
    {
        int id = 3;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

        var roomCreateDTO = new RoomCreateDTO
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        roomCreateDTO.Equipments = [];
        
        var room = new Room
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        room.Equipments = [];   

        _mockRoomRepo.Setup(repo => repo.CreateRoom(It.Is<Room>(r =>
                    r.Name == name &&
                    r.Floor == floor &&
                    r.Capacity == capacity &&
                    r.Description == description
                    )))
                    .Returns(room);
        var result = _roomService.CreateRoom(roomCreateDTO);
        _mockRoomRepo.VerifyAll();
        Assert.NotNull(result);
        Assert.IsType<RoomDTO>(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
        Assert.Equal(0, result.Equipments.Count);
    }

    [Fact]
    public void CreateRoom_ValidRoomCreateDTOwithNullEquipments_ReturnRoomDTO()
    {
        int id = 3;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

        var roomCreateDTO = new RoomCreateDTO
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        roomCreateDTO.Equipments = null;
        
        var room = new Room
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        room.Equipments = [];   

        _mockRoomRepo.Setup(repo => repo.CreateRoom(It.IsAny<Room>()))
                    .Returns(room);
        var result = _roomService.CreateRoom(roomCreateDTO);
        _mockRoomRepo.VerifyAll();
        Assert.NotNull(result);
        Assert.IsType<RoomDTO>(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
    }

    [Fact]
    public void CreateRoom_WhenRepoReturnNull_ReturnNull()
    {
        int id = 3;
        string name = "Paris";
        int floor = 5;
        int capacity = 6;
        string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

        var roomCreateDTO = new RoomCreateDTO
                        {
                            Name = name,
                            Floor = floor,
                            Capacity = capacity,
                            Description = description
                        };
        roomCreateDTO.Equipments = [];
         
        _mockRoomRepo.Setup(repo => repo.CreateRoom(It.IsAny<Room>()))
                    .Returns((Room)null);
        var result = _roomService.CreateRoom(roomCreateDTO);
        Assert.Null(result);
    }
}