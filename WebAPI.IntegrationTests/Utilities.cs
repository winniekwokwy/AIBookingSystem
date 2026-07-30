using AIBookingSystem.Data;
using AIBookingSystem.Models;

namespace AIBookingSystem.IntegrationTests
{
    public static class Utilities
    {
        // This method seeds the in-memory database with test data for integration tests.
        // It ensures the database is in a clean, known state before each test run.
        public static void InitializeDbForTests(RoomBookingDbContext db)
        {
            // Remove all existing data from Products table to avoid duplicate key errors and stale data
            db.Users.RemoveRange(db.Users);
            // Remove all existing data from Customers table for a clean slate
            db.Rooms.RemoveRange(db.Rooms);
            // Remove all existing data from Orders table to prevent foreign key conflicts
            db.Bookings.RemoveRange(db.Bookings);
            // Remove all existing data from OrderItems table (child of Orders) to ensure consistency
            db.Equipments.RemoveRange(db.Equipments);

            db.Clients.RemoveRange(db.Clients);

            db.RefreshTokens.RemoveRange(db.RefreshTokens);

            // Persist all deletions to the in-memory database
            string password = "App13M@ng0";
            db.Users.AddRange(
                new User(){ Id = 1, Name = "Apple Mango", UserName = "applemango", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.Admin, Status = UserStatus.Active},
                new User() { Id = 2, Name = "Ben Smith", UserName = "bensmith", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.User, Status = UserStatus.Active}
            );

            db.SaveChanges();            
        }
    }
}