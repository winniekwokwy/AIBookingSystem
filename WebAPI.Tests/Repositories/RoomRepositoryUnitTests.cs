using System.Reflection;
using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;

using Microsoft.EntityFrameworkCore;

namespace WebAPI.Tests.Repositories
{
    public class RoomRepositoryUnitTests
    {
        // Helper method that creates a fresh, isolated ApplicationDbContext using EF Core InMemory provider
        private RoomBookingDbContext GetInMemoryDbContext(bool requiredData)
        {
            var options = new DbContextOptionsBuilder<RoomBookingDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new RoomBookingDbContext(options);

            if (requiredData)
            {
                context.Rooms.AddRange(
                    new Room(){ Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."},
                    new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."}
                );

                context.SaveChanges();
            }
            return context;
        }

        [Fact]
        public void ListRooms_FoundRooms_ReturnListOfRoom()
        {
            int id = 1;
            string name = "Mongkok";
            int floor = 2;
            int capacity = 5;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.ListRooms();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(id, result.First().Id);
                Assert.Equal(name, result.First().Name);
                Assert.Equal(floor, result.First().Floor);
                Assert.Equal(capacity, result.First().Capacity);   
                Assert.Equal(description, result.First().Description);
            }
        }

        [Fact]
        public void ListRooms_NoRooms_ReturnEmptyList()
        {

            var context = GetInMemoryDbContext(false);
            var repository = new RoomRepository(context);
            var result = repository.ListRooms();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetRoombyID_ValidId_ReturnRoom()
        {
            int id = 1;
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.GetRoombyID(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Mongkok", result.Name);
            Assert.Equal(2, result.Floor);
            Assert.Equal(5, result.Capacity);
        }

        [Fact]
        public void GetRoombyID_InvalidId_ReturnNull()
        {
            int id = -1;
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.GetRoombyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetRoombyID_NonExistingId_ReturnNull()
        {
            int id = 10;
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.GetRoombyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetRoombyID_IdIsZero_ReturnNull()
        {
            int id = 0;
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.GetRoombyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void CreateRoom_ValidRoomCreateDTO_ReturnRoomDTO()
        {
            string name = "Admiralty";
            int floor = 11;
            int capacity = 6;
            string description = $"This room is located at {floor}/f which can accommodate {capacity} people.";

            var room = new Room
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description
                            };
            room.Equipments = [new Equipment{
                        Name = "Telephone",
                        RoomId = room.Id
                        }];

            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.CreateRoom(room);

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(floor, result.Floor);
            Assert.Equal(capacity, result.Capacity);
            Assert.Equal(description, result.Description);
            Assert.Equal(room.Equipments, result.Equipments);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public void CreateRoom_NullRoomCreateDTO_ReturnNull()
        {

            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.CreateRoom((Room?)null!);

            Assert.Null(result);
        }

        [Fact]
        public void CreateRoom_ValidRoomCreateDTOWithNullEquipments_ReturnRoomDTO()
        {
            string name = "Admiralty";
            int floor = 11;
            int capacity = 6;
            string description = $"This room is located at {floor}/f which can accommodate {capacity} people.";

            var room = new Room
                            {
                                Name = name,
                                Floor = floor,
                                Capacity = capacity,
                                Description = description
                            };
            room.Equipments = null!;

            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.CreateRoom(room);

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(floor, result.Floor);
            Assert.Equal(capacity, result.Capacity);
            Assert.Equal(description, result.Description);
            Assert.Equal(room.Equipments, result.Equipments);
            Assert.True(result.Id > 0);
        }      
    }
}