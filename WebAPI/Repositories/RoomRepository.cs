using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace AIBookingSystem.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public RoomRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public IEnumerable<Room>? ListRooms()
        {
            return _dBContext.Rooms
                .Include(r => r.Equipments)
                .ToList();
        }

        public Room? GetRoombyID(int id)
        {
            var room = _dBContext.Rooms
                        .Include(r => r.Equipments)
                        .FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return null;
            }

            return room;
        }


        public Room? CreateRoom(Room room)
        {    
            if (room != null) 
            {
                _dBContext.Rooms.Add(room);
                _dBContext.SaveChanges();

                var addedRoom = _dBContext.Rooms.FirstOrDefault(r => r.Id == room.Id);

                if (addedRoom != null)
                {
                    return addedRoom;
                }
            }
            return null;
        }

        public IEnumerable<Room>? FindAvailableRoomsbyDateTime(DateTimeOffset from, DateTimeOffset to)
        {
 
            if (to < from)
            {
                return null;
            }
            if (from < DateTimeOffset.UtcNow)
            {
                return null;
            }
            if (to.Date != from.Date)
            {
                return null;
            }

            return _dBContext.Rooms
                    .Where(r => !r.Bookings.Any
                    (
                        b => b.BookingFrom <= to 
                        && b.BookingTo >= from
                        && b.Status == BookingStatus.Confirmed
                    ))
                    .Include(r => r.Equipments)
                    .ToList();

        }

    }
}