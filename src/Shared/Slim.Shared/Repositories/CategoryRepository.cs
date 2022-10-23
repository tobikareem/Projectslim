using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories;

public class CategoryRepository: IBaseStore<Category>
{
    private readonly SlimDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;
    private readonly ICacheService _cacheService;
    public CategoryRepository(SlimDbContext context, ILogger<CategoryRepository> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }
    public void AddEntity(Category entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Categories.Add(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding Category entity");
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
    }

    public void UpdateEntity(Category entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Categories.Update(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Category entity");
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
    }

    public Category GetEntity(int id)
    {
        return _context.Categories.Find(id) ?? new Category();
    }

    public IEnumerable<Category> GetAll()
    {
        var categories = _context.Categories.ToList();

        _cacheService.Add(CacheKey.ProductCategories, categories, 60);
        return categories;
    }

    public void DeleteEntity(Category entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Categories.Remove(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting Category entity");
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
    }
}