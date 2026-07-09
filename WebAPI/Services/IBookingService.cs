using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;

namespace AIBookingSystem.Services
{
    public interface IBookingService
    {
        BookingDTO? MapBooking2DTO(Booking booking); 
        bool IsStatusValid(BookingStatus status);

        string? StatusMappingEnum2String(BookingStatus status);

        BookingStatus StatusMappingString2Enum(string status);
        IEnumerable<BookingDTO>? ListBookings(int userId);
        BookingDTO? GetBookingbyID(int id);
        BookingDTO? BookRoom(BookingCreateDTO bookingCreateDTO);

    }
}