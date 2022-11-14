using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;

namespace Slim.Pages.Pages
{
    public class CartModel : PageModel
    {
        private readonly ICart<ShoppingCart> _shoppingCartBaseStore;
        private readonly IBaseStore<Product> _productStore;
        private readonly ILogger<CartModel> _logger;
        public CartModel(ICart<ShoppingCart> shopingCart, IBaseStore<Product> product, ILogger<CartModel> logger)
        {
            _shoppingCartBaseStore = shopingCart;
            _productStore = product;

            ShoppingCartUserId = string.Empty;
            _logger=logger;
        }

        private string ShoppingCartUserId { get; set; }
        public const string SessionKeyName = "CartUserId";
        public List<ShoppingCart> CartItems { get; set; } = new List<ShoppingCart>();

        public void OnGet(int? id)
        {
            ShoppingCartUserId = GetShoppingCartUserId(); 
            
            CartItems = _shoppingCartBaseStore.GetAll().Where(x => x.CartUserId == ShoppingCartUserId).ToList();
        }

        public void OnPostNewProductToCart(int id)
        {
            ShoppingCartUserId = GetShoppingCartUserId();

            var cartItem = _shoppingCartBaseStore.GetCartUserItem(ShoppingCartUserId, id);

            if (cartItem == default(ShoppingCart))
            {
                _logger.LogInformation("... Adding new item to Shopping Cart");

                cartItem = new ShoppingCart
                {
                    Quantity = 1,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ShoppingCartUserId,
                    Product = _productStore.GetEntity(id),
                    CartUserId = ShoppingCartUserId
                };

                _shoppingCartBaseStore.AddEntity(cartItem, Core.Model.CacheKey.GetShoppingCartItem, true);
                return;
            }

            cartItem.Quantity++;
            cartItem.ModifiedDate = DateTime.UtcNow;

            _shoppingCartBaseStore.UpdateEntity(cartItem, Core.Model.CacheKey.GetShoppingCartItem, true);
            _logger.LogInformation("... Shopping Product is updated to cart by user {cartUser}", ShoppingCartUserId);
        }

        private string GetShoppingCartUserId()
        {
            var hasSession = HttpContext.Session.GetString(SessionKeyName);
            if (string.IsNullOrWhiteSpace(hasSession))
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity?.Name))
                {
                    HttpContext.Session.SetString(SessionKeyName, HttpContext.User.Identity.Name);
                }
                else
                {
                    var tempCartId = Guid.NewGuid();
                    HttpContext.Session.SetString(SessionKeyName, tempCartId.ToString());
                }
            }

            var sessionName = HttpContext.Session.GetString(SessionKeyName);

            return string.IsNullOrEmpty(sessionName) ? string.Empty : sessionName.ToString();

        }
    }
}
