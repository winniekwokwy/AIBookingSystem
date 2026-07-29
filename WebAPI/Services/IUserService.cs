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

        bool IsStatusValid(UserStatus status);

        string? StatusMappingEnum2String(UserStatus status);

        UserStatus StatusMappingString2Enum(string status);

        bool IsUserValid(int id, string username);

        Task<AuthResponseDTO?> AuthenticateUser(UserLoginDTO loginDto, string ipAddress);
        Task<AuthResponseDTO?> RefreshToken(string refreshToken, string clientId, string ipAddress);
        bool RevokeRefreshToken(string refreshToken);

    }
}