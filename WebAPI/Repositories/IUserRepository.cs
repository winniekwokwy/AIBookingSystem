using AIBookingSystem.DTO;

namespace AIBookingSystem.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User>? ListUsers();
        User? GetUserbyID(int ID);
        User? GetUserbyUsername(string userName);
        User? CreateUser(User user);
        bool IsUserValid(int UserId, string createdBy);
        bool UsernameInUse(string username);
    }
}