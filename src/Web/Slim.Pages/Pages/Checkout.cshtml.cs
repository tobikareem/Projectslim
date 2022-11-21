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


        public async Task OnGet()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems = _cartService.GetCartItemsForUser(loggedInUser, ShoppingCartUserId);
            TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);
            await LoadUserInformationAsync();
        }

        public async Task<IActionResult> OnPostRedirectToShipping()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            if (!ModelState.IsValid)
            {
                TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);
                await LoadUserInformationAsync();
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

            if (!string.IsNullOrWhiteSpace(Input.Address1))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, ClaimTypes.StreetAddress, Input.Address1);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Address1.";
                    _logger.LogWarning("Unexpected error when trying to set Address1.");
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.Address2))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.Address2, Input.Address2);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Address2.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.ZipCode))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.Zipcode, Input.ZipCode);
                if (!isSuccess)
                {
                    StatusMessage = "Unexpected error when trying to set Zipcode.";
                    return RedirectToPage();
                }
            }

            var isSame = await _userService.UpsertUserClaim(user, CustomClaims.IsSameAsAddress, Input.IsSameAsAddress.ToString());
            if (!isSame)
            {
                StatusMessage = "Unable to select this option for same address";
                return RedirectToPage();
            }

            if (!string.IsNullOrWhiteSpace(Input.BillingAddress1))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingAddress1, Input.BillingAddress1);
                if (!isSuccess)
                {
                    StatusMessage = "Unable to replace Billing Address1.";
                    return RedirectToPage();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.BillingAddress2))
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingAddress2, Input.BillingAddress2);
                if (!isSuccess)
                {
                    StatusMessage = "Unable to replace Billing Address2.";
                    return RedirectToPage();
                }
            }

            if (string.IsNullOrWhiteSpace(Input.BillingZipCode)) return RedirectToPage("/Shipping");
            {
                var isSuccess = await _userService.UpsertUserClaim(user, CustomClaims.BillingZipCode, Input.BillingZipCode);
                if (isSuccess) return RedirectToPage("/Shipping");
                StatusMessage = "Unable to replace Billing Zip code.";
                return RedirectToPage();
            }

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

        private async Task LoadUserInformationAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userClaims = await _userManager.GetClaimsAsync(user);
            bool.TryParse(userClaims.FirstOrDefault(x => x.Type == nameof(Input.ZipCode))?.Value, out var isSame);
            Input = new AddressModel
            {
                FirstName = User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Skip(1).FirstOrDefault()?.Value ?? string.Empty,
                LastName = User.FindFirstValue(ClaimTypes.Surname),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsSameAsAddress = isSame,
                Address1 = User.FindFirstValue(ClaimTypes.StreetAddress),
                Address2 = User.FindFirstValue(CustomClaims.Address2),
                ZipCode = User.FindFirstValue(CustomClaims.Zipcode),

                BillingAddress1 = User.FindFirstValue(CustomClaims.BillingAddress1),
                BillingAddress2 = User.FindFirstValue(CustomClaims.BillingAddress2),
                BillingZipCode = User.FindFirstValue(CustomClaims.BillingZipCode),


            };
        }
        
    }
}
