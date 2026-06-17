using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public interface IUserService
    {
        IEnumerable<UserDTO>? ListUsers();
        UserDTO? GetUserbyID(int ID);
        UserDTO? GetUserbyUsername(string userName);
        UserDTO? CreateUser(UserCreateDTO user);
        bool IsUserValid(UserCreateDTO userDTO);
        bool IsRoleValid(string role);
        bool UsernameExsited(string username);
    }
}