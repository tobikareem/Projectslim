using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
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
        public CheckoutModel(IBaseCart<ShoppingCart> shoppingCartBaseStore, IBaseStore<Product> productStore, ILogger<CheckoutModel> logger, ICacheService cacheService, ICartService cartService)
        {
            _shoppingCartBaseStore = shoppingCartBaseStore;
            _productStore = productStore;
            _logger = logger;
            _cacheService = cacheService;
            _cartService = cartService;

            ShoppingCartUserId = string.Empty;
        }
        
        private string ShoppingCartUserId { get; set; }
        public List<ShoppingCart> CartItems { get; set; } = new();
        public decimal TotalCartPrice { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new();


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

        public class InputModel
        {
            [Required, Display(Name = "First Name"), StringLength(50)]
            public string FirstName { get; set; } = string.Empty;

            [Required, Display(Name = "Last Name"), StringLength(50)]
            public string LastName { get; set; } = string.Empty;

            [Required, Display(Name = "E-mail Address"), DataType(DataType.EmailAddress), StringLength(50), EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, Display(Name = "Phone Number"), DataType(DataType.PhoneNumber), StringLength(50)]
            public string PhoneNumber { get; set; } = string.Empty;

            [Display(Name = "Company"), StringLength(50)]
            public string Company { get; set; } = string.Empty;

            [Required, Display(Name = "ZIP Code"), StringLength(50)]
            public string ZipCode { get; set; } = string.Empty;

            [Required, Display(Name = "Address 1"), StringLength(50)]
            public string Address1 { get; set; } = string.Empty;

            [Display(Name = "Address 2"), StringLength(50)]
            public string Address2 { get; set; } = string.Empty;

            [Required, Display(Name = "Same as Shipping Address")]
            public bool IsSameAsAddress { get; set; }
        }
    }
}
