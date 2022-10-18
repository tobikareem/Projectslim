using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using System.ComponentModel.DataAnnotations;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class AddNewProductModel : PageModel
    {
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly ICacheService _cacheService;

        private readonly IImage<Image> _imageBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly ILogger<AddNewProductModel> _logger;

        public string TextCaption { get; set; } = "Add New Product";
        public string BtnCaption { get; set; } = "Upload Product";
        public bool IsEditing { get; set; } = false;

        public List<SelectListItem> RazorPageSelectList { get; set; }

        public AddNewProductModel(IBaseStore<RazorPage> razorPagesBaseStore, ICacheService cacheService, IImage<Image> imageBaseStore, ILogger<AddNewProductModel> logger, IBaseStore<Product> productBaseStore)
        {
            _razorPagesBaseStore = razorPagesBaseStore;
            _cacheService = cacheService;
            _logger = logger;
            _imageBaseStore = imageBaseStore;
            _productBaseStore = productBaseStore;
            RazorPageSelectList = new List<SelectListItem>();
            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
        }

        [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();

        public void OnGet()
        {
            TextCaption = "Add New Product";
            IsEditing   = false;
        }


        public IActionResult OnGetEditProduct(int id)
        {
            var product = _productBaseStore.GetEntity(id);
            TextCaption = $"Edit: {product.ProductName}";
            BtnCaption  = "Update Product";
            IsEditing   = true;

            InModel = new InputModel
            {
                RazorPageId = product.RazorPageId.ToString(),
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                SalePrice = product.SalePrice,
                StandardPrice = product.StandardPrice,
                ProductTags = product.ProductTags,
                IsOnSale = product.IsOnSale,
                IsNewProduct = product.IsNewProduct,
                IsTrending = product.IsTrending,
                Id = product.Id
            };

            //var pImg = product.Images.First(c => c.IsPrimaryImage);
            
            //var file = new FormFile(new MemoryStream(pImg.UploadedImage), 0, InModel.ProductImage.Length, pImg.ImageId.ToString(), "image.jpg" )
            //{
            //    Headers = new HeaderDictionary(),
            //    ContentType = "image/jpg",
            //    ContentDisposition = "form-data"
            //};

            //InModel.ProductImage = file;

            return Page();
        }

        public IActionResult OnGetDeleteProduct(int id)
        {
            var product = _productBaseStore.GetEntity(id);
            _productBaseStore.DeleteEntity(product);
            return RedirectToPage("./AllProducts");
        }

        public IActionResult OnPostAddNewProduct()
        {

            var isEdit = InModel.Id > 0;

            if (isEdit)
            {
                var prd = _productBaseStore.GetEntity(InModel.Id);

                prd.ProductName = InModel.ProductName;
                prd.ProductDescription = InModel.ProductDescription;
                prd.SalePrice = InModel.SalePrice;
                prd.StandardPrice = InModel.StandardPrice;
                prd.ProductTags = InModel.ProductTags;
                prd.IsOnSale = InModel.IsOnSale;
                prd.IsNewProduct = InModel.IsNewProduct;
                prd.IsTrending = InModel.IsTrending;
                prd.RazorPageId = int.Parse(InModel.RazorPageId);

                

                _productBaseStore.UpdateEntity(prd);
            }

            var product = ValidateProductionUpload();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _productBaseStore.AddEntity(product);
            return RedirectToPage("./AllProducts");
        }


        private Product ValidateProductionUpload()
        {
            var product = new Product
            {
                CreatedBy ="Test User",
                CreatedDate = DateTime.UtcNow,
                ProductDescription = InModel.ProductDescription,
                ProductName = InModel.ProductName,
                SalePrice = InModel.SalePrice,
                Enabled = true,
                Images = ValidateFileUploads(),
                RazorPageId = int.Parse(InModel.RazorPageId),
                StandardPrice = InModel.StandardPrice,
                IsNewProduct = InModel.IsNewProduct,
                IsOnSale = InModel.IsOnSale,
                IsTrending = InModel.IsTrending,
                ProductTags = InModel.ProductTags

            };

            return product;
        }

        private List<Image> ValidateFileUploads()
        {
            var images = new List<Image>();
            try
            {

                if (InModel.ProductImage == null)
                {
                    ModelState.AddModelError("File Upload", "Please upload at least one image");
                    return images;
                }

                var image = new Image();
                using var ms = new MemoryStream();
                InModel.ProductImage.CopyTo(ms);

                // upload the file if less than 2 MB
                if (ms.Length < 2097152)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(InModel.ProductImage.ContentDisposition).FileName.Trim();
                    var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
                    var fileExtension = fileName.Value[(index + 1)..];

                    if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg")
                    {
                        image.ImageId = Guid.NewGuid();
                        image.UploadedImage = ms.ToArray();
                        image.IsPrimaryImage = true;
                        image.Enabled = true;
                        image.CreatedDate = DateTime.UtcNow;
                        image.CreatedBy = "Test User";

                        images.Add(image);
                    }
                    else
                    {
                        ModelState.AddModelError("File Upload", "Please upload a valid image file (jpg, png, jpeg)");
                    }
                    
                }
                else
                {
                    var fileName = ContentDispositionHeaderValue.Parse(InModel.ProductImage.ContentDisposition).FileName.Trim();
                    ModelState.AddModelError("ProductImage", $"The file {fileName} is too large. Must be less than 2 MB");
                }

                if (InModel.ProductImages == null || !InModel.ProductImages.Any())
                {
                    return images;
                }


                foreach (var file in InModel.ProductImages)
                {
                    if (file.Length is > 0 and < 2097152)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim();
                        var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
                        var fileExtension = fileName.Value[(index + 1)..];

                        if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg")
                        {
                            using var ms1 = new MemoryStream();
                            file.CopyTo(ms1);
                            image = new Image
                            {
                                ImageId = Guid.NewGuid(),
                                UploadedImage = ms1.ToArray(),
                                IsPrimaryImage = false,
                                Enabled = true,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = "Test User"
                            };
                            images.Add(image);
                        }
                        else
                        {
                            ModelState.AddModelError("File Upload", "Please upload a valid image file (jpg, png, jpeg)");
                        }
                    }
                    else
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(InModel.ProductImage.ContentDisposition).FileName.Trim();
                        ModelState.AddModelError("ProductImage", $"The file {fileName} is too large.");
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading file");
                throw;
            }

            return images;
        }

        public class InputModel
        {

            public int Id { get; set; }

            [Required, Display(Name = "Product Page")]
            public string RazorPageId { get; set; } = default!;

            /// <summary>
            /// Must only use letters.
            /// The first letter is required to be uppercase. White spaces are allowed while numbers, and special characters are not allowed.
            /// </summary>
            [Required, Display(Name = "Product Name"), StringLength(50, ErrorMessage = "Product Name must be less than 50 characters", MinimumLength = 3)]
            // [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
            public string? ProductName { get; set; }

            [Required, Display(Name = "Product Description")]
            public string? ProductDescription { get; set; }

            [Required, Display(Name = "Product Price"), DataType(DataType.Currency, ErrorMessage = "Please enter a valid price"), Range(typeof(decimal), "0.01", "1000000.00", ErrorMessage = "Please enter a valid price")]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal StandardPrice { get; set; }

            [Required, Display(Name = "Sales Price"), DataType(DataType.Currency, ErrorMessage = "Please enter a valid price"), Range(typeof(decimal), "0.01", "1000000.00", ErrorMessage = "Please enter a valid price")]
            [Column(TypeName = "decimal(18, 2)")]
            public decimal SalePrice { get; set; }

            [Display(Name = "Product tags")]
            public string? ProductTags { get; set; }

            [Required, Display(Name = "Product Image")]
            public IFormFile ProductImage { get; set; } = default!;

            [Display(Name = "Product files for sale")]
            public IEnumerable<IFormFile> ProductImages { get; set; } = default!;

            [Display(Name = "On Sale?")]
            public bool IsOnSale { get; set; }

            [Display(Name = "New Product")]
            public bool IsNewProduct { get; set; } = true;

            [Display(Name = "Trending")]
            public bool IsTrending { get; set; }

        }
    }
}
