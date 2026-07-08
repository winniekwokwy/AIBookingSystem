using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;
using AIBookingSystem.Models;

using Moq;
using System.Reflection;
using System.Net.Cache;

namespace WebAPI.Tests.Services

{
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
            Assert.NotNull(result.Equipments);
            Assert.Equal(room.Equipments.Count, result.Equipments.Count);
            if (room.Equipments.Count>0)
            {
                Assert.Equal(room.Equipments.First().Name, result.Equipments.First().Name);
            }
        }

        [Fact]
        public void MapRoom2DTO_NullRoom_ReturnNull()
        {

            var result = _roomService.MapRoom2DTO((Room) null!);
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
            room.Equipments = null!;

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
            Assert.NotNull(result.Equipments);
            Assert.Empty(result.Equipments);
        }

        [Fact]
        public void ListRooms_FoundRooms_ReturnListOfRoomDTO()
        {
            List<Room> rooms = new List<Room>
            {
                new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."},
                new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."}
            };
            _mockRoomRepo.Setup(r => r.ListRooms())
                        .Returns(rooms);

            var result = _roomService.ListRooms();
            Assert.NotNull(result);
            Assert.Equal(rooms.Count, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(rooms.First().Name, result.First().Name);
                Assert.Equal(rooms.First().Floor, result.First().Floor);
                Assert.Equal(rooms.First().Capacity, result.First().Capacity);
                Assert.Equal(rooms.First().Description, result.First().Description);
            }       
        }

        [Fact]
        public void ListRooms_NoRooms_ReturnListOfRoomDTO()
        {
            _mockRoomRepo.Setup(r => r.ListRooms())
                        .Returns((List<Room>?)null);

            var result = _roomService.ListRooms();
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
                        .Returns((Room?)null);

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
            Assert.NotNull(result.Equipments);
            Assert.Equal(room.Equipments.Count, result.Equipments.Count);
            if (room.Equipments.Count>0)
            {
                Assert.Equal(room.Equipments.First().Name, result.Equipments.First().Name);
            }

        }

        [Fact]
        public void CreateRoom_NullRoomCreateDTO_ReturnNull()
        {
            var result = _roomService.CreateRoom((RoomCreateDTO) null!);

            Assert.Null(result);
        }

        [Fact]
        public void CreateRoom_ValidRoomCreateDTOwithEmptyEquipments_ReturnRoomDTO()
        {
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
            if (result.Equipments!= null){
                Assert.Empty(result.Equipments);
            }
        }

        [Fact]
        public void CreateRoom_ValidRoomCreateDTOwithNullEquipments_ReturnRoomDTO()
        {
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
            roomCreateDTO.Equipments = null!;
            
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
                        .Returns((Room?)null);
            var result = _roomService.CreateRoom(roomCreateDTO);
            Assert.Null(result);
        }
        
        [Fact]
        public void FindAvailableRoomsbyDateTime_ValidPeriod_ReturnListofRooms()
        {
            Room room1 = new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."};
            Room room2 = new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."};

            List<Room> rooms = new List<Room> {room1, room2};
                
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
            _mockRoomRepo.Setup(repo => repo.FindAvailableRoomsbyDateTime(from, to))
                            .Returns(rooms);
            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.NotNull(result);
            Assert.Equal(rooms.Count, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(room1.Name, result.First().Name);
                Assert.Equal(room1.Floor, result.First().Floor);
                Assert.Equal(room1.Capacity, result.First().Capacity);
                Assert.Equal(room1.Description, result.First().Description);
            }            
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_InvalidPeriod_ReturnNull()
        {                
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.Null(result);        
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_PeriodInThePast_ReturnNull()
        {                
            DateTimeOffset from = new DateTimeOffset(2025, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2025, 12, 20, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.Null(result);        
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_PeriodNotOnTheSameDay_ReturnNull()
        {    
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 21, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.Null(result);          
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_WhenRepoReturnNull_ReturnNull()
        {    
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
            _mockRoomRepo.Setup(repo => repo.FindAvailableRoomsbyDateTime(from, to))
                            .Returns((List<Room>?)null);
            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.Null(result);          
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_WhenRepoReturnEmpty_ReturnEmptyList()
        {    
            List<Room> rooms = [];
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
            _mockRoomRepo.Setup(repo => repo.FindAvailableRoomsbyDateTime(from, to))
                            .Returns(rooms);
            var result = _roomService.FindAvailableRoomsbyDateTime(from, to);
            
            Assert.NotNull(result);
            Assert.Empty(result);          
        }

        [Fact]
        public void IsRoomAvailable_RoomAvailableWithValidInput_ReturnTrue()
        {
            int roomId = 1;
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
            _mockRoomRepo.Setup(repo => repo.IsRoomAvailable(roomId, from, to))
                            .Returns(true);
            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.True(result);         
        }

        [Fact]
        public void IsRoomAvailable_RoomNotAvailableWithValidInput_ReturnFalse()
        {
            int roomId = 1;
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
            _mockRoomRepo.Setup(repo => repo.IsRoomAvailable(roomId, from, to))
                            .Returns(false);
            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.False(result);         
        }

        [Fact]
        public void IsRoomAvailable_InvalidPeriod_ReturnFalse()
        {
            int roomId = 1;
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.False(result);         
        }

        [Fact]
        public void IsRoomAvailable_PeriodInThePast_ReturnFalse()
        {
            int roomId = 1;
            DateTimeOffset from = new DateTimeOffset(2025, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2025, 12, 20, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.False(result);         
        }

        [Fact]
        public void IsRoomAvailable_FromNToNotOnTheSameDay_ReturnFalse()
        {
            int roomId = 1;
            DateTimeOffset from = new DateTimeOffset(2025, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2025, 12, 21, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.False(result);         
        }

        [Fact]
        public void IsRoomAvailable_InvalidRoomId_ReturnFalse()
        {
            int roomId = -1;
            DateTimeOffset from = new DateTimeOffset(2025, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2025, 12, 21, 15, 30, 0, TimeSpan.Zero);

            var result = _roomService.IsRoomAvailable(roomId, from, to);
            
            Assert.False(result);         
        }
    }
}