using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Serv;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Xml.Linq;

namespace Slim.Pages.Pages
{
    public class ReviewModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ReviewModel> _logger;
        private string ShoppingCartUserId { get; set; }

        public ReviewModel(ICartService cartService, UserManager<IdentityUser> userManager, ILogger<ReviewModel> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;

            ShoppingCartUserId = string.Empty;
        }
        public List<ShoppingCart> CartItems { get; set; } = new();
        public decimal TotalCartPrice { get; set; }

        public InputModel Input { get; set; } = new();

        public async Task OnGet()
        {
            var loggedInUser = User.Identity?.Name ?? string.Empty;
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems = _cartService.GetCartItemsForUser(loggedInUser, ShoppingCartUserId);
            TotalCartPrice = _cartService.GetTotalCartPrice(loggedInUser, ShoppingCartUserId);

            await LoadUserInformationAsync();
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

            var firstName = User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Skip(1).FirstOrDefault()?.Value ?? string.Empty;
            var lastName = User.FindFirstValue(ClaimTypes.Surname);
            var phoneNumber = user.PhoneNumber;

            var zipCode = userClaims.FirstOrDefault(x => x.Type == nameof(Input.ZipCode))?.Value ?? string.Empty;
            var address1 = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.StreetAddress)?.Value ?? string.Empty;
            var address2 = userClaims.FirstOrDefault(x => x.Type == nameof(Input.Address2))?.Value ?? string.Empty;


            Input = new InputModel
            {
                FullName = $"{firstName} {lastName}",
                Address = $"{address1}, {address2}, {zipCode}",
                PhoneNumber = phoneNumber,
            };



        }

        public class InputModel
        {
            public string FullName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public bool IsSameAsAddress { get; set; }
            public string ZipCode { get; set; } = string.Empty;
            public string Address1 { get; set; } = string.Empty;
            public string Address2 { get; set; } = string.Empty;
        }
    }
}
