using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Pages.Extensions;
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
        private readonly IBaseStore<UserPageImage> _userPagesBaseStore;


        private IEnumerable<RazorPage> _razorPages;
             
        public List<Product> DisplayBags { get; set; } = new();
        public List<Product> DisplayShoes { get; set; } = new();
        public List<Product> DisplayAccessories { get; set; } = new();


        public IndexModel(ILogger<IndexModel> logger, ICacheService cacheService, IPageSection pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore, IBaseStore<Product> productBaseStore, ICartService cartService, IBaseStore<UserPageImage> userPagesBaseStore)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;
            _productBaseStore = productBaseStore;
            _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _cartService = cartService;
            _userPagesBaseStore = userPagesBaseStore;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();
        [BindProperty] public ImageModel ImgModel { get; set; } = new();
        public Dictionary<int, IGrouping<int, ShoppingCart>> ShoppingCart = new();
             
        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();

            GetHomePageImages();

            var products = _cartService.GetProductsWithInCartCheck(GetAllProducts(), User.Identity?.Name ?? string.Empty, GetCartUserId());

            var bagsRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "bags")?.Id ?? 1;
            var accessoriesRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "accessories")?.Id ?? 1;
            var shoesRazorPageId = _razorPages.FirstOrDefault(x => x.PageName.ToLowerInvariant() == "shoes")?.Id ?? 1;
            DisplayBags = products.Where(x => x.RazorPageId == bagsRazorPageId).Take(8).ToList();
            DisplayAccessories = products.Where(x => x.RazorPageId == accessoriesRazorPageId).ToList();
            DisplayShoes = products.Where(x => x.RazorPageId == shoesRazorPageId).Take(12).ToList();

            _logger.LogInformation("Obtained Products for the following. Bags = {Hair}. Shoes = {Lip}. Accessories = {Lashes}", DisplayBags.Count, DisplayShoes.Count, DisplayAccessories.Count);

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


        private void GetHomePageImages()
        {
            var images = _cacheService.GetOrCreate(CacheKey.GetHomePageImages, _userPagesBaseStore.GetAll, 60);

            var homePageImages = images.Where(x => x.ImageName == "HomePage").ToList();

            if (homePageImages == null || !homePageImages.Any())
            {
                return;
            }

            var imgBag = homePageImages.FirstOrDefault(x => x.ImageDescription == "HomePage Bag Image")?.UploadedImage;
            ImgModel.ImageBag = GetImageStr(imgBag);

            var imgShoe = homePageImages.FirstOrDefault(x => x.ImageDescription == "HomePage Shoe Image")?.UploadedImage;
            ImgModel.ImageShoe = GetImageStr(imgShoe);

            var imgAccess = homePageImages.FirstOrDefault(x => x.ImageDescription == "HomePage Accessory Image")?.UploadedImage;
            ImgModel.ImageAccess = GetImageStr(imgAccess);

        }

        private string GetImageStr(byte[]? image)
        {
            var imgSrc = string.Empty;
            return imgSrc.GetImageSrc(image); ;
        }

        private List<Product> GetAllProducts() =>_cacheService.GetOrCreate(CacheKey.GetProducts, _productBaseStore.GetAll).ToList();


        public class ImageModel
        {

            public string ImageBag { get; set; } = string.Empty;
            public string ImageShoe { get; set; } = string.Empty;
            public string ImageAccess { get; set; } = string.Empty;
        }
    }
}