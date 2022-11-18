using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class PageSectionRepo : IPageSection
    {
        private readonly SlimDbContext _context;
        private readonly ILogger<PageSectionRepo> _logger;
        private readonly ICacheService _cacheService;
        public PageSectionRepo(SlimDbContext context, ILogger<PageSectionRepo> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        public void UpdatePageSections(List<PageSection> pageSections, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.PageSections.UpdateRange(pageSections);
                _context.SaveChanges(true);

                if (cacheKey != CacheKey.None)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddEntity(PageSection entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity(PageSection entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Attach(entity).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to update Page Section: {message}", e.Message);
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
        }

        public PageSection GetEntity(int id)
        {
            try
            {
                var pageSection = _context.PageSections.FirstOrDefault(x => x.RazorPageId == id);
                return pageSection ?? new PageSection();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to get Page Section: {message}", e.Message);
                throw;
            }
        }

        public IEnumerable<PageSection> GetAll()
        {
            return _context.PageSections.ToList();
        }

        public void DeleteEntity(PageSection entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.PageSections.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to delete Page Section: {message}", e.Message);
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
