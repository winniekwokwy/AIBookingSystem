using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AIBookingSystem.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public BookingRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public IEnumerable<Booking>? ListBookings(int userId)
        {
            return _dBContext.Bookings
                .Where(b => b.UserId == userId)
                .ToList();
        }

        public Booking? BookRoom(Booking booking)
        {
            if (booking != null)
            {
                _dBContext.Bookings.Add(booking);
                _dBContext.SaveChanges();

                return _dBContext.Bookings.FirstOrDefault(b => b.Id == booking.Id);

            }
            return null;
        }

        public Booking? GetBookingbyID(int id)
        {
            return _dBContext.Bookings
                        .FirstOrDefault(b => b.Id == id);
   
        }

        public Booking? CancelBooking(int id)
        {
            var booking = _dBContext.Bookings
                        .FirstOrDefault(b => b.Id == id);

            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                _dBContext.SaveChanges();
            }
            return booking;
        }
    }
}