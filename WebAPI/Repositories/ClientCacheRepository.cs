using AIBookingSystem.Data;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AIBookingSystem.Repositories
{
    public class ClientCacheRepository : IClientCacheRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public ClientCacheRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

    public async Task<Client?> GetClientByClientId(string clientId)
        {
            return _dBContext.Clients
                 .FirstOrDefault(c => c.ClientId == clientId && c.IsActive);
        }
    }
}
