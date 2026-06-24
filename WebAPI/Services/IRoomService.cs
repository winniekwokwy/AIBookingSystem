using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public interface IRoomService
    {
        RoomDTO? GetRoombyID(int ID);
        RoomDTO? MapRoom2DTO(Room room);
        RoomDTO? CreateRoom(RoomCreateDTO room);
    }
}