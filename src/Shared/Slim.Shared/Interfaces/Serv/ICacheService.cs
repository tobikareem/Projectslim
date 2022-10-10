using Slim.Core.Model;

namespace Slim.Shared.Interfaces.Serv;

public interface ICacheService
{
    void Add<T>(CacheKey key, T item, int duration);

    T GetItem<T>(CacheKey key);

    bool Contains(CacheKey key);

    void Remove(CacheKey key);

    T GetOrCreate<T>(CacheKey key, Func<T> createItem, int duration = 0);
}