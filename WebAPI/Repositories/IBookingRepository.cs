using AIBookingSystem.Models;

namespace AIBookingSystem.Repositories
{
    public interface IBookingRepository
    {
        IEnumerable<Booking>? ListBookings(int userId);
        Booking? BookRoom(Booking booking);
        Booking? GetBookingbyID(int id);

        Booking? CancelBooking(int id);
    }
}