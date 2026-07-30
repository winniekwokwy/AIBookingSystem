using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Repositories;
using AIBookingSystem.DTO;

using Microsoft.EntityFrameworkCore;

namespace WebAPI.Tests.Repositories
{
    public class UserRepositoryUnitTests
    {
        // Helper method that creates a fresh, isolated ApplicationDbContext using EF Core InMemory provider
        private RoomBookingDbContext GetInMemoryDbContext(bool requiredData)
        {
            var options = new DbContextOptionsBuilder<RoomBookingDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new RoomBookingDbContext(options);

            string password = "App13M@ng0";

            if (requiredData)
            {
                context.Users.AddRange(
                    new User(){ Id = 1, Name = "Apple Mango", UserName = "applemango", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.Admin, Status = UserStatus.Active},
                    new User() { Id = 2, Name = "Ben Smith", UserName = "bensmith", PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), Role = UserRoles.User, Status = UserStatus.Active}
                );

                context.SaveChanges();
            }
            return context;
        }

        [Fact]
        public void IsUserValid_ValidUserIdnCreatedBySmallLetter_ReturnTrue()
        {
            int id = 1;
            string createdBy = "applemango";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.True(userValid);
        }

        [Fact]
        public void IsUserValid_ValidUserIdnCreatedByCapitalLetter_ReturnTrue()
        {
            int id = 1;
            string createdBy = "APPLEMANGO";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.True(userValid);
        }

        [Fact]
        public void IsUserValid_InvalidUserIdnValidCreatedBy_ReturnFalse()
        {
            int id = -1;
            string createdBy = "AppleMango";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.False(userValid);
        }

        [Fact]
        public void IsUserValid_ValidUserIdnNullCreatedBy_ReturnFalse()
        {
            int id = 1;
            string createdBy = null!;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.False(userValid);
        }

        [Fact]
        public void IsUserValid_ValidUserIdnMissingCreatedBy_ReturnFalse()
        {
            int id = 1;
            string createdBy = "";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.False(userValid);
        }

        [Fact]
        public void IsUserValid_UserNotFound_ReturnFalse()
        {
            int id = 3;
            string createdBy = "IanSmith";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.False(userValid);
        }

        [Fact]
        public void IsUserValid_UserIdnCreatedByNotMatch_ReturnFalse()
        {
            int id = 2;
            string createdBy = "AppleMango";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var userValid = repository.IsUserValid(id, createdBy);

            Assert.False(userValid);
        }

        [Fact]
        public void GetUserByID_ValidId_ReturnUser()
        {
            int id = 1;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyID(id);

            Assert.NotNull(user);
            Assert.Equal("Apple Mango", user.Name);
            Assert.Equal("applemango", user.UserName);
            Assert.Equal(1, user.Id);
            Assert.Equal(UserRoles.Admin, user.Role);
            Assert.Equal(UserStatus.Active, user.Status);
        }

        [Fact]
        public void GetUserByID_InvalidId_ReturnNull()
        {
            int id = -1;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyID(id);

            Assert.Null(user);
        }

        [Fact]
        public void GetUserByID_NonExistingUser_ReturnNull()
        {
            int id = 3;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyID(id);

            Assert.Null(user);
        }

        [Fact]
        public void GetUserByUsername_ValidUsername_ReturnUser()
        {
            string username = "applemango";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyUsername(username);

            Assert.NotNull(user);
            Assert.Equal("Apple Mango", user.Name);
            Assert.Equal(username, user.UserName);
            Assert.Equal(1, user.Id);
            Assert.Equal(UserRoles.Admin, user.Role);
            Assert.Equal(UserStatus.Active, user.Status);
        }

        [Fact]
        public void GetUserByID_MissingUsername_ReturnNull()
        {
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyUsername("");

            Assert.Null(user);
        }

        [Fact]
        public void GetUserByID_NullUsername_ReturnNull()
        {
            string username = null!;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyUsername(username);

            Assert.Null(user);
        }

        [Fact]
        public void GetUserByID_NonExistingUsername_ReturnNull()
        {
            string username = "aprilfool";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var user = repository.GetUserbyUsername(username);

            Assert.Null(user);
        }

        [Fact]
        public void CreateUser_ValidUser_ReturnUser()
        {
            string name = "May Nicolaos";
            string username = "maynicolaos";
            string password = "M@yNic01@0s";
            UserRoles role = UserRoles.User;
            UserStatus status = UserStatus.Active;

            var user = new User
            {
                Name = name,
                UserName = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                Status = status
            };

            var context = GetInMemoryDbContext(false);

            var repository = new UserRepository(context);

            var addedUser = repository.CreateUser(user);

            Assert.NotNull(addedUser);
            Assert.Equal(user.Name, addedUser.Name);
            Assert.Equal(user.UserName, addedUser.UserName);
            Assert.Equal(user.Role, addedUser.Role);
            Assert.Equal(user.Status, addedUser.Status);    
        }

        [Fact]
        public void CreateUser_NullUser_ReturnNull()
        {
            User user = null!;
            var context = GetInMemoryDbContext(false);
            var repository = new UserRepository(context);

            var addedUser = repository.CreateUser(user);

            Assert.Null(addedUser);  
        }

        [Fact]
        public void CreateUser_ExistingUser_ReturnNull()
        {
            string name = "Apple Mango";
            string username = "applemango";
            string password = "Appl3M@ng0";
            UserRoles role = UserRoles.User;
            UserStatus status = UserStatus.Active;

            var user = new User
            {
                Name = name,
                UserName = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                Status = status
            };

            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var addedUser = repository.CreateUser(user);

            Assert.Null(addedUser);   
        }

        [Fact]
        public void UsernameInUse_ValidUsernameSmallLetter_ReturnTrue()
        {
            string username = "applemango";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var result = repository.UsernameInUse(username);

            Assert.True(result);
        }       

        [Fact]
        public void UsernameInUse_ValidUsernameCapitalLetter_ReturnTrue()
        {
            string username = "APPLEMANGO";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var result = repository.UsernameInUse(username);

            Assert.True(result);
        }  

        [Fact]
        public void UsernameInUse_NullUsername_ReturnFalse()
        {
            string username = null!;
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var result = repository.UsernameInUse(username);

            Assert.False(result);
        }    

        [Fact]
        public void UsernameInUse_NonExistingUsername_ReturnFalse()
        {
            string username = "aprilpool";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var result = repository.UsernameInUse(username);

            Assert.False(result);
        }    

        [Fact]
        public void UsernameInUse_MissingUsername_ReturnFalse()
        {
            string username = "";
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var result = repository.UsernameInUse(username);

            Assert.False(result);
        }

        [Fact]
        public void ListUsers_UsersInDb_ReturnUsers()
        {
            var context = GetInMemoryDbContext(true);
            var repository = new UserRepository(context);

            var users = repository.ListUsers();

            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public void ListUsers_NoUsersInDb_ReturnEmptyList()
        {
            var context = GetInMemoryDbContext(false);
            var repository = new UserRepository(context);

            var users = repository.ListUsers();

            Assert.NotNull(users);
            Assert.Empty(users);
        }
    }
}