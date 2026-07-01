using System.Net;
using System.Reflection;
using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;
using AIBookingSystem.Models;

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

        [Fact]
        public void FindAvailableRoomsbyDateTime_NoBookingsOnRequstedDate_ReturnListofAvailableRooms()
        {
            Room room1 = new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."};
            Room room2 = new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."};
            Room room3 = new Room() { Id = 3, Name = "Shatin", Floor = 5, Capacity = 8, Description = "This room is located at 5/F which can accommodate 8 people."};

            room1.Bookings = [new Booking{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 21, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 21, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed},
                             new Booking{Id = 2, RoomId = 1, BookedBy = "MarySmith", UserId = 2, BookingFrom = new DateTimeOffset(2026, 7, 20, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 7, 20, 12, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}   
                                ];    
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(false);

            context.Rooms.AddRange(room1, room2, room3);
            context.SaveChanges();
            var repository = new RoomRepository(context);
            var checkRooms = repository.ListRooms();

            if (checkRooms != null)
            {
                foreach (var room in checkRooms)
                {
                    Console.WriteLine($"Room {room.Name}");
                }
            }

            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.NotNull(result);

            foreach (var room in result)
            {
                Console.WriteLine($"Room {room.Name}");
            }
                        
            Assert.Equal(3, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(room1.Name, result.First().Name);
                Assert.Equal(room1.Floor, result.First().Floor);
                Assert.Equal(room1.Capacity, result.First().Capacity);
                Assert.Equal(room1.Description, result.First().Description);
            } 
        }    

        [Fact]
        public void FindAvailableRoomsbyDateTime_NoBookingsAtRequstedPeriod_ReturnListofAvailableRooms()
        {
            Room room1 = new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."};
            Room room2 = new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."};
            Room room3 = new Room() { Id = 3, Name = "Shatin", Floor = 5, Capacity = 8, Description = "This room is located at 5/F which can accommodate 8 people."};

            room1.Bookings = [new Booking{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 21, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 21, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed},
                             new Booking{Id = 2, RoomId = 1, BookedBy = "MarySmith", UserId = 2, BookingFrom = new DateTimeOffset(2026, 7, 20, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 7, 20, 12, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}   
                                ];
            room2.Bookings = [new Booking{Id = 3, RoomId = 2, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 11, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}  
                                ];      
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(false);

            context.Rooms.AddRange(room1, room2, room3);
            context.SaveChanges();
            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(room1.Name, result.First().Name);
                Assert.Equal(room1.Floor, result.First().Floor);
                Assert.Equal(room1.Capacity, result.First().Capacity);
                Assert.Equal(room1.Description, result.First().Description);
            } 
        }      

        [Fact]
        public void FindAvailableRoomsbyDateTime_BookingsAtRequestPeriod_ReturnListofAvailableRooms()
        {
            Room room1 = new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."};
            Room room2 = new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."};
            Room room3 = new Room() { Id = 3, Name = "Shatin", Floor = 5, Capacity = 8, Description = "This room is located at 5/F which can accommodate 8 people."};

            room1.Bookings = [new Booking{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed},
                             new Booking{Id = 2, RoomId = 1, BookedBy = "MarySmith", UserId = 2, BookingFrom = new DateTimeOffset(2026, 7, 20, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 7, 20, 12, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}   
                                ];    
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(false);

            context.Rooms.AddRange(room1, room2, room3);
            context.SaveChanges();

            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            if (result.Count()>0)
            {
                Assert.Equal(room2.Name, result.First().Name);
                Assert.Equal(room2.Floor, result.First().Floor);
                Assert.Equal(room2.Capacity, result.First().Capacity);
                Assert.Equal(room2.Description, result.First().Description);
            } 
        }

        [Fact]
        public void FindAvailableRoomsbyDateTime_CancelledBookingAtRequestPeriod_ReturnListofAvailableRooms()
        {
            Room room1 = new Room() { Id = 1, Name = "Mongkok", Floor = 2, Capacity = 5, Description = "This room is located at 2/F which can accommodate 5 people."};
            Room room2 = new Room() { Id = 2, Name = "Tai wai", Floor = 3, Capacity = 8, Description = "This room is located at 3/F which can accommodate 8 people."};
            Room room3 = new Room() { Id = 3, Name = "Shatin", Floor = 5, Capacity = 8, Description = "This room is located at 5/F which can accommodate 8 people."};

            room1.Bookings = [new Booking{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Cancelled},
                             new Booking{Id = 2, RoomId = 1, BookedBy = "MarySmith", UserId = 2, BookingFrom = new DateTimeOffset(2026, 7, 20, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 7, 20, 12, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}   
                                ];    
            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 20, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(false);

            context.Rooms.AddRange(room1, room2, room3);
            context.SaveChanges();

            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
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
     
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.Null(result);
        }  

        [Fact]
        public void FindAvailableRoomsbyDateTime_PeriodInThePast_ReturnNull()
        {

            DateTimeOffset from = new DateTimeOffset(2025, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2025, 12, 20, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.Null(result);
        }  

        [Fact]
        public void FindAvailableRoomsbyDateTime_PeriodNotOnTheSameDay_ReturnNull()
        {

            DateTimeOffset from = new DateTimeOffset(2026, 12, 20, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset to = new DateTimeOffset(2026, 12, 21, 15, 30, 0, TimeSpan.Zero);
     
            var context = GetInMemoryDbContext(true);
            var repository = new RoomRepository(context);
            var result = repository.FindAvailableRoomsbyDateTime(from, to);
            Assert.Null(result);
        }  
    }
}