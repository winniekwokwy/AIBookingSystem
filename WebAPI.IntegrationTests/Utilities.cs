using AIBookingSystem.Data;
using AIBookingSystem.Models;
using AIBookingSystem.Enums;

namespace AIBookingSystem.IntegrationTests
{
    public static class Utilities
    {
        // This method seeds the in-memory database with test data for integration tests.
        // It ensures the database is in a clean, known state before each test run.
        public static void InitializeDbForTests(RoomBookingDbContext db)
        {

            db.Users.RemoveRange(db.Users);

            db.Rooms.RemoveRange(db.Rooms);

            db.Bookings.RemoveRange(db.Bookings);

            db.Equipments.RemoveRange(db.Equipments);

            db.Clients.RemoveRange(db.Clients);

            db.RefreshTokens.RemoveRange(db.RefreshTokens);

            string password = "App13M@ng0";
            db.Users.AddRange(
                new User(){ Id = 1, Name = "Apple Mango", UserName = "applemango", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.Admin, Status = UserStatus.Active},
                new User(){ Id = 2, Name = "Ben Smith", UserName = "bensmith", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.User, Status = UserStatus.Active}
            );

            db.SaveChanges();            

                        Client client1 = new Client()
                            {
                                Id = 1,
                                ClientId = "client-app-one", // Unique client identifier used in JWT tokens
                                Name = "Demo Client Application One",
                                ClientSecret = "fPXxcJw8TW5sA+S4rl4tIPcKk+oXAqoRBo+1s2yjUS4=", // Base64-encoded secret key
                                ClientURL = "https://clientappone.example.com", // Used as Audience in JWT validation
                                IsActive = true // Active client flag
                            };
            Client client2 = new Client()
                            {
                                Id = 2,
                                ClientId = "client-app-two",
                                Name = "Demo Client Application Two",
                                ClientSecret = "UkY2JEdtWqKFY5cEUuWqKZut2o6BI5cf3oexOlCMZvQ=",
                                ClientURL = "https://clientapptwo.example.com",
                                IsActive = true
                            };
            db.Add(client1);
            db.Add(client2);
            db.SaveChanges();
        }
    }
}