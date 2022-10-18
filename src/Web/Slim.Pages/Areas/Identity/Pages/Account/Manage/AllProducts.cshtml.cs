using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.Operations;
using Slim.Pages.Extensions;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage;

public class AllProductsModel : PageModel
{
    private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AllProductsModel> _logger;
    private readonly IBaseStore<Product> _productBaseStore;
    public List<SelectListItem> RazorPageSelectList { get; set; }
    public List<string> PrimaryImages { get; set; }
    public AllProductsModel(IBaseStore<RazorPage> razorPagesBaseStore, ICacheService cacheService, ILogger<AllProductsModel> logger, IBaseStore<Product> productBaseStore)
    {
        _razorPagesBaseStore = razorPagesBaseStore;
        _cacheService = cacheService;
        _logger = logger;
        _productBaseStore = productBaseStore;
        RazorPageSelectList = new List<SelectListItem>();
        PrimaryImages = new List<string>();
        var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
        RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
    }
    [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();
    [BindProperty] public List<Product> Products { get; set; } = new();


    public void OnGet()
    {
        Products = _productBaseStore.GetAll().ToList();
        
        Products.ForEach(product =>
        {
            var imgPrimary = string.Empty;
            var primaryImage = product.Images.FirstOrDefault(image => image.IsPrimaryImage);
            if (primaryImage == null) return;
            PrimaryImages.Add(imgPrimary.GetImageSrc(primaryImage.UploadedImage));
        });
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