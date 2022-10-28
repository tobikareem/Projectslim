using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class LipModel : PageModel
    {
        private readonly ILogger<LipModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IPageSection<PageSection> _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly int _pageId;
        private readonly IBaseStore<Category> _categoryBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        public Dictionary<string, List<Product>> ProductWithCategories = new();

        public LipModel(ILogger<LipModel> logger, ICacheService cacheService, IPageSection<PageSection> pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore, IBaseStore<Category> categoryBaseStore, IBaseStore<Product> productBaseStore)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;
            _categoryBaseStore = categoryBaseStore;
            _productBaseStore = productBaseStore;

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _pageId = razorPages.FirstOrDefault(x => string.Compare(x.PageName, "Lip Gloss", StringComparison.OrdinalIgnoreCase) == 0)?.Id ?? 0;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();

        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();

            var allCategories = GetAllCategoriesForLips();
            var allProducts = GetAllProductsForLips();

            allCategories.ForEach(x =>
            {
                var products = allProducts.Where(y => y.CategoryId == x.Id).ToList();
                ProductWithCategories.Add(x.CategoryName, products);
            });

            _logger.LogInformation("Lip Page Loaded with {0} Categories and {1} Products", allCategories.Count, allProducts.Count);
        }

        private List<Product> GetAllProductsForLips() => _cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();
        private List<Category> GetAllCategoriesForLips() => _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryBaseStore.GetAll).Where(x =>x.RazorPageId == _pageId).ToList();
        
    }
}
