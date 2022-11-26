
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories;
public class CartRepository : IBaseCart<ShoppingCart>
{
    private readonly SlimDbContext _context;
    private readonly ILogger<CartRepository> _logger;
    private readonly ICacheService _cacheService;
    public string ShoppingCartId { get; set; }

    public CartRepository(SlimDbContext context, ILogger<CartRepository> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;

        ShoppingCartId = string.Empty;
    }

    public void AddEntity(ShoppingCart entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ShoppingCarts.Add(entity);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error occurred while adding new item to Shopping Cart {message}", ex.Message);
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

    public void DeleteEntity(ShoppingCart entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ShoppingCarts.Remove(entity);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error in Deleting Entity {message}", ex.Message);
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

    public IEnumerable<ShoppingCart> GetAll()
    {
        try
        {
            return _context.ShoppingCarts.Include(x => x.Product)
                .Include(x => x.Product!.Images)
                .Include(x => x.Product!.Category)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error in Getting all Entity {message}", ex.Message);
            throw;
        }
    }

    public ShoppingCart GetEntity(int id)
    {
        try
        {
            return new ShoppingCart();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error in Getting Entity {message}", ex.Message);
            throw;
        }
    }

    public void UpdateEntity(ShoppingCart entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ShoppingCarts.Update(entity);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error in Updating Entity {message}", ex.Message);
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

    public ShoppingCart GetCartUserItem(string cartUserId, int productId, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            var cartUser = _context.ShoppingCarts.SingleOrDefault(x => x.CartUserId == cartUserId && x.ProductId == productId);
            return cartUser ?? new ShoppingCart();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error getting cart item for user {user}, {error}", cartUserId, ex);
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

    public void UpdateCartItems(List<ShoppingCart> cartItems, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ShoppingCarts.UpdateRange(cartItems);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError("... Error in Updating Entities {message}", ex.Message);
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

    public List<ShoppingCart> GetAllCartItemsByUserId(string cartUserId)
    {
        return _context.ShoppingCarts.Where(x => x.CartUserId == cartUserId).ToList();
    }

    public void DeleteAllCartItems(List<ShoppingCart> cartItems, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
    {
        try
        {
            _context.ShoppingCarts.RemoveRange(cartItems);
            _context.SaveChanges(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred trying to delete all cart items {message}", ex.Message);
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