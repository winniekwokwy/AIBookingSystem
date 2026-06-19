using AIBookingSystem.DTO;
using AIBookingSystem.Enums;

namespace AIBookingSystem.Services
{
    public interface IUserService
    {
        IEnumerable<UserDTO>? ListUsers();
        UserDTO? GetUserbyID(int ID);
        UserDTO? GetUserbyUsername(string userName);
        UserDTO? CreateUser(UserCreateDTO user);
        bool IsRoleValid(string role);
        string? StatusMappingEnum2String(UserStatus status);
        UserStatus StatusMappingString2Enum(string status);
        bool IsStatusValid(UserStatus status);
        string? RoleMappingEnum2String(UserRoles role);
        UserRoles RoleMappingString2Enum(string role);
        bool IsRoleValid(UserRoles role);
    }
}