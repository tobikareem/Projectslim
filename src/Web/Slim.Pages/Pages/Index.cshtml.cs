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
        private readonly IPageSection _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly ICartService _cartService;

        private IEnumerable<RazorPage> _razorPages;
             
        public List<Product> Lashes { get; set; } = new();
        public List<Product> Hair { get; set; } = new();
        public List<Product> LipGloss { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, ICacheService cacheService, IPageSection pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore, IBaseStore<Product> productBaseStore, ICartService cartService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;
            _productBaseStore = productBaseStore;
            _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _cartService = cartService;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();
        public Dictionary<int, IGrouping<int, ShoppingCart>> ShoppingCart = new();
             
        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();

            var products = _cartService.GetProductsWithInCartCheck(GetAllProducts(), User.Identity?.Name ?? string.Empty, GetCartUserId());

            var lashesRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "lashes")?.Id ?? 1;
            var hairRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "hair")?.Id ?? 1;
            var lipRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "lip gloss")?.Id ?? 1;
            Lashes = products.Where(x => x.RazorPageId == lashesRazorPageId).Take(8).ToList();
            Hair = products.Where(x => x.RazorPageId == hairRazorPageId).ToList();
            LipGloss = products.Where(x => x.RazorPageId == lipRazorPageId).Take(12).ToList();

            _logger.LogInformation("Obtained Products for the following. Hair = {Hair}. Lips = {Lip}. Lashes = {Lashes}", Hair.Count, LipGloss.Count, Lashes.Count);

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



        private List<Product> GetAllProducts() =>_cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll).ToList();
        
    }
}