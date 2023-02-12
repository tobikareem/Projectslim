using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class CartModel : PageModel
    {
        private readonly IBaseCart<ShoppingCart> _shoppingCartBaseStore;
        private readonly IBaseStore<Product> _productStore;
        private readonly ILogger<CartModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly ICartService _cartService;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;

        public List<SelectListItem> BagSizesListItems { get; set; }
        public List<SelectListItem> QuantitySelectListItem { get; set; }

        public CartModel(IBaseCart<ShoppingCart> shoppingCart, IBaseStore<Product> product, ILogger<CartModel> logger, ICacheService cacheService, ICartService cartService, IBaseStore<RazorPage> razorPagesBaseStore)
        {
            _shoppingCartBaseStore = shoppingCart;
            _productStore = product;
            _cacheService = cacheService;
            _cartService = cartService;
            _razorPagesBaseStore = razorPagesBaseStore;

            ShoppingCartUserId = string.Empty;
            _logger=logger;

            QuantitySelectListItem = Enumerable.Range(1, 10).Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString()
            }).ToList();

            BagSizesListItems = SlmConstant.BagSizes.Select(size => new SelectListItem
            {
                Value = size,
                Text = size
            }).ToList();

        }

        private string ShoppingCartUserId { get; set; }
        [BindProperty] public List<ShoppingCart> CartItems { get; set; } = new();
        [BindProperty] public string SelectedBagSize { get; set; } = string.Empty;

        public decimal TotalCartPrice { get; set; }

        public void OnGet(int? id)
        {
            CartItems =  _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, GetShoppingCartUserId());
            TotalCartPrice = GetTotalCartPrice();

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            // var isBagCategory = razorPages.First(x => x.PageName == "Bags").Id == Product.RazorPageId;
        }

        public JsonResult OnGetAddNewProductToCart(int id, string bagSize)
        {
            ShoppingCartUserId = GetShoppingCartUserId();

            var cartItem = _shoppingCartBaseStore.GetCartUserItem(ShoppingCartUserId, id);

            if (cartItem.Quantity == 0)
            {
                cartItem = new ShoppingCart
                {
                    Quantity = 1,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? ShoppingCartUserId,
                    Product = _productStore.GetEntity(id),
                    CartUserId = User.Identity?.Name ?? ShoppingCartUserId
                };

                if (!string.IsNullOrWhiteSpace(bagSize))
                {
                    cartItem.BagSize = bagSize;
                }

                _shoppingCartBaseStore.AddEntity(cartItem, CacheKey.GetShoppingCartItem, true);

                _logger.LogInformation("... Added new item to Shopping Cart by user: {user}", ShoppingCartUserId);

                CartItems = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);
                TotalCartPrice = GetTotalCartPrice();

                return new JsonResult(TotalCartPrice);
            }

            cartItem.Quantity++;
            cartItem.ModifiedDate = DateTime.UtcNow;

            _shoppingCartBaseStore.UpdateEntity(cartItem, CacheKey.GetShoppingCartItem, true);
            _logger.LogInformation("... Shopping Product is updated to cart by user {cartUser}", ShoppingCartUserId);

            CartItems = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);
            TotalCartPrice = GetTotalCartPrice();
            return new JsonResult(TotalCartPrice);
        }
        

        public JsonResult OnGetUpdateShoppingCart(string cartItemId, int quantity, string bagSize, string changeType)
        {
            ShoppingCartUserId = GetShoppingCartUserId();
            var itemsFromCache = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);

            // get all items where quantity has changed
            var changedItemFromCache = itemsFromCache.FirstOrDefault(x => x.Id == cartItemId);

            if (changedItemFromCache == null)
            {
                _logger.LogInformation("... No changes in Shopping Cart");
                return new JsonResult(TotalCartPrice);
            }

           
            changedItemFromCache.ModifiedDate = DateTime.UtcNow;
            changedItemFromCache.ModifiedBy = ShoppingCartUserId;

            switch (changeType)
            {
                case "quantity":
                    changedItemFromCache.Quantity = quantity;
                    break;
                case "bagSize":
                    changedItemFromCache.BagSize = bagSize;
                    break;
            }

            _shoppingCartBaseStore.UpdateEntity(changedItemFromCache, CacheKey.GetShoppingCartItem, true);

            CartItems  = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);
            TotalCartPrice = GetTotalCartPrice();

            ShoppingCartUserId = GetShoppingCartUserId();
            _logger.LogInformation("... Shopping Cart is updated by user {cartUser}", ShoppingCartUserId);

            return new JsonResult(TotalCartPrice);
        }

        public IActionResult OnPostRemoveCartItem(string id)
        {
            var cartItem = _cacheService.GetItem<IEnumerable<ShoppingCart>>(CacheKey.GetShoppingCartItem).FirstOrDefault(x => x.Id == id);

            if (cartItem == null)
            {
                _logger.LogInformation("... No item in the shopping cart");
                return Page();
            }


            ShoppingCartUserId = GetShoppingCartUserId();
            _shoppingCartBaseStore.DeleteEntity(cartItem, CacheKey.GetShoppingCartItem, true);
            _logger.LogInformation("... Shopping Cart Item is removed by user {cartUser}", ShoppingCartUserId);

            CartItems  = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);
            TotalCartPrice = GetTotalCartPrice();

            return RedirectToPage("./Index");
        }
        
        private string GetShoppingCartUserId()
        {
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
        private decimal GetTotalCartPrice()
        {
            return CartItems.Sum(c =>
            {
                if (c.Product != null)
                {
                    return c.Product is { IsOnSale: true }
                        ? c.Product.SalePrice * c.Quantity

                        : c.Product.StandardPrice * c.Quantity;
                }

                return 0.0m;
            });
        }

        public JsonResult OnGetTotalCartCount()
        {
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems  = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId); 
            var totalItem = CartItems.Distinct().Count();
            return new JsonResult(totalItem);
        }

        public JsonResult OnGetTotalCartPrice()
        {
            ShoppingCartUserId = GetShoppingCartUserId();
            CartItems  = _cartService.GetCartItemsForUser(User.Identity?.Name ?? string.Empty, ShoppingCartUserId);
            var totalPrice = GetTotalCartPrice();
            return new JsonResult(totalPrice);
        }

    }
}
