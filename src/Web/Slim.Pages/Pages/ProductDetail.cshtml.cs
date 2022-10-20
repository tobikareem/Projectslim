using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;

namespace Slim.Pages.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IBaseStore<Product> _productBaseStore;
        private ILogger<ProductDetailModel> _logger;

        public List<ImageData> ImagesToShow { get; set; }

        public ProductDetailModel(IBaseStore<Product> productBaseStore, ILogger<ProductDetailModel> logger)
        {
            _productBaseStore = productBaseStore;
            _logger = logger;

            ImagesToShow = new List<ImageData>();
            Product = new Product();
        }


        [BindProperty] public Product Product { get; set; }

        public Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var product = _productBaseStore.GetEntity(id.Value);
            Product = product;
            
            return Task.FromResult<IActionResult>(Page());
        }

        public class ImageData
        {
            public ImageData()
            {
                SecondaryImages = new List<string>();
            }
            public string PrimaryImage { get; set; } = string.Empty;

            public string PlaceHolderImage { get; set; } = string.Empty;
            public List<string> SecondaryImages { get; set; }
        }
    }
}
