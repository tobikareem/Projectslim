using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;

namespace Slim.Pages.Pages
{
    public class CompleteOrderModel : PageModel
    {
        private readonly ILogger<CompleteOrderModel> _logger;
        private readonly IBaseCart<ShoppingCart> _baseCart;

        public CompleteOrderModel(IBaseCart<ShoppingCart> baseCart, ILogger<CompleteOrderModel> logger)
        {
            _baseCart=baseCart;
            _logger=logger;
        }

        public void OnGet()
        {

            var cartUser = User.Identity?.Name ?? GetCartUserId();
            var userCartItems = _baseCart.GetAllCartItemsByUserId(cartUser);
            _baseCart.DeleteAllCartItems(userCartItems, CacheKey.GetShoppingCartItem, true);

        }

        private string GetCartUserId()
        {
            var hasSession = HttpContext.Session.GetString(SlmConstant.SessionKeyName);
            if (string.IsNullOrWhiteSpace(hasSession))
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity?.Name))
                {
                    HttpContext.Session.SetString(SlmConstant.SessionKeyName, HttpContext.User.Identity.Name);
                    return HttpContext.User.Identity.Name;
                }
                else
                {
                    var tempCartId = Guid.NewGuid();
                    HttpContext.Session.SetString(SlmConstant.SessionKeyName, tempCartId.ToString());
                }
            }

            var sessionName = HttpContext.Session.GetString(SlmConstant.SessionKeyName);

            return string.IsNullOrEmpty(sessionName) ? string.Empty : sessionName;
        }
    }
}
