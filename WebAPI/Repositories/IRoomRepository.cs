using NodaTime;

namespace AIBookingSystem.Repositories
{
    public interface IRoomRepository
    {
        IEnumerable<Room>? ListRooms();
        Room? GetRoombyID(int ID);
        Room? CreateRoom(Room room);
        IEnumerable<Room>? FindAvailableRoomsbyDateTime(DateTimeOffset from, DateTimeOffset to);
        bool IsRoomAvailable(int roomId, DateTimeOffset from, DateTimeOffset to);
    }
}