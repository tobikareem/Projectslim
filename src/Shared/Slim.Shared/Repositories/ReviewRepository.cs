

using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories;

public class ReviewRepository : IBaseStore<Review>
{
    private readonly SlimDbContext _context;
    private readonly ILogger<ReviewRepository> _logger;
    private readonly ICacheService _cacheService;

    public ReviewRepository(SlimDbContext context, ILogger<ReviewRepository> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    public void AddEntity(Review entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Reviews.Add(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding Review entity");
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
    }

    public void UpdateEntity(Review entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Reviews.Update(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Review entity");
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
    }

    public Review GetEntity(int id)
    {
        try
        {
            var entity = GetAll().FirstOrDefault(x => x.Id == id);
            return entity ?? new Review();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting Review entity by id");
            return new Review();
        }
    }

    public IEnumerable<Review> GetAll()
    {
        try
        {
            return _context.Reviews.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DeleteEntity(Review entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.Reviews.Remove(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting Review entity");
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