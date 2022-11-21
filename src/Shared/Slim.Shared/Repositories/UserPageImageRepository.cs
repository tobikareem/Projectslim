using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class UserPageImageRepository: IBaseStore<UserPageImage>
    {
        private readonly SlimDbContext _context;
        private readonly ILogger<UserPageImageRepository> _logger;
        private readonly ICacheService _cacheService;

        public UserPageImageRepository(SlimDbContext context, ILogger<UserPageImageRepository> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }
        
        public void AddEntity(UserPageImage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.UserPageImages.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding UserPageImage entity");
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
        }

        public void UpdateEntity(UserPageImage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.UserPageImages.Update(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating UserPageImage entity");
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
        }

        public UserPageImage GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserPageImage> GetAll()
        {
            try
            {
                return _context.UserPageImages.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting UserPageImage entity");
                throw;
            }
        }

        public void DeleteEntity(UserPageImage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.UserPageImages.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting UserPageImage entity");
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
}
