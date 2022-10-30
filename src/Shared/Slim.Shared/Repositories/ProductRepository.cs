using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class ProductRepository : IBaseStore<Product>
    {
        private readonly SlimDbContext _context;
        private readonly ILogger<ProductRepository> _logger;
        private readonly ICacheService _cacheService;

        public ProductRepository(SlimDbContext context, ILogger<ProductRepository> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }
        
        public void AddEntity(Product entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                //  _context.Entry(typeof(Image)).State = EntityState.Detached;
                _context.Products.Add(entity);
                _context.SaveChanges();

                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Add Product Entity");
                throw;
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(CacheKey.GetProducts);
                }
            }
        }

        public void UpdateEntity(Product entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Products.Update(entity);
                _context.SaveChanges();

                if (hasCache)
                {
                    _cacheService.Remove(cacheKey);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Update Product Entity");
                throw;
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(CacheKey.GetProducts);
                }
            }
        }

        public Product GetEntity(int id)
        {
            try
            {
                return GetAll().FirstOrDefault(x => x.Id == id) ?? throw new Exception("Product not found");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Get Product Entity");
                throw;
            }
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                return _context.Products.Include(x => x.Images)
                    .Include(x => x.Category)
                    .Include(y => y.Comments)
                    .Include(d => d.Reviews).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Get All Product Entities");
                throw;
            }
        }

        public void DeleteEntity(Product entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Products.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Delete Product Entity");
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
