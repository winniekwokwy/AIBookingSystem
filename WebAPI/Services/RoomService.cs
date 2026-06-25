using AIBookingSystem.DTO;
using AIBookingSystem.Repositories;

namespace AIBookingSystem.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepo;
        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public RoomDTO? MapRoom2DTO(Room room)
        {
            if (room != null){
                var roomDTO = new RoomDTO{
                    Id = room.Id,
                    Name = room.Name,
                    Floor = room.Floor,
                    Capacity = room.Capacity,
                    Description = room.Description,
                };

                if (roomDTO.Equipments == null)
                {
                    roomDTO.Equipments = [];                        
                }
                if (room.Equipments != null)
                {
                    foreach (var equipment in room.Equipments)
                    {
                        roomDTO.Equipments.Add(new EquipmentDTO
                        {
                            Id = equipment.Id,
                            Name = equipment.Name,
                            RoomId = equipment.RoomId
                        });
                    }
                }
                return roomDTO;
            }
            return null;
        }

        public RoomDTO? GetRoombyID(int id)
        {
            if (id >= 0)
            {
                var room = _roomRepo.GetRoombyID(id);
                if (room == null)
                {
                    return null;
                }

                var newRoom = MapRoom2DTO(room);

                return newRoom;
            }
            return null;
        }

        public RoomDTO? CreateRoom(RoomCreateDTO room)
        {
            if (room != null){
                var newRoom = new Room
                                {
                                    Name = room.Name,
                                    Floor = room.Floor,
                                    Capacity = room.Capacity,
                                    Description = room.Description
                                };
                if (room.Equipments != null)
                {
                    if (room.Equipments.Count()>0)
                    {
                        if (newRoom.Equipments == null)
                        {
                            newRoom.Equipments = [];
                        }
                        foreach (var equipment in room.Equipments)
                        {
                            newRoom.Equipments.Add(new Equipment
                            {
                                Name = equipment.Name,
                                RoomId = equipment.RoomId
                            });
                        }
                    }
                }

                var addedRoom = _roomRepo.CreateRoom(newRoom);
                if (addedRoom != null)
                {
                    return MapRoom2DTO(addedRoom);
                }
            }
            return null;
        }
    }
}