using Slim.Core.Model;

namespace Slim.Shared.Interfaces.Serv;

public interface ICacheService
{
    /// <summary>
    /// Add an Item to cache
    /// </summary>
    /// <typeparam name="T">The type to add</typeparam>
    /// <param name="key">CacheKey</param>
    /// <param name="item">actual item to add</param>
    /// <param name="duration">int number will be converted to minutes</param>
    void Add<T>(CacheKey key, T item, int duration);

    T GetItem<T>(CacheKey key);

    bool Contains(CacheKey key);

    void Remove(CacheKey key);

    T GetOrCreate<T>(CacheKey key, Func<T> createItem, int duration = 0);
}