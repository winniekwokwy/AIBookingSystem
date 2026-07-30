using AIBookingSystem.Models;
namespace AIBookingSystem.Services
{
    public interface IClientCacheService
    {
        // Async method: fetch from cache or DB if missing and update cache
        Task<Client?> GetClientByClientId(string clientId);
    }
}