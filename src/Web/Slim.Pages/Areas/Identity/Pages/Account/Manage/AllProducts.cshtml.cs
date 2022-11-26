using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using System.ComponentModel.DataAnnotations;
using Slim.Pages.Extensions;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage;

public class AllProductsModel : PageModel
{
    private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AllProductsModel> _logger;
    private readonly IBaseStore<Product> _productBaseStore;
    public List<SelectListItem> RazorPageSelectList { get; set; }
    public Dictionary<int, string> ProductAndPrimaryImage { get; set; }
    public AllProductsModel(IBaseStore<RazorPage> razorPagesBaseStore, ICacheService cacheService, ILogger<AllProductsModel> logger, IBaseStore<Product> productBaseStore)
    {
        _razorPagesBaseStore = razorPagesBaseStore;
        _cacheService = cacheService;
        _logger = logger;
        _productBaseStore = productBaseStore;
        RazorPageSelectList = new List<SelectListItem>();
        ProductAndPrimaryImage = new Dictionary<int, string>();

        var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll).Where(x => SlmConstant.PagesForDropDown.Contains(x.PageName));
        RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
    }
    [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();
    [BindProperty] public List<Product> Products { get; set; } = new();

    public int YourProductCount;


    public IActionResult OnGet()
    {
        if (RazorPageSelectList.Any() && int.TryParse(RazorPageSelectList.First().Value, out var pageId))
        {
            Products = GetAllProductsByProductId(pageId);

            _logger.LogInformation($"Getting Product information for {pageId}");
        }
        else
        {
            Products = GetAllProductsByProductId(-1);
        }
        YourProductCount = Products.Count;
        return Page();
    }

    public List<Product> GetAllProductsByProductId(int productId)
    {
        Products = _productBaseStore.GetAll().ToList();

        Products.ForEach(product =>
        {
            var imgPrimary = string.Empty;
            var primaryImage = product.Images.FirstOrDefault(image => image.IsPrimaryImage);
            if (primaryImage == null) return;

            ProductAndPrimaryImage.TryAdd(product.Id, imgPrimary.GetImageSrc(primaryImage.UploadedImage));
        });

        Products = productId == -1 ? Products : Products.Where(product => product.RazorPageId == productId).ToList();
        YourProductCount = Products.Count;

        return Products;
    }

    public IActionResult OnGetProductById(int id)
    {
        Products = GetAllProductsByProductId(id);
        YourProductCount = Products.Count;
        return Page();
    }

    public IActionResult OnPostEachProduct(int id)
    {
        return Page();
    }

    public class InputModel
    {

        [Required, Display(Name = "Product Page")]
        public string RazorPageId { get; set; } = default!;
    }
}