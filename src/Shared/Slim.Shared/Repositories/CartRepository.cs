
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class CartRepository : IBaseStore<ShoppingCart>
    {
        private readonly SlimDbContext _context;
        private readonly ILogger<CartRepository> _logger;
        private readonly ICacheService _cacheService;

        public const string CacheKey = "CartUserId";
        public string ShoppingCartId { get; set; }

        public CartRepository(SlimDbContext context, ILogger<CartRepository> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;

            ShoppingCartId = string.Empty;
        }

        public void AddEntity(ShoppingCart entity, CacheKey cacheKey = Slim.Core.Model.CacheKey.None, bool hasCache = false)
        {
            try
            {
               


            }
            catch (Exception ex)
            {
                _logger.LogError("... Error in Adding Entity {message}", ex.Message);
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


        public void DeleteEntity(ShoppingCart entity, CacheKey cacheKey = Slim.Core.Model.CacheKey.None, bool hasCache = false)
        {
            try
            {

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

                return new List<ShoppingCart>();
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

        public void UpdateEntity(ShoppingCart entity, CacheKey cacheKey = Slim.Core.Model.CacheKey.None, bool hasCache = false)
        {
            try
            {

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
    }
}
