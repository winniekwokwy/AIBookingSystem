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
}