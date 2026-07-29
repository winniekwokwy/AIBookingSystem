using AIBookingSystem.Data;
using AIBookingSystem.Models;
using AIBookingSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
namespace AIBookingSystem.Services
{
    public class ClientCacheService : IClientCacheService
    {
        // Prefix to uniquely identify each client entry in the cache
        private const string CacheKeyPrefix = "Client_";
        // Service provider to create scopes for scoped services like DbContext
        private readonly IServiceProvider _serviceProvider;
        // Memory cache instance for storing cached client data in-memory
        private readonly IMemoryCache _memoryCache;

        // Constructor injects required services: IServiceProvider and IMemoryCache
        public ClientCacheService(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }
        // Gets a Client entity by ClientId asynchronously.
        // First attempts to get the client from in-memory cache.
        // If the client is not found in cache, fetches from the database,
        // caches the client, then returns it.
        public async Task<Client?> GetClientByClientId(string clientId)
        {
            // Construct cache key for this client using prefix and clientId
            var cacheKey = CacheKeyPrefix + clientId;
            // Attempt to retrieve the client from in-memory cache
            if (_memoryCache.TryGetValue<Client>(cacheKey, out var client))
            {
                // Cache hit - return the cached client immediately
                return client;
            }
            // Cache miss - resolve the repository from a child scope so it can use the scoped DbContext safely.
            using var scope = _serviceProvider.CreateScope();
            var clientCacheRepository = scope.ServiceProvider.GetRequiredService<IClientCacheRepository>();
            client = await clientCacheRepository.GetClientByClientId(clientId);
            if (client != null)
            {
                // Store the retrieved client into cache with expiration policy
                // Here it expires 1 hour after being added to cache; adjust as needed
                _memoryCache.Set(cacheKey, client, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }
            // Return the client entity (null if not found in DB)
            return client;
        }
    }
}