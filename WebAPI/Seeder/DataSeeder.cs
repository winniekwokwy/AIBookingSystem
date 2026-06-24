using Bogus;
using AIBookingSystem.Enums;
using AIBookingSystem.Data;

public class DataSeeder
{
    private readonly RoomBookingDbContext _context;

    public DataSeeder(RoomBookingDbContext context)
    {
        _context = context;
    }

    public void SeedUsers(int noOfUsers, UserRoles typeOfUser)
    {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Password, f => "@bcd3fgh")
                .RuleFor(u => u.Role, f => typeOfUser)
                .RuleFor(u => u.Status, f => UserStatus.Active);

           var users = userFaker.Generate(noOfUsers);

            _context.AddRange(users);
            _context.SaveChanges();
    }

    public void SeedUsers(int noOfUsers, int noOfAdmins)
    {
        if (!_context.Users.Any())
        {
            SeedUsers(noOfUsers, UserRoles.User);
            SeedUsers(noOfAdmins, UserRoles.Admin);
            
        }
    }

    public void SeedRooms(int noOfRooms)
    {
        if (!_context.Rooms.Any()){
            var roomFaker = new Faker<Room>()
                .RuleFor(r => r.Name, f => f.Address.Country())
                .RuleFor(r => r.Capacity, f => f.Random.Number(1, 20))
                .RuleFor(r => r.Floor, f => f.Random.Number(1, 100))
                .RuleFor(r => r.Description, f => "");

            var rooms = roomFaker.Generate(noOfRooms);

            if (rooms != null)
            {
                int count = 1;
                foreach (var room in rooms) 
                {
                    room.Description = $"This room is located at {room.Floor}/F which can accomodate {room.Capacity} people.";
                    room.Equipments = new List<Equipment>();
                    room.Equipments.Add(new Equipment()
                            {
                                Name = "Telephone",
                                RoomId = room.Id,
                            });
                    if (room.Capacity <= 2)
                    {
                        room.Description = room.Description + "There is no windows in the room.";
                    }
                    else {
                        if (count%2 == 0)
                            {
                                room.Description = room.Description + " It faces to the main road.";
                                room.Equipments.Add(new Equipment()
                                {
                                    Name = "TV",
                                    RoomId = room.Id,
                                });
                            }
                        if (count%3 == 0)
                            {
                                room.Description = room.Description + " It has a garden view.";
                                                            room.Equipments.Add(new Equipment()
                                {
                                    Name = "Projector",
                                    RoomId = room.Id,
                                });
                            }
                        if (count%5 == 0)
                            {
                                room.Description = room.Description + " it has a sea view.";
                                room.Equipments.Add(new Equipment()
                                {
                                    Name = "Teleconferencing",
                                    RoomId = room.Id,
                                });
                            }
                    }
                    count = count+1;
                }
                _context.AddRange(rooms);
                _context.SaveChanges();
            }
        }
    }
}