using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using System.Globalization;

namespace Slim.Shared.Services
{
    public class CartService : ICartService
    {
        private readonly ICacheService _cacheService;
        private readonly IBaseCart<ShoppingCart> _shoppingCartBaseStore;

        public CartService(IBaseCart<ShoppingCart> shoppingCartBaseStore, ICacheService cacheService)
        {
            _shoppingCartBaseStore = shoppingCartBaseStore;
            _cacheService = cacheService;
        }

        public List<ShoppingCart> GetCartItemsForUser(string loggedInUser, string defaultSessionUser)
        {

            var allCarts = _cacheService.GetOrCreate(CacheKey.GetShoppingCartItem, _shoppingCartBaseStore.GetAll).ToList();

            var cartItems = allCarts.Where(x => x.CartUserId == defaultSessionUser).ToList();

            if (string.IsNullOrWhiteSpace(loggedInUser) || string.Equals(loggedInUser, defaultSessionUser, StringComparison.OrdinalIgnoreCase))
            {
                return cartItems;
            }

            var more = allCarts.Where(x => x.CartUserId == loggedInUser).ToList();
            cartItems.AddRange(more);
            return cartItems;
        }

        public decimal GetTotalCartPrice(string loggedInUser, string defaultSessionUser)
        {
            var cartItems = GetCartItemsForUser(loggedInUser, defaultSessionUser);
            return cartItems.Sum(c =>
            {
                if (c.Product != null)
                {
                    return c.Product is { IsOnSale: true }
                        ? c.Product.SalePrice * c.Quantity

                        : c.Product.StandardPrice * c.Quantity;
                }

                return 0.0m;
            });
        }

        public (int StandardWholePrice, string StandardPriceRoundUp, int SalesWholePrice, string SalesPriceRoundUp)  GetPriceForProduct( decimal standardPrice, decimal salesPrice)
        {
            var standardPriceRoundUp = Convert.ToString(standardPrice, CultureInfo.CurrentCulture).Split('.')[1];
            var standardPriceWholePrice = Convert.ToInt32(Math.Truncate(standardPrice));
            
            var salesPriceRoundUp = Convert.ToString(salesPrice, CultureInfo.CurrentCulture).Split('.')[1];
            var salesPriceWholePrice = Convert.ToInt32(Math.Truncate(salesPrice));

            return (standardPriceWholePrice, standardPriceRoundUp, salesPriceWholePrice, salesPriceRoundUp);
        }

        public List<Product> GetProductsWithInCartCheck(IEnumerable<Product> products, string loggedInUser, string defaultSessionUser)
        {
            var cartItems = GetCartItemsForUser(loggedInUser, defaultSessionUser);
            var productIds = cartItems.Select(x => x.ProductId).ToList();
            var productsWithInCartCheck = products.Select(x =>
            {
                x.IsProductInCart = productIds.Contains(x.Id);
                return x;
            }).ToList();

            return productsWithInCartCheck;
        }
    }
}
