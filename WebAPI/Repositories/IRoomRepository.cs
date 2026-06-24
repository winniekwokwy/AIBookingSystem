namespace AIBookingSystem.Repositories
{
    public interface IRoomRepository
    {
        Room? GetRoombyID(int ID);
        Room? CreateRoom(Room room);
    }
}