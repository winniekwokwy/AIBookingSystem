using AIBookingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AIBookingSystem.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public RoomRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
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

    }
}