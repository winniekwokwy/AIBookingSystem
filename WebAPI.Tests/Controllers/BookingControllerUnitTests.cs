using AIBookingSystem.Controllers;
using AIBookingSystem.Services;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;

using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Net;
using System.Reflection;

namespace WebAPI.Tests.Controllers

{
    public class BookingControllerUnitTests
    {
        private readonly BookingController _bookingController;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<ILogger<BookingController>> _mockLogger;

        public BookingControllerUnitTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<ILogger<BookingController>>();
            _bookingController = new BookingController(_mockBookingService.Object, _mockLogger.Object);
        }

        [Fact]
        public void ListBookings_ValidUserId_ReturnListOfBookings()
        {
            int userId = 1;

            var booking1 = new BookingDTO{Id = 1, RoomId = 1, BookedBy = "HenrySmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = "Cancelled"};
            var bookings = new List<BookingDTO> (){booking1};
     
            _mockBookingService.Setup(b => b.ListBookings(userId))
                                .Returns(bookings);
            var result = _bookingController.ListBookings(userId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingDTO>>(okResult.Value);
            Assert.Single(returnedBookings);
            if (returnedBookings.Count()>0)
            {
                Assert.Equal(booking1.RoomId, returnedBookings.First().RoomId);
                Assert.Equal(booking1.BookedBy, returnedBookings.First().BookedBy);
                Assert.Equal(booking1.UserId, returnedBookings.First().UserId);
                Assert.Equal(booking1.BookingFrom, returnedBookings.First().BookingFrom);
                Assert.Equal(booking1.BookingTo, returnedBookings.First().BookingTo);
                Assert.Equal(booking1.Status, returnedBookings.First().Status);
            } 
        }

        [Fact]
        public void ListBookings_InvalidUserId_ReturnBadRequest()
        {
            int userId = -1;
            string expected = "User Id is invalid.";

            var result = _bookingController.ListBookings(userId);
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);
        }

        [Fact]
        public void ListBookings_WhenServiceReturnNull_ReturnNotFound()
        {
            int userId = 99;
            string expected = "No booking is found. Please check if the user id is valid.";
            
            _mockBookingService.Setup(b => b.ListBookings(userId))
                                .Returns((List<BookingDTO>?)null);
            var result = _bookingController.ListBookings(userId);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.Equal(expected, notFoundResult.Value);
        }


        [Fact]
        public void ListBookings_WhenServiceReturnEmptyList_ReturnNotFound()
        {
            int userId = 99;
            List<BookingDTO> bookings = [];
            string expected = "No booking is found. Please check if the user id is valid.";
            
            _mockBookingService.Setup(b => b.ListBookings(userId))
                                .Returns(bookings);
            var result = _bookingController.ListBookings(userId);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.Equal(expected, notFoundResult.Value);
        }

        [Fact]
        public void BookRoom_ValidInput_ReturnBookingDTO()
        {
            int id = 1;
            int roomId = 1;
            string bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string  status = "Confirmed";

            var bookingDTO = new BookingDTO{Id = id, RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = status};
            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            _mockBookingService.Setup(b => b.BookRoom(bookingCreateDTO))
                                .Returns(bookingDTO);
            var result = _bookingController.BookRoom(bookingCreateDTO);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsAssignableFrom<BookingDTO>(okResult.Value);
            Assert.Equal(bookingDTO.RoomId, returnedBooking.RoomId);
            Assert.Equal(bookingDTO.BookedBy, returnedBooking.BookedBy);
            Assert.Equal(bookingDTO.UserId, returnedBooking.UserId);
            Assert.Equal(bookingDTO.BookingFrom, returnedBooking.BookingFrom);
            Assert.Equal(bookingDTO.BookingTo, returnedBooking.BookingTo);
            Assert.Equal(bookingDTO.Status, returnedBooking.Status);        
        }

        [Fact]
        public void BookRoom_BookingCreateDTONull_ReturnBadRequest()
        {
            string expected = "Booking details are not provided.";
            
            var result = _bookingController.BookRoom((BookingCreateDTO) null!);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_BookedByNull_ReturnBadRequest()
        {
            string expected = "Name of the user for the booking is not provided.";
            
            int roomId = 1;
            string? bookedBy = null;
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_BookedByEmpty_ReturnBadRequest()
        {
            string expected = "Name of the user for the booking is not provided.";
            
            int roomId = 1;
            string? bookedBy = "";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_InvalidUserId_ReturnBadRequest()
        {
            string expected = "Invalid user Id is provided.";
            
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = -1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_InvalidRoomId_ReturnBadRequest()
        {
            string expected = "Invalid room Id is provided.";
            
            int roomId = -1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_InvalidPeriod_ReturnBadRequest()
        {
            string expected = "Invalid booking period is provided.";
            
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_PeriodInThePast_ReturnBadRequest()
        {
            string expected = "Booking date must be in the future.";
            
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2025, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2025, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_BookingPeriodIsNotOnTheSameDay_ReturnBadRequest()
        {
            string expected = "Booking From and To are not on the same day.";
            
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 21, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void BookRoom_WhenServiceReturnNull_ReturnBadRequest()
        {
            string expected = "Booking is not made successfully. The following could be the reason:-\n 1. Room or user is invalid.\n 2. The room is unavailable.";
            
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);

            var bookingCreateDTO = new BookingCreateDTO{RoomId = roomId, BookedBy = bookedBy!, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo}; 

            _mockBookingService.Setup(b => b.BookRoom(bookingCreateDTO))
                                    .Returns((BookingDTO?)null);
            var result = _bookingController.BookRoom(bookingCreateDTO);
            
            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);      
        }

        [Fact]
        public void GetBookingbyID_ValidInput_ReturnRoomDTO()
        {
            int id = 1;
            int roomId = 1;
            string? bookedBy = "HenrySmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero);
            string status = "Confirmed";
            BookingDTO bookingDTO = new BookingDTO{Id = id, RoomId = roomId, BookedBy = bookedBy, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = status};

            _mockBookingService.Setup(b => b.GetBookingbyID(id))
                                .Returns(bookingDTO);
            var result = _bookingController.GetBookingbyID(id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsAssignableFrom<BookingDTO>(okResult.Value);
            Assert.Equal(bookingDTO.RoomId, returnedBooking.RoomId);
            Assert.Equal(bookingDTO.BookedBy, returnedBooking.BookedBy);
            Assert.Equal(bookingDTO.UserId, returnedBooking.UserId);
            Assert.Equal(bookingDTO.BookingFrom, returnedBooking.BookingFrom);
            Assert.Equal(bookingDTO.BookingTo, returnedBooking.BookingTo);
            Assert.Equal(bookingDTO.Status, returnedBooking.Status);    
        }

        [Fact]
        public void GetBookingbyID_InvalidId_ReturnBadRequest()
        {
            int id = -1;
            string expected = "Please provide valid Id for getting a booking.";
            var result = _bookingController.GetBookingbyID(id);

            var badRequstResult = result.Result as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(badRequstResult);
            Assert.Equal(expected, badRequstResult.Value);     
        }

        [Fact]
        public void GetBookingbyID_WhenServiceReturnNull_ReturnNotFound()
        {
            int id = 1;
            string expected = $"Booking with ID, {id}, not found.";

            _mockBookingService.Setup(b => b.GetBookingbyID(id))
                                .Returns((BookingDTO?)null);
            var result = _bookingController.GetBookingbyID(id);

            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.Equal(expected, notFoundResult.Value);     
        }
    }
}