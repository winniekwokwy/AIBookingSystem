using System.Net;
using System.Reflection;
using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;
using AIBookingSystem.Models;

using Microsoft.EntityFrameworkCore;

namespace WebAPI.Tests.Repositories
{
    public class BookingRepositoryUnitTests
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
                context.Bookings.AddRange(
                    new Booking{Id = 1, RoomId = 1, BookedBy = "henrysmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 12, 20, 14, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 12, 20, 15, 0, 0, TimeSpan.Zero), Status = BookingStatus.Cancelled},
                    new Booking{Id = 3, RoomId = 2, BookedBy = "henrysmith", UserId = 1, BookingFrom = new DateTimeOffset(2026, 10, 06, 16, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 10, 06, 17, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed},
                    new Booking{Id = 2, RoomId = 1, BookedBy = "cameroncash", UserId = 2, BookingFrom = new DateTimeOffset(2026, 07, 10, 10, 0, 0, TimeSpan.Zero), BookingTo = new DateTimeOffset(2026, 07, 10, 12, 0, 0, TimeSpan.Zero), Status = BookingStatus.Confirmed}
                );

                context.SaveChanges();
            }
            return context;
        }

        [Fact]
        public void ListBookings_ValidUser_ReturnListOfBookings()
        {
            int id = 1;
            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.ListBookings(id);

            Assert.NotNull(result);
            var bookings = Assert.IsType<List<Booking>>(result);
            Assert.NotNull(bookings);
            Assert.Equal(2, bookings.Count());
            if (bookings.Count()>0)
            {
                Assert.Equal(1, bookings.First().Id);
                Assert.Equal(1, bookings.First().RoomId);
                Assert.Equal("henrysmith", bookings.First().BookedBy);
                Assert.Equal(1, bookings.First().UserId);
                Assert.Equal(BookingStatus.Cancelled, bookings.First().Status);
            }          
        }

        [Fact]
        public void ListBookings_InvalidUser_ReturnEmpty()
        {
            int id = -1;
            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.ListBookings(id);

            Assert.NotNull(result);
            Assert.Empty(result);    
        }

        [Fact]
        public void BookRoom_ValidBooking_ReturnBooking()
        {
            int roomId = 1;
            string bookedBy = "henrysmith";
            int userId = 1;
            DateTimeOffset bookingFrom = new DateTimeOffset(2026, 11, 16, 14, 30, 0, TimeSpan.Zero);
            DateTimeOffset bookingTo = new DateTimeOffset(2026, 11, 16, 15, 30, 0, TimeSpan.Zero);
            BookingStatus status = BookingStatus.Confirmed;

            var booking = new Booking{BookedBy = bookedBy, RoomId = roomId, UserId = userId, BookingFrom = bookingFrom, BookingTo = bookingTo, Status = status}; 

            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.BookRoom(booking);

            Assert.NotNull(result);
            Assert.Equal(4, result.Id);
            Assert.Equal(roomId, result.RoomId);
            Assert.Equal(bookedBy, result.BookedBy);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(bookingFrom, result.BookingFrom);
            Assert.Equal(bookingTo, result.BookingTo);
            Assert.Equal(status, result.Status);
        }

        [Fact]
        public void BookRoom_BookingNull_ReturnNull()
        {
            Booking? booking = null;

            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.BookRoom(booking!);

            Assert.Null(result);
        }

        [Fact]
        public void BookRoom_ValidId_ReturnBooking()
        {
            int id = 1;

            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.GetBookingbyID(id);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.RoomId);
            Assert.Equal("henrysmith", result.BookedBy);
            Assert.Equal(1, result.UserId);
            Assert.Equal(BookingStatus.Cancelled, result.Status);
        }

        [Fact]
        public void BookRoom_InvalidId_ReturnNull()
        {
            int id = -1;

            var context = GetInMemoryDbContext(true);
            var repository = new BookingRepository(context);
            var result = repository.GetBookingbyID(id);

            Assert.Null(result);
        }
    }
}