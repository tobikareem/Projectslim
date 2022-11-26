using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Serv;
using Slim.Shared.Services;

namespace Slim.Pages.Pages
{
    public class ShippingModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ShippingModel> _logger;
        private readonly IUserService _userService;
        private string ShoppingCartUserId { get; set; }

        public ShippingModel(ICartService cartService, UserManager<IdentityUser> userManager, ILogger<ShippingModel> logger, IUserService userService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;

            ShoppingCartUserId = string.Empty;
            _userService = userService;
        }
        public List<ShoppingCart> CartItems { get; set; } = new();
        public decimal TotalCartPrice { get; set; }


        public async Task<IActionResult> OnGet()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems = _cartService.GetCartItemsForUser(loggedInUser, ShoppingCartUserId);
            TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);

            var userInfo = await _userService.LoadUserAddressInformationAsync(User);

            if (!userInfo.nullOrEmptyProperties.Any())
            {
                return Page();
            }

            var essentials = SlmConstant.EssentialAddressModel;

            if (!userInfo.addressModel.IsSameAsAddress)
            {
                essentials.AddRange(SlmConstant.EssentialBillingAddressModel);
            }

            if(essentials.Any(x => userInfo.nullOrEmptyProperties.Contains(x)))
            {
                return RedirectToPage("/Checkout", new { isRedirect = true });
            }

            return Page();

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