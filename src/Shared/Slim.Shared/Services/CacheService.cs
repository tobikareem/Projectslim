using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> cacheLogger)
        {
            _cache = cache;
            _logger = cacheLogger;
        }

        public void Add<T>(CacheKey key, T item, int duration)
        {
            _cache.Set(key, item, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration == default ? TimeSpan.FromHours(1) : TimeSpan.FromMinutes(duration)
            });
        }

        public T GetItem<T>(CacheKey key)
        {
            return _cache.Get<T>(key);
        }

        private T Get<T>(CacheKey key)
        {
            return _cache.Get<T>(key);
        }

        public bool Contains(CacheKey key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Remove(CacheKey key)
        {
            _cache.Remove(key);
        }

        public T GetOrCreate<T>(CacheKey key, Func<T> createItem, int duration)
        {
            if (Contains(key))
            {
                return Get<T>(key);
            }

            _logger.Log(LogLevel.Information, "{cacheKey} Item not in cache with {duration} duration", key, duration);
            var item = createItem();

            Add(key, item, duration);
            return item;
        }
    }
}
