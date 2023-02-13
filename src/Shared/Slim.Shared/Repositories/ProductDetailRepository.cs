using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories;

public class ProductDetailRepository : IBaseStore<ProductDetail>
{
    private readonly SlimDbContext _context;
    private readonly ILogger<ProductDetailRepository> _logger;
    private readonly ICacheService _cacheService;


    public ProductDetailRepository(SlimDbContext context, ILogger<ProductDetailRepository> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    public void AddEntity(ProductDetail entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            //  _context.Entry(typeof(Image)).State = EntityState.Detached;
            _context.ProductDetails.Add(entity);
            _context.SaveChanges();

            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Adding Product Detail Entity");
            throw;
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(CacheKey.GetProductDetails);
            }
        }
    }

    public void UpdateEntity(ProductDetail entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ProductDetails.Update(entity);
            _context.SaveChanges();

            if (hasCache)
            {
                _cacheService.Remove(cacheKey);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Updating Product Detail Entity");
            throw;
        }
        finally
        {
            if (hasCache)
            {
                _cacheService.Remove(CacheKey.GetProductDetails);
            }
        }
    }

    public ProductDetail GetEntity(int id)
    {
        try
        {
            return GetAll().FirstOrDefault(x => x.Id == id) ?? throw new Exception("Product Detail not found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Get Product Detail Entity");
            throw;
        }
    }

    public IEnumerable<ProductDetail> GetAll()
    {
        return _context.ProductDetails.ToList();
    }

    public void DeleteEntity(ProductDetail entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {

        try
        {
            _context.ProductDetails.Remove(entity);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in Delete ProductDetails Entity");
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