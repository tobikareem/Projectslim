using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PaymentModel> _logger;
        private string ShoppingCartUserId { get; set; }

        public PaymentModel(ICartService cartService, UserManager<IdentityUser> userManager, ILogger<PaymentModel> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;

            ShoppingCartUserId = string.Empty;
        }
        public List<ShoppingCart> CartItems { get; set; } = new();
        public decimal TotalCartPrice { get; set; }


        public void OnGet()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems = _cartService.GetCartItemsForUser(loggedInUser, ShoppingCartUserId);
            TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);
        }


        private string GetShoppingCartUserId()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (HttpContext == null)
            {
                return string.Empty;
            }

            var hasSession = HttpContext.Session.GetString(SlmConstant.SessionKeyName);
            if (string.IsNullOrWhiteSpace(hasSession))
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity?.Name))
                {
                    HttpContext.Session.SetString(SlmConstant.SessionKeyName, HttpContext.User.Identity.Name);
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
