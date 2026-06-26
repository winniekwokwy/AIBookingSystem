namespace AIBookingSystem.DTO
{
    public class EquipmentDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int RoomId { get; set; }
    }
}