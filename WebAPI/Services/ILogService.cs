using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public interface ILogService
    {
        bool AddUserChangeLog(UserCreateDTO user);
    }
}