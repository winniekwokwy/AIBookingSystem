using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;
using AIBookingSystem.Models;

using Moq;
using System.Reflection;

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

        roomDTO.Equipments = [new EquipmentDTO{
           Name = "Telephone",
           RoomId = id
        }];   

        var result = _roomService.MapRoom2DTO(room);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(floor, result.Floor);
        Assert.Equal(capacity, result.Capacity);
        Assert.Equal(description, result.Description);
    }

    [Fact]
    public void MapRoom2DTO_NullRoom_ReturnNull()
    {

        var result = _roomService.MapRoom2DTO((Room) null);
        Assert.Null(result);
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
}