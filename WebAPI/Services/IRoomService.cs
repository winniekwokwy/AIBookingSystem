using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public interface IRoomService
    {
        IEnumerable<RoomDTO>? ListRooms();
        RoomDTO? GetRoombyID(int id);
        RoomDTO? MapRoom2DTO(Room room);
        RoomDTO? CreateRoom(RoomCreateDTO room);
    }
}