using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;

namespace Slim.Pages.Pages
{
    public class CartModel : PageModel
    {
        private readonly IBaseStore<ShoppingCart> _shoppingCartBaseStore;
        private readonly ILogger<CartModel> _logger;
        public CartModel(IBaseStore<ShoppingCart> shopingCart, ILogger<CartModel> logger)
        {
            _shoppingCartBaseStore = shopingCart;

            ShoppingCartId = string.Empty;
            _logger=logger;
        }

        public string ShoppingCartId { get; set; }
        public const string SessionKeyName = "CartUserId";

        public void OnGet(int? id)
        {
        }

        public void OnPostNewProductToCart(int? id)
        {
            ShoppingCartId = GetShoppingCartId();

            _logger.LogInformation("... Shopping CartId is {cart}", ShoppingCartId);

        }

        private string GetShoppingCartId()
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
