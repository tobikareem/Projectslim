using Slim.Core.Model;

namespace Slim.Shared.Interfaces.Repo;

public interface IBaseStore <T> where T: class
{
    void AddEntity(T entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
    void UpdateEntity(T entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
    T GetEntity(int id);
    IEnumerable<T> GetAll();
}