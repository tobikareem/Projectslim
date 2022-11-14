using Slim.Core.Model;
using Slim.Data.Entity;

namespace Slim.Shared.Interfaces.Repo;

public interface IBaseStore <T> where T: class
{
    void AddEntity(T entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
    void UpdateEntity(T entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
    T GetEntity(int id);
    IEnumerable<T> GetAll();
    void DeleteEntity(T entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
}

public interface IPageSection<T> : IBaseStore<PageSection>
{
    void UpdatePageSections(List<PageSection> pageSections, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
}

public interface IImage<T> : IBaseStore<Image>
{
    void UpdateImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false);

    void AddImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false);

    void DeleteImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
}

public interface ICart<T>: IBaseStore<ShoppingCart>
{
    T GetCartUserItem(string cartUserId, int productId, CacheKey cacheKey = CacheKey.None, bool hasCache = false);
}