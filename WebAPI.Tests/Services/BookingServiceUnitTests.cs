using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Enums;
using AIBookingSystem.DTO;
using AIBookingSystem.Models;

using Moq;
using System.Reflection;
using System.Net.Cache;
using System.Net.Mail;

namespace WebAPI.Tests.Services

{
    public class BookingServiceUnitTests
    {
        private readonly BookingService _bookingService;

        private readonly Mock<IUserService> _mockUserService;

        private readonly Mock<IRoomService> _mockRoomService;
        private readonly Mock<IBookingRepository> _mockBookingRepo;

        public BookingServiceUnitTests()
        {
            _mockBookingRepo = new Mock<IBookingRepository>();
            _mockRoomService = new Mock<IRoomService>();
            _mockUserService = new Mock<IUserService>();
            _bookingService = new BookingService(_mockBookingRepo.Object, _mockUserService.Object, _mockRoomService.Object);
        }
        
        [Fact]
        public void MapBooking2DTO_ValidInput_ReturnBookingDTO()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            BookingStatus status = BookingStatus.Confirmed;

            var booking = new Booking{Id = id, BookedBy = bookedBy, RoomId = roomId, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = status}; 

            var result = _bookingService.MapBooking2DTO(booking);

            Assert.NotNull(result);
            Assert.IsType<BookingDTO>(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(bookedBy, result.BookedBy);
            Assert.Equal(roomId, result.RoomId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(bookingFrom, result.BookingFrom);
            Assert.Equal(bookingTo, result.BookingTo);
            Assert.Equal("Confirmed", result.Status);
        }

        [Fact]
        public void MapBooking2DTO_InputNull_ReturnNull()
        {
            var result = _bookingService.MapBooking2DTO((Booking?)null!);

            Assert.Null(result);
        }

        [Fact]
        public void IsStatusValid_ValidConfirmedStatus_ReturnTrue()
        {
            BookingStatus status = BookingStatus.Confirmed;
            var result = _bookingService.IsStatusValid(status);
            Assert.True(result);
        }

        [Fact]
        public void IsStatusValid_ValidCancelledStatus_ReturnTrue()
        {
            BookingStatus status = BookingStatus.Cancelled;
            var result = _bookingService.IsStatusValid(status);
            Assert.True(result);
        }

        [Fact]
        public void IsStatusValid_InvalidStatus_ReturnTrue()
        {
            BookingStatus status = (BookingStatus) (-1);
            var result = _bookingService.IsStatusValid(status);
            Assert.False(result);
        }

        [Fact]
        public void StatusMappingEnum2String_ValidConfirmedStatus_ReturnConfirmedString()
        {
            BookingStatus status = BookingStatus.Confirmed;
            var result = _bookingService.StatusMappingEnum2String(status);

            Assert.NotNull(result);
            Assert.Equal("Confirmed", result);
        }


        [Fact]
        public void StatusMappingEnum2String_ValidCancelledStatus_ReturnCancelledString()
        {
            BookingStatus status = BookingStatus.Cancelled;
            var result = _bookingService.StatusMappingEnum2String(status);

            Assert.NotNull(result);
            Assert.Equal("Cancelled", result);
        }


        [Fact]
        public void StatusMappingEnum2String_InvalidStatus_ReturnNull()
        {
            BookingStatus status = (BookingStatus) (-1);
            var result = _bookingService.StatusMappingEnum2String(status);

            Assert.Null(result);
        }

        [Fact]
        public void StatusMappingString2Enum_ValidConfirmedStatus_ReturnConfirmedBookingStatus()
        {
            string status = "Confirmed";
            var result = _bookingService.StatusMappingString2Enum(status);

            Assert.Equal(BookingStatus.Confirmed, result);
        }

        [Fact]
        public void StatusMappingString2Enum_ValidCancelledStatus_ReturnCancelledBookingStatus()
        {
            string status = "Cancelled";
            var result = _bookingService.StatusMappingString2Enum(status);

            Assert.Equal(BookingStatus.Cancelled, result);
        }

        [Fact]
        public void StatusMappingString2Enum_InvalidStatus_ReturnInvalidBookingStatus()
        {
            string status = "Modified";
            var result = _bookingService.StatusMappingString2Enum(status);

            Assert.Equal((BookingStatus) (-1), result);
        }

        [Fact]
        public void ListBookings_ValidUserId_ReturnListOfBookingDTO()
        {
            int userId = 1;

            var booking1 = new Booking{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Cancelled};
            var booking2 = new Booking{Id = 2, RoomId = 2, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 07, 28, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 07, 28, 11, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed};
            var bookings = new List<Booking> (){booking1, booking2};

            _mockBookingRepo.Setup(r => r.ListBookings(userId))
                            .Returns(bookings);
            
            var result = _bookingService.ListBookings(userId);

            Assert.NotNull(result);
            Assert.Equal(bookings.Count(), result.Count());
            if(result.Count()>0)
            {
                Assert.Equal(booking1.Id, result.First().Id);
                Assert.Equal(booking1.RoomId, result.First().RoomId);
                Assert.Equal(booking1.BookedBy, result.First().BookedBy);
                Assert.Equal(booking1.UserId, result.First().UserId);
                Assert.Equal(booking1.BookingFrom, result.First().BookingFrom);
                Assert.Equal(booking1.BookingTo, result.First().BookingTo);
                Assert.Equal("Cancelled", result.First().Status);
            }
        }

        [Fact]
        public void ListBookings_InvalidUserId_ReturnNull()
        {
            int userId = -1;
            
            var result = _bookingService.ListBookings(userId);

            Assert.Null(result);
        }

        [Fact]
        public void ListBookings_WhenRepoReturnNull_ReturnNull()
        {
            int userId = -1;

            _mockBookingRepo.Setup(r => r.ListBookings(userId))
                            .Returns((List<Booking>?) null);
            
            var result = _bookingService.ListBookings(userId);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_BookingCreateDTONull_ReturnNull()
        {
            var bookingCreateDTO = (BookingCreateDTO) null!;
            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_BookedByNull_ReturnNull()
        {
            int roomId = 1;
            string? bookedBy = null!;
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_InvalidUserId_ReturnNull()
        {
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = -1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_InvalidRoomId_ReturnNull()
        {
            int roomId = -1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }
                [Fact]
        public void BookRoom_InvalidBookingPeriod_ReturnNull()
        {
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_BookingPeriodInThePast_ReturnNull()
        {
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2025, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2025, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_BookingFromNToNotOnTheSameDay_ReturnNull()
        {
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 21, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }
        
        [Fact]
        public void BookRoom_InvalidUser_ReturnNull()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 


            _mockUserService.Setup(u => u.IsUserValid(id, bookedBy))
                            .Returns(false);

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }
        
        [Fact]
        public void BookRoom_RequestedRoomNotFound_ReturnNull()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            _mockUserService.Setup(u => u.IsUserValid(id, bookedBy))
                            .Returns(true);
            _mockRoomService.Setup(r => r.GetRoombyID(roomId))
                            .Returns((RoomDTO?)null);

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_RequestedRoomNotAvailable_ReturnNull()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var roomDTO = new RoomDTO{
                Id = roomId,
                Name = name,
                Floor = floor,
                Capacity = capacity,
                Description = description,
            };

            _mockUserService.Setup(u => u.IsUserValid(id, bookedBy))
                            .Returns(true);
            _mockRoomService.Setup(r => r.GetRoombyID(roomId))
                            .Returns(roomDTO);
            _mockRoomService.Setup(r => r.IsRoomAvailable(roomId, bookingFrom, bookingTo))
                            .Returns(false);
 
            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_RepoReturnNull_ReturnNull()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var roomDTO = new RoomDTO{
                Id = roomId,
                Name = name,
                Floor = floor,
                Capacity = capacity,
                Description = description,
            };

            _mockUserService.Setup(u => u.IsUserValid(id, bookedBy))
                            .Returns(true);
            _mockRoomService.Setup(r => r.GetRoombyID(roomId))
                            .Returns(roomDTO);
            _mockRoomService.Setup(r => r.IsRoomAvailable(roomId, bookingFrom, bookingTo))
                            .Returns(true);
            _mockBookingRepo.Setup(r => r.BookRoom(It.Is<Booking>(b =>
                            b.BookedBy == bookingCreateDTO.BookedBy.ToLower() &&
                            b.UserId == bookingCreateDTO.UserId &&
                            b.RoomId == bookingCreateDTO.RoomId &&
                            b.BookingFrom == bookingCreateDTO.BookingFrom &&
                            b.BookingTo == bookingCreateDTO.BookingTo &&
                            b.Status ==  BookingStatus.Confirmed)))
                            .Returns((Booking?)null);

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_ValidBookingCreateDTO_ReturnBookingDTO()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string  status = "Confirmed";

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 
            
            var returnedBooking = new Booking
            {
                Id = id,
                BookedBy = bookingCreateDTO.BookedBy.ToLower(),
                UserId = bookingCreateDTO.UserId,
                RoomId = bookingCreateDTO.RoomId,
                BookingFrom = bookingCreateDTO.BookingFrom,
                BookingTo = bookingCreateDTO.BookingTo,
                Status = BookingStatus.Confirmed
            };

            string name = "Paris";
            int floor = 5;
            int capacity = 6;
            string description = $"This room is located at {floor}/F which can accommodate {capacity} people.";

            var roomDTO = new RoomDTO{
                Id = roomId,
                Name = name,
                Floor = floor,
                Capacity = capacity,
                Description = description,
            };

            _mockUserService.Setup(u => u.IsUserValid(id, bookedBy.ToLower()))
                            .Returns(true);
            _mockRoomService.Setup(r => r.GetRoombyID(roomId))
                            .Returns(roomDTO);
            _mockRoomService.Setup(r => r.IsRoomAvailable(roomId, bookingFrom, bookingTo))
                            .Returns(true);
            _mockBookingRepo.Setup(r => r.BookRoom(It.Is<Booking>(b =>
                            b.BookedBy == bookingCreateDTO.BookedBy.ToLower() &&
                            b.UserId == bookingCreateDTO.UserId &&
                            b.RoomId == bookingCreateDTO.RoomId &&
                            b.BookingFrom == bookingCreateDTO.BookingFrom &&
                            b.BookingTo == bookingCreateDTO.BookingTo &&
                            b.Status ==  BookingStatus.Confirmed)))
                            .Returns(returnedBooking);

            var result = _bookingService.BookRoom(bookingCreateDTO);

            Assert.NotNull(result);
            Assert.IsType<BookingDTO>(result);
            Assert.Equal(roomId, result.RoomId);
            Assert.Equal(bookedBy.ToLower(), result.BookedBy.ToLower());
            Assert.Equal(userId, result.UserId);
            Assert.Equal(bookingFrom, result.BookingFrom);
            Assert.Equal(bookingTo, result.BookingTo);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetBookingbyID_ValidId_ReturnBookingDTO()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string  status = "Confirmed";

            var bookingDTO = new BookingDTO{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = status};
            var booking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Confirmed}; 
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns(booking);
            
            var result = _bookingService.GetBookingbyID(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(roomId, result.RoomId);
            Assert.Equal(bookedBy.ToLower(), result.BookedBy);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(bookingFrom, result.BookingFrom);
            Assert.Equal(bookingTo, result.BookingTo);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void GetBookingbyID_InvalidId_ReturnNull()
        {
            int id = 1;
            
            var result = _bookingService.GetBookingbyID(id);

            Assert.Null(result);   
        }

        [Fact]
        public void GetBookingbyID_RepoReturnNull_ReturnNull()
        {
            int id = 1;
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns((Booking?)null);
            
            var result = _bookingService.GetBookingbyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void CancelBooking_ValidId_ReturnBookingDTO()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string  status = "Cancelled";

            var booking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Confirmed}; 
            var updatedBooking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Cancelled}; 
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns(booking);
            _mockBookingRepo.Setup(m => m.CancelBooking(id))
                            .Returns(updatedBooking);

            var result = _bookingService.CancelBooking(id);
            
            _mockBookingRepo.Verify(m => m.GetBookingbyID(id), Times.Once);
            _mockBookingRepo.Verify(m => m.CancelBooking(id), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void CancelBooking_InvalidId_ReturnNull()
        {
            int id = -1;

            var result = _bookingService.GetBookingbyID(id);

            Assert.Null(result);
        }

        [Fact]
        public void CancelBooking_NoBookingFound_ReturnNull()
        {
            int id = 1;
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns((Booking?)null);

            var result = _bookingService.CancelBooking(id);
            
            _mockBookingRepo.Verify(m => m.GetBookingbyID(id), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void CancelBooking_BookingStatusCancelled_ReturnBooking()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string  status = "Cancelled";

            var booking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Cancelled}; 
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns(booking);

            var result = _bookingService.CancelBooking(id);
            
            _mockBookingRepo.Verify(m => m.GetBookingbyID(id), Times.Once);
            _mockBookingRepo.Verify(m => m.CancelBooking(id), Times.Never);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void CancelBooking_RepoReturnNull_ReturnNull()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var booking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Confirmed}; 
            var updatedBooking = new Booking{Id = id, RoomId = roomId, BookedBy = bookedBy.ToLower(), UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = BookingStatus.Cancelled}; 
 
            _mockBookingRepo.Setup(m => m.GetBookingbyID(id))
                            .Returns(booking);
            _mockBookingRepo.Setup(m => m.CancelBooking(id))
                            .Returns((Booking?)null);

            var result = _bookingService.CancelBooking(id);
            
            _mockBookingRepo.Verify(m => m.GetBookingbyID(id), Times.Once);
            _mockBookingRepo.Verify(m => m.CancelBooking(id), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void ListBookings_ValidUsername_ReturnListOfBookingDTO()
        {
            string username = "henrysmith";

            var booking1 = new Booking{Id = 1, RoomId = 1, BookedBy = username, UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Cancelled};
            var booking2 = new Booking{Id = 2, RoomId = 2, BookedBy = username, UserId = 1, BookingFrom = new DateTimeOffset(2026, 07, 28, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 07, 28, 11, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed};
            var bookings = new List<Booking> (){booking1, booking2};

            _mockBookingRepo.Setup(r => r.ListBookings(username))
                            .Returns(bookings);
            
            var result = _bookingService.ListBookings(username);

            Assert.NotNull(result);
            Assert.Equal(bookings.Count(), result.Count());
            if(result.Count()>0)
            {
                Assert.Equal(booking1.Id, result.First().Id);
                Assert.Equal(booking1.RoomId, result.First().RoomId);
                Assert.Equal(booking1.BookedBy, result.First().BookedBy);
                Assert.Equal(booking1.UserId, result.First().UserId);
                Assert.Equal(booking1.BookingFrom, result.First().BookingFrom);
                Assert.Equal(booking1.BookingTo, result.First().BookingTo);
                Assert.Equal("Cancelled", result.First().Status);
            }
        }

        [Fact]
        public void ListBookings_NullUsername_ReturnNull()
        {
            string? username = null!;

            var result = _bookingService.ListBookings(username);

            Assert.Null(result);
        }

        [Fact]
        public void ListBookings_EmptyUsername_ReturnNull()
        {
            string username = "";
            
            var result = _bookingService.ListBookings(username);

            Assert.Null(result);
        }

        [Fact]
        public void ListBookings_RepoReturnNull_ReturnNull()
        {
            string username = "henrysmith";

            _mockBookingRepo.Setup(r => r.ListBookings(username))
                            .Returns((List<Booking>?)null);
            
            var result = _bookingService.ListBookings(username);

            Assert.Null(result);
        }
    }
}