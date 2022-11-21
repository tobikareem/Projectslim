using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly IBaseCart<ShoppingCart> _shoppingCartBaseStore;
        private readonly IBaseStore<Product> _productStore;
        private readonly ILogger<CheckoutModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;
        public CheckoutModel(IBaseCart<ShoppingCart> shoppingCartBaseStore, IBaseStore<Product> productStore, ILogger<CheckoutModel> logger, ICacheService cacheService, ICartService cartService, UserManager<IdentityUser> userMgr)
        {
            _shoppingCartBaseStore = shoppingCartBaseStore;
            _productStore = productStore;
            _logger = logger;
            _cacheService = cacheService;
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
                    return RedirectToPage();
                }
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            foreach (var userClaim in userClaims)
            {
                if (userClaim.Type == ClaimTypes.StreetAddress && Input.Address1 != userClaim.Value)
                {
                    var oldClaim = new Claim(ClaimTypes.StreetAddress, userClaim.Value);
                    var newClaim = new Claim(ClaimTypes.StreetAddress, Input.Address1);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = $"Unable to replace {nameof(Input.Address1)}";
                        return RedirectToPage();
                    }
                }

                if (userClaim.Type == nameof(Input.Address2) && Input.Address2 != userClaim.Value)
                {
                    var oldClaim = new Claim(nameof(Input.Address2), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.Address2), Input.Address2);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = $"Unable to replace {nameof(Input.Address2)}";
                        return RedirectToPage();
                    }
                }

                if (userClaim.Type ==  nameof(Input.ZipCode) && Input.ZipCode != userClaim.Value)
                {
                    var oldClaim = new Claim(nameof(Input.ZipCode), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.ZipCode), Input.ZipCode);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = $"Unable to replace {nameof(Input.ZipCode)}";
                        return RedirectToPage();
                    }
                }

                if (userClaim.Type == nameof(Input.IsSameAsAddress) && Input.IsSameAsAddress != Convert.ToBoolean(userClaim.Value))
                {
                    var oldClaim = new Claim(nameof(Input.IsSameAsAddress), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.IsSameAsAddress), Input.IsSameAsAddress.ToString());
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = $"Unable to select this option for same address";
                        return RedirectToPage();
                    }
                }

                // Billing Address
                if (userClaim.Type == nameof(Input.BillingAddress1) && Input.BillingAddress1 != userClaim.Value)
                {
                    var oldClaim = new Claim(nameof(Input.BillingAddress1), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.BillingAddress1), Input.BillingAddress1);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = "Unable to replace Billing Address1";
                        return RedirectToPage();
                    }
                }

                if (userClaim.Type == nameof(Input.BillingAddress2) && Input.BillingAddress2 != userClaim.Value)
                {
                    var oldClaim = new Claim(nameof(Input.BillingAddress2), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.BillingAddress2), Input.BillingAddress2);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = "Unable to replace Billing Address2";
                        return RedirectToPage();
                    }
                }

                if (userClaim.Type ==  nameof(Input.BillingZipCode) && Input.BillingZipCode != userClaim.Value)
                {
                    var oldClaim = new Claim(nameof(Input.BillingZipCode), userClaim.Value);
                    var newClaim = new Claim(nameof(Input.BillingZipCode), Input.BillingZipCode);
                    var replaced = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
                    if (!replaced.Succeeded)
                    {
                        StatusMessage = $"Unable to replace Billing Zip code";
                        return RedirectToPage();
                    }
                }

            }

            var expectedClaims = new List<AbsentClaim> {
              new AbsentClaim{ClaimName = ClaimTypes.StreetAddress, ClaimValue = Input.Address1 },
                 new AbsentClaim{ClaimName = nameof(Input.Address2), ClaimValue = Input.Address2 },
                 new AbsentClaim{ClaimName = nameof(Input.ZipCode),ClaimValue = Input.ZipCode },
                new AbsentClaim{ClaimName = nameof(Input.IsSameAsAddress),ClaimValue = Input.IsSameAsAddress.ToString() },
                 new AbsentClaim{ClaimName = nameof(Input.BillingAddress1),ClaimValue = Input.BillingAddress1 },
                 new AbsentClaim{ClaimName = nameof(Input.BillingAddress2),ClaimValue = Input.BillingAddress2 },
                 new AbsentClaim{ClaimName = nameof(Input.BillingZipCode), ClaimValue = Input.BillingZipCode }
            };

            var absentClaims = expectedClaims.Select(x => x.ClaimName).Except(userClaims.Select(x => x.Type));

            if (!absentClaims.Any())
            {
                return RedirectToPage("/Shipping");
            }

            var claims = new List<Claim>();

            foreach (var claim in absentClaims)
            {
                var cName = expectedClaims.First(x => x.ClaimName == claim);
                var c = new Claim(claim, cName.ClaimValue);
                claims.Add(c);
            }

            var addUserClaims = await _userManager.AddClaimsAsync(user, claims);

            if (!addUserClaims.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to save information. please contact support.";
                return RedirectToPage();
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
                ZipCode = userClaims.FirstOrDefault(x => x.Type == nameof(Input.ZipCode))?.Value ?? string.Empty,
                Address1 = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.StreetAddress)?.Value ?? string.Empty,
                Address2 = userClaims.FirstOrDefault(x => x.Type == nameof(Input.Address2))?.Value ?? string.Empty,

                BillingAddress1 = userClaims.FirstOrDefault(x => x.Type == nameof(Input.BillingAddress1))?.Value ?? string.Empty,
                BillingAddress2 = userClaims.FirstOrDefault(x => x.Type == nameof(Input.BillingAddress2))?.Value ?? string.Empty,
                BillingZipCode = userClaims.FirstOrDefault(x => x.Type == nameof(Input.BillingZipCode))?.Value ?? string.Empty,


            };
        }

        public class AbsentClaim
        {
            public string ClaimName { get; set; }
            public string ClaimValue { get; set; }
        }
    }
}
