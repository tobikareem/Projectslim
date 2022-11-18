using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class ImageUploadRepo :IBaseImage
    {

        private readonly SlimDbContext _context;
        private readonly ILogger<ImageUploadRepo> _logger;
        private readonly ICacheService _cacheService;
        public ImageUploadRepo(SlimDbContext context, ILogger<ImageUploadRepo> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }
        public void AddEntity(Image entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                //_context.Entry(typeof(Product)).State = EntityState.Detached;
                _context.Images.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding Image entity");
                throw;
            }
        }

        public void UpdateEntity(Image entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Images.Update(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating Image entity");
                throw;
            }
        }

        public Image GetEntity(int id)
        {
            try
            {
                return _context.Images.FirstOrDefault(x => x.Id == id) ?? new Image();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting Image entity");
                throw;
            }
        }

        public IEnumerable<Image> GetAll()
        {
            try
            {
                return _context.Images.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting all Image entities");
                throw;
            }
        }

        public void DeleteEntity(Image entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Images.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting Image entity");
                throw;
            }
            finally
            {
                if(hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
        }

        public void UpdateImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Images.UpdateRange(images);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating Image entities");
                throw;
            }
        }

        public void AddImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
               // _context.Entry(typeof(Image)).State = EntityState.Detached;
                _context.Images.AddRange(images);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding Image entities");
                throw;
            }
        }

        public void DeleteImages(List<Image> images, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Images.RemoveRange(images);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting Image entities");
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
