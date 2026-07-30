using AIBookingSystem.Models;

namespace AIBookingSystem.Repositories
{
    public interface IClientCacheRepository
    {
        Task<Client?> GetClientByClientId(string clientId);
    }
}