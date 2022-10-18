using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class RazorPagesRepo : IBaseStore<RazorPage>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<RazorPagesRepo> _logger;
        private readonly SlimDbContext _context;
        
        public RazorPagesRepo(ICacheService cacheService, ILogger<RazorPagesRepo> logger, SlimDbContext context)
        {
            _cacheService = cacheService;
            _logger = logger;
            _context = context;
        }
        
        public void AddEntity(RazorPage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }

                _context.RazorPages.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to add new Razor Page: {message}", e.Message);
                throw;
            }
            
        }

        public void UpdateEntity(RazorPage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            throw new NotImplementedException();
        }

        public RazorPage GetEntity(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorPage> GetAll()
        {
            try
            {
                var pages = _context.RazorPages.ToList();
                return pages;
            }
            catch (Exception e)
            {
                _logger.LogError("Unable read all item: {message}", e.Message);
                throw;
            }
        }

        public void DeleteEntity(RazorPage entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.RazorPages.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to delete Razor Page: {message}", e.Message);
                throw;
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
