using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IPageSection<PageSection> _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly IBaseStore<Category> _categoryBaseStore;
        private readonly int _pageId;

        private IEnumerable<RazorPage> _razorPages;
             
        public List<Product> Lashes { get; set; } = new();
        public List<Product> Hair { get; set; } = new();
        public List<Product> LipGloss { get; set; } = new();


        public IndexModel(ILogger<IndexModel> logger, ICacheService cacheService, IPageSection<PageSection> pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore, IBaseStore<Product> productBaseStore, IBaseStore<Category> categoryBaseStore)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;
            _productBaseStore = productBaseStore;
            _categoryBaseStore = categoryBaseStore;

             _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            
            _pageId = _razorPages
                         .FirstOrDefault(x =>
                             string.Compare(x.PageName, "Home", StringComparison.OrdinalIgnoreCase) == 0)?.Id ??
                     0;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();

        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();

            var products = GetAllProducts();

            var lashesRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "lashes")?.Id ?? 1;
            var hairRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "hair")?.Id ?? 1;
            var lipRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "lip gloss")?.Id ?? 1;

            Lashes = products.Where(x => x.RazorPageId == lashesRazorPageId).Take(8).ToList();
            Hair = products.Where(x => x.RazorPageId == hairRazorPageId).ToList();
            LipGloss = products.Where(x => x.RazorPageId == lipRazorPageId).Take(12).ToList();

            _logger.LogInformation("Obtained Products for the following. Hair = {Hair}. Lips = {Lip}. Lashes = {Lashes}", Hair.Count, LipGloss.Count, Lashes.Count);

        }

        private List<Product> GetAllProducts() =>_cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll).ToList();
    }
}