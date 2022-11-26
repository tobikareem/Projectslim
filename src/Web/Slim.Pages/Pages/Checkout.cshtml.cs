using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ILogger<CheckoutModel> _logger;
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutModel(IUserService userService, ILogger<CheckoutModel> logger, ICartService cartService, UserManager<IdentityUser> userMgr)
        {
            _userService = userService;
            _logger = logger;
            _cartService = cartService;
            _userManager = userMgr;

            ShoppingCartUserId = string.Empty;
        }

        private string ShoppingCartUserId { get; set; }
        public List<ShoppingCart> CartItems { get; set; } = new();
        public decimal TotalCartPrice { get; set; }

        [BindProperty] public AddressModel Input { get; set; } = new();
        [TempData] public string StatusMessage { get; set; } = string.Empty;


        public async Task OnGet(bool isRedirect = false)
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems = _cartService.GetCartItemsForUser(loggedInUser, ShoppingCartUserId);
            TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);

            var userInfo = await _userService.LoadUserAddressInformationAsync(User);
            Input = userInfo.addressModel;

            StatusMessage = String.Empty;

            if (isRedirect)
            {
                var essentials = SlmConstant.EssentialAddressModel;

                if (!userInfo.addressModel.IsSameAsAddress)
                {
                    essentials.AddRange(SlmConstant.EssentialBillingAddressModel);
                }

                if (essentials.Any(x => userInfo.nullOrEmptyProperties.Contains(x)))
                {
                    foreach (var prop in essentials)
                    {
                        ModelState.AddModelError(prop, $"The {prop} field is required.");
                    }
                    StatusMessage = "Error. Please, fill in required information";
                }

            }


        }

        public async Task<IActionResult> OnPostRedirectToShipping()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();

            if (!ModelState.IsValid)
            {
                TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);

                var userInfo = await _userService.LoadUserAddressInformationAsync(User);
                Input = userInfo.addressModel;

                StatusMessage = "Missing some User Information";
                _logger.LogWarning("Missing some User Information");
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (phoneNumber != Input.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    _logger.LogWarning("Unexpected error when trying to set phone number.");
                    return RedirectToPage();
                }
            }

            foreach (var property in Input.GetType().GetProperties())
            {
                var claimType = property.Name;

                if (claimType == nameof(Input.PhoneNumber))
                {
                    continue;
                }

                var value = property.GetValue(Input);
                if (value != null)
                {

                    if (claimType == nameof(Input.Address1))
                    {
                        claimType = ClaimTypes.StreetAddress;
                    }

                    var claimValue = value.ToString();

                    var isSuccess = await _userService.UpsertUserClaim(user, claimType, claimValue ?? string.Empty);
                    if (!isSuccess)
                    {
                        StatusMessage = $"Unexpected error when trying to set {nameof(property.Name)}";
                        _logger.LogWarning($"Unexpected error when trying to set {nameof(property.Name)}.");
                        return RedirectToPage();
                    }
                }
                else
                {
                    var claim = User.FindFirst(claimType);
                    if (claim != null)
                    {
                        _ = await _userManager.RemoveClaimAsync(user, claim);
                    }

                }
            }

            return RedirectToPage("/Shipping");

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
