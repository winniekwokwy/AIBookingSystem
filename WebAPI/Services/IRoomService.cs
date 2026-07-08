using AIBookingSystem.DTO;
using NodaTime;

namespace AIBookingSystem.Services
{
    public interface IRoomService
    {
        IEnumerable<RoomDTO>? ListRooms();
        RoomDTO? GetRoombyID(int id);
        RoomDTO? MapRoom2DTO(Room room);
        RoomDTO? CreateRoom(RoomCreateDTO room);

        IEnumerable<RoomDTO>? FindAvailableRoomsbyDateTime(DateTimeOffset from, DateTimeOffset to);
        bool IsRoomAvailable(int roomId, DateTimeOffset from, DateTimeOffset to);
    }
}