using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class BagsModel : PageModel
    {
        private readonly ILogger<BagsModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IPageSection _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IBaseStore<Category> _categoryBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly ICartService _cartService;
        private readonly int _pageId;

        public BagsModel(ILogger<BagsModel> logger, ICacheService cacheService, IPageSection pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore, IBaseStore<Category> categoryBaseStore, IBaseStore<Product> productBaseStore, ICartService cartService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;
            _categoryBaseStore = categoryBaseStore;
            _productBaseStore = productBaseStore;
            _cartService = cartService;

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _pageId = razorPages.FirstOrDefault(x => string.Compare(x.PageName, "bags", StringComparison.OrdinalIgnoreCase) == 0)?.Id ?? 0;

        }

        public Dictionary<string, List<Product>> ProductWithCategories = new();
        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();


        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();

            var allCategories = GetAllCategoriesForBags();
            var allProducts = _cartService.GetProductsWithInCartCheck(GetAllProductsForBags(), User.Identity?.Name ?? string.Empty, GetCartUserId());

            allCategories.ForEach(x =>
            {
                var products = allProducts.Where(y => y.CategoryId == x.Id).ToList();
                ProductWithCategories.Add(x.CategoryName, products);
            });

            _logger.LogInformation("Lip Page Loaded with {0} Categories and {1} Products", allCategories.Count, allProducts.Count);

        }

        private string GetCartUserId()
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

        private List<Category> GetAllCategoriesForBags() => _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();
        private List<Product> GetAllProductsForBags() => _cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();

    }
}
