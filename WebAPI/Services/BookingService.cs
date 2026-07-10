using AIBookingSystem.DTO;
using AIBookingSystem.Models;
using AIBookingSystem.Repositories;
using AIBookingSystem.Enums;
using AIBookingSystem.Migrations;

namespace AIBookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        public BookingService(IBookingRepository bookingRepo, IUserService userService, IRoomService roomService)
        {
            _bookingRepo = bookingRepo;
            _userService = userService;
            _roomService = roomService;
        }

        public BookingDTO? MapBooking2DTO(Booking booking)
        {
            if (booking != null){
                var bookingDTO = new BookingDTO{
                    Id = booking.Id,
                    BookedBy = booking.BookedBy,
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    BookingFrom = booking.BookingFrom,
                    BookingTo = booking.BookingTo,
                    Status = StatusMappingEnum2String(booking.Status)
                };
                return bookingDTO;
            }
             return null;
        }

        public bool IsStatusValid(BookingStatus status)
        {
            if (Enum.IsDefined(typeof(BookingStatus), status))
            {
                return true;
            }
            return false;
        }
        
        public string? StatusMappingEnum2String(BookingStatus status)
        {
            if (IsStatusValid(status))
            {
                switch (status)
                {
                    case BookingStatus.Cancelled:
                        return "Cancelled";
                    case BookingStatus.Confirmed:
                        return "Confirmed";
                }
            }
            return null;
        }
        
        public BookingStatus StatusMappingString2Enum(string status)
        {
            if (status.ToLower() == "cancelled")
            {
                return BookingStatus.Cancelled;
            }
            else if (status.ToLower() == "confirmed")
            {
                return BookingStatus.Confirmed;
            }
            return (BookingStatus) (-1);
        }

        public IEnumerable<BookingDTO>? ListBookings(int userId)
        {
            if (userId>0)
            {
                var bookings = _bookingRepo.ListBookings(userId);

                if (bookings != null)
                {
                    return (IEnumerable<BookingDTO>?) bookings
                        .ToList()
                        .Select(b => MapBooking2DTO(b));
                }
            }
            return null;
        }

        public BookingDTO? BookRoom(BookingCreateDTO bookingCreateDTO)
        {
            if (bookingCreateDTO == null)
            {
                return null;
            }
            if (bookingCreateDTO.BookedBy == null || bookingCreateDTO.BookedBy == "")
            {
                return null;
            }
            if (bookingCreateDTO.UserId <=0)
            {
                return null;
            }
            if (!_userService.IsUserValid(bookingCreateDTO.UserId, bookingCreateDTO.BookedBy))
            {
                return null;
            }
            if (bookingCreateDTO.RoomId <= 0)
            {
                return null;
            }
            if (_roomService.GetRoombyID(bookingCreateDTO.RoomId)== null)
            {
                return null;
            }
            if (bookingCreateDTO.BookingTo < bookingCreateDTO.BookingFrom)
            {
                return null;
            }
            if (bookingCreateDTO.BookingFrom < DateTimeOffset.UtcNow)
            {
                return null;
            }
            if (bookingCreateDTO.BookingFrom.Date != bookingCreateDTO.BookingTo.Date)
            {
                return null;
            }
            if (!_roomService.IsRoomAvailable(bookingCreateDTO.RoomId, bookingCreateDTO.BookingFrom, bookingCreateDTO.BookingTo))
            {
                return null;
            }
            var booking = new Booking
            {
                BookedBy = bookingCreateDTO.BookedBy.ToLower(),
                UserId = bookingCreateDTO.UserId,
                RoomId = bookingCreateDTO.RoomId,
                BookingFrom = bookingCreateDTO.BookingFrom,
                BookingTo = bookingCreateDTO.BookingTo,
                Status = BookingStatus.Confirmed
            };

            var createdBooking = _bookingRepo.BookRoom(booking);    
   
            if (createdBooking == null)
            {  
             return null;
            }
            else {
                return new BookingDTO
                {
                    Id = createdBooking.Id,
                    BookedBy = createdBooking.BookedBy,
                    UserId = createdBooking.UserId,
                    RoomId = createdBooking.RoomId,
                    BookingFrom = createdBooking.BookingFrom,
                    BookingTo = createdBooking.BookingTo,
                    Status = StatusMappingEnum2String(createdBooking.Status)
                };
            }
        }

        public BookingDTO? GetBookingbyID(int id)
        {
            if (id >= 0)
            {
                var booking = _bookingRepo.GetBookingbyID(id);
                if (booking == null)
                {
                    return null;
                }

                return MapBooking2DTO(booking);
            }
            return null;
        }

        public BookingDTO? CancelBooking(int id)
        {
            if(id >= 0)
            {
                var booking = GetBookingbyID(id);

                if (booking != null)
                {
                    if (booking.Status == "Confirmed")
                    {
                        var updatedBooking = _bookingRepo.CancelBooking(id);
                        if (updatedBooking == null)
                        {
                            return null;
                        }

                        return MapBooking2DTO(updatedBooking);
                    }
                    else
                    {
                        return booking;
                    } 
                }
            }
            return null;
        }

        public IEnumerable<BookingDTO>? ListBookings(string username)
        {
            if (username != null && username != "")
            {
                var bookings = _bookingRepo.ListBookings(username);

                if (bookings != null)
                {
                    return (IEnumerable<BookingDTO>?) bookings
                        .ToList()
                        .Select(b => MapBooking2DTO(b));
                }
            }
            return null;
        }
    }
}