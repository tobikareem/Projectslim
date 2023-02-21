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
using System.Globalization;
using NuGet.Packaging;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class AddNewProductModel : PageModel
    {
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly ICacheService _cacheService;
        private readonly IBaseImage _imageBaseStore;
        private readonly IBaseStore<Product> _productBaseStore;
        private readonly IBaseStore<ProductDetail> _productDetailBaseStore;
        private readonly ILogger<AddNewProductModel> _logger;
        private readonly IBaseStore<Category> _categoryBaseStore;
        private readonly IEnumerable<Category> _categories;
        private readonly IEnumerable<RazorPage> _razorPages;
        private readonly ICartService _cartService;

        public TestCaptions TextCaptions { get; set; }
        public List<SelectListItem> RazorPageSelectList { get; set; }
        public List<SelectListItem> CategorySelectList { get; set; }
        public List<SelectListItem> AllMenShoeSizesItems { get; set; }
        public List<SelectListItem> AllWomenShoeSizesItems { get; set; }

        public string[] Genders = SlmConstant.Genders;
        public double[] MenShoeSizes = ShoeSize.MensNgSize;
        public double[] WomenShoeSizes = ShoeSize.WomensNgSize;
        [BindProperty] public IEnumerable<string> SelectedShoeSizes { get; set; }

        public AddNewProductModel(IBaseStore<RazorPage> razorPagesBaseStore, ICacheService cacheService, IBaseImage imageBaseStore, ILogger<AddNewProductModel> logger, IBaseStore<Product> productBaseStore, IBaseStore<Category> categoryBaseStore, ICartService cartService, IBaseStore<ProductDetail> productDetailBaseStore)
        {
            _razorPagesBaseStore = razorPagesBaseStore;
            _cacheService = cacheService;
            _logger = logger;
            _imageBaseStore = imageBaseStore;
            _productBaseStore = productBaseStore;
            _categoryBaseStore = categoryBaseStore;
            _cartService = cartService;
            _productDetailBaseStore = productDetailBaseStore;

            _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll).Where(x => x.PageName != "Home").ToList();
            _categories = _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryBaseStore.GetAll);

            var pageId = _razorPages.FirstOrDefault(x => string.Compare(x.PageName, "Bags", StringComparison.OrdinalIgnoreCase) == 0)?.Id ?? 1;

            RazorPageSelectList = _razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
            CategorySelectList = _categories.Where(x => x.RazorPageId == pageId).Select(category => new SelectListItem { Text = category.CategoryName, Value = category.Id.ToString() }).ToList();
            AllMenShoeSizesItems = MenShoeSizes.Select(x => new SelectListItem { Text = x.ToString(CultureInfo.CurrentCulture), Value = x.ToString(CultureInfo.CurrentCulture) }).ToList();
            AllWomenShoeSizesItems = WomenShoeSizes.Select(x => new SelectListItem { Text = x.ToString(CultureInfo.CurrentCulture), Value = x.ToString(CultureInfo.CurrentCulture) }).ToList();

            SelectedShoeSizes = Enumerable.Empty<string>();
            TextCaptions = new TestCaptions();
        }

        [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();
        [TempData] public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            TextCaptions = new TestCaptions
            {
                TitleCaption = "Add New Product",
                IsEditing = false
            };

            InModel.Category = _categories.FirstOrDefault(x => x.CategoryName == "General")?.Id ?? 1;
        }

        public IActionResult OnPostAddNewProduct()
        {

            var shoeSize = Convert.ToString(TempData["ringSize"]);
            if (!string.IsNullOrWhiteSpace(shoeSize))
            {
                InModel.SelectedRingSizes = shoeSize;
            }

            var isEdit = InModel.Id > 0;

            var product = isEdit ? UpdateExistingProductTemplate() : CreateNewProductTemplate();

            if (_cartService.GetProductType(product.RazorPageId) == "bags" && !InModel.HasMini && !InModel.HasMidi && !InModel.HasMaxi)
            {
                StatusMessage = "Error. Bag must have a Mini, Midi or Maxi size.";
                return Page();
            }


            var isDetailExist = product.ProductDetails.Any();
            var productDetail = CreateProductDetailTemplate(product, isDetailExist);

            if (isEdit)
            {
                _productBaseStore.UpdateEntity(product, CacheKey.GetProducts, true);

                if (isDetailExist)
                {
                    _productDetailBaseStore.UpdateEntity(productDetail, CacheKey.GetProductDetails, true);
                }
                else
                {
                    productDetail.Product = product;
                    _productDetailBaseStore.AddEntity(productDetail, CacheKey.GetProductDetails, true);
                }

                return RedirectToPage("./AllProducts");
            }

            if (!ModelState.IsValid)
            {
                StatusMessage = "Error. Missing validations.";
                return Page();
            }

            _productBaseStore.AddEntity(product, CacheKey.GetProducts, true);

            productDetail.Product = product;
            _productDetailBaseStore.AddEntity(productDetail, CacheKey.GetProductDetails, true);

            return RedirectToPage("./AllProducts");
        }

        public IActionResult OnGetEditProduct(int id)
        {
            var product = _productBaseStore.GetEntity(id);

            TextCaptions = new TestCaptions
            {
                IsEditing = true,
                TitleCaption = $"Edit: {product.ProductName}",
                BtnCaption = "Update Product",
                ProfileImageEditText = "Uploading a new image will replace the current image",
                ProfileImagesEditText = "Uploading one or more images will remove all current Images"
            };

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
                ProductQuantity = product.ProductQuantity,
                Category = product.Category.Id,
                Id = product.Id
            };

            if (!product.ProductDetails.Any())
            {
                return Page();
            }

            InModel.Gender = string.IsNullOrWhiteSpace(product.ProductDetails.First().Gender)
                ? "All"
                : product.ProductDetails.First().Gender;
            InModel.HasMaxi = product.ProductDetails.Any(x => x.HasMaxi);
            InModel.HasMidi = product.ProductDetails.Any(x => x.HasMidi);
            InModel.HasMini = product.ProductDetails.Any(x => x.HasMini);
            SelectedShoeSizes = product.ProductDetails.First().ShoeSize.Split(',');

            var categories = _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryBaseStore.GetAll).Where(x => x.RazorPageId == product.RazorPageId);
            CategorySelectList = categories.Select(category => new SelectListItem { Text = category.CategoryName, Value = category.Id.ToString() }).ToList();

            if (_cartService.GetProductType(product.RazorPageId) == "jewelries")
            {
                TempData["ringSize"] = product.ProductDetails.First().JewelrySize;
            }

            return Page();
        }

        public IActionResult OnGetDeleteProduct(int id)
        {
            var product = _productBaseStore.GetEntity(id);
            _productBaseStore.DeleteEntity(product, CacheKey.GetProducts, true);

            if (!product.ProductDetails.Any())
            {
                return RedirectToPage("./AllProducts");
            }

            var details = product.ProductDetails.First();
            _productDetailBaseStore.DeleteEntity(details, CacheKey.GetProductDetails, true);

            return RedirectToPage("./AllProducts");
        }

        public JsonResult OnGetSelectionChanged(int id)
        {
            var categories = _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryBaseStore.GetAll).Where(x => x.RazorPageId == id);
            return new JsonResult(categories);
        }

        public JsonResult OnGetReturnRingSizes(int productId)
        {
            var product = _productBaseStore.GetEntity(productId);

            if (product?.Id == 0 || !product.ProductDetails.Any())
            {
                return new JsonResult(null);
            }

            var ringSizes = product.ProductDetails.First().JewelrySize;

            var allSizes = ringSizes.Split(',').Where(x => int.TryParse(x, out _)).Select(int.Parse);

            return new JsonResult(allSizes);
        }

        public JsonResult OnGetAddRingSizes(string ringSizes, int productId)
        {
            var productIdDetails = _productDetailBaseStore.GetAll().FirstOrDefault(x => x.ProductId == productId);

            switch (productIdDetails)
            {
                case null when !string.IsNullOrWhiteSpace(ringSizes):
                    TempData["ringSize"] = ringSizes;
                    return new JsonResult(null);
                case null:
                    return new JsonResult(null);
            }

            // Get the product detail for jewelry size
            productIdDetails.JewelrySize = ringSizes;
            _productDetailBaseStore.UpdateEntity(productIdDetails, CacheKey.GetProductDetails, true);
            return new JsonResult(productIdDetails);
        }

        private Product CreateNewProductTemplate()
        {
            var product = new Product
            {
                CreatedBy = User.Identity?.Name,
                CreatedDate = DateTime.UtcNow,
                ProductDescription = InModel.ProductDescription,
                ProductName = InModel.ProductName,
                SalePrice = Convert.ToDecimal(InModel.SalePrice),
                Enabled = true,
                Images = ValidateFileUploads(),
                RazorPageId = int.Parse(InModel.RazorPageId),
                StandardPrice = Convert.ToDecimal(InModel.StandardPrice),
                IsNewProduct = InModel.IsNewProduct,
                IsOnSale = InModel.IsOnSale,
                IsTrending = InModel.IsTrending,
                ProductTags = InModel.ProductTags,
                ProductQuantity = InModel.ProductQuantity,
                CategoryId = InModel.Category
            };
            return product;
        }

        private ProductDetail CreateProductDetailTemplate(Product product, bool isDetailExist)
        {
            ProductDetail detail;

            if (isDetailExist)
            {
                detail = product.ProductDetails.First();
                detail.Gender = InModel.Gender;

                if (_cartService.GetProductType(product.RazorPageId) == "bags")
                {
                    detail.HasMini = InModel.HasMini;
                    detail.HasMidi = InModel.HasMidi;
                    detail.HasMaxi = InModel.HasMaxi;
                }

                if (_cartService.GetProductType(product.RazorPageId) == "shoes")
                {
                    detail.ShoeSize = string.Join(',', SelectedShoeSizes);
                }

                detail.ModifiedBy = User.Identity?.Name;
                detail.ModifiedDate = DateTime.UtcNow;

                return detail;
            }

            detail = new ProductDetail
            {
                Gender = InModel.Gender,

                HasMini = InModel.HasMini,
                HasMidi = InModel.HasMidi,
                HasMaxi = InModel.HasMaxi,
                ShoeSize = string.Join(',', SelectedShoeSizes),
                JewelrySize = InModel.SelectedRingSizes,

                CreatedDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name,
            };

            return detail;

        }

        private Product UpdateExistingProductTemplate()
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
            prd.ProductQuantity = InModel.ProductQuantity;
            prd.CategoryId = InModel.Category;
            prd.ModifiedBy = User.Identity?.Name;
            prd.ModifiedDate = DateTime.UtcNow;

            if (InModel?.ProductImage != null)
            {
                // delete existing image
                var existingImage = prd.Images.FirstOrDefault(c => c.IsPrimaryImage);

                if (existingImage != null)
                {
                    _imageBaseStore.DeleteEntity(existingImage);
                }

                var img = ProcessProductImage();
                prd.Images.Add(img);
            }


            if (InModel?.ProductImages != null)
            {
                // delete existing images
                var existingImages = prd.Images.Where(c => !c.IsPrimaryImage).ToList();

                if (existingImages.Any())
                {
                    _imageBaseStore.DeleteImages(existingImages, CacheKey.UploadImage, true);
                }

                var images = ProcessAdditionalProductImages();
                prd.Images.AddRange(images);
            }

            if (InModel?.ProductImage != null || InModel?.ProductImages != null) return prd;

            // To skip validation
            InModel.ProductImage = new FormFile(null!, 0, 0, null!, null!);
            InModel.ProductImages = new List<IFormFile>();

            return prd;
        }

        private Image ProcessProductImage()
        {
            var image = new Image();
            try
            {
                if (InModel.ProductImage == null)
                {
                    StatusMessage = "Error. Please upload at least one image";
                    ModelState.AddModelError("File Upload", "Please upload at least one image");
                    return image;
                }

                using var ms = new MemoryStream();
                InModel.ProductImage.CopyTo(ms);


                // upload the file if less than 2 MB
                if (ms.Length <= 2097152)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(InModel.ProductImage.ContentDisposition).FileName.Trim();
                    var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
                    var fileExtension = fileName.Value[(index + 1)..];

                    if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg" or "heif" or "heic")
                    {
                        image.ImageId = Guid.NewGuid();
                        image.UploadedImage = ms.ToArray();
                        image.IsPrimaryImage = true;
                        image.Enabled = true;
                        image.CreatedDate = DateTime.UtcNow;
                        image.CreatedBy = User.Identity?.Name;
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
                    StatusMessage = $"Error. The file {fileName} is too large. Must be less than 2 MB";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading file for product");
                StatusMessage = "Error. Can not upload file";
                throw;
            }

            return image;
        }

        private List<Image> ProcessAdditionalProductImages()
        {
            var images = new List<Image>();

            try
            {
                if (InModel.ProductImages == null || !InModel.ProductImages.Any())
                {
                    return images;
                }

                var errFile = string.Empty;

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
                            var image = new Image
                            {
                                ImageId = Guid.NewGuid(),
                                UploadedImage = ms1.ToArray(),
                                IsPrimaryImage = false,
                                Enabled = true,
                                CreatedDate = DateTime.UtcNow,
                                CreatedBy = User.Identity?.Name
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
                        errFile += $"{fileName}, ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(errFile))
                {
                    StatusMessage = $"Error. Files {errFile}uploaded is too large. Must be less than 2 MB";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading Multiple files for product");
                throw;
            }

            return images;
        }

        private List<Image> ValidateFileUploads()
        {
            var images = new List<Image>();
            try
            {
                var image = ProcessProductImage();
                images.Add(image);

                var additionalImages = ProcessAdditionalProductImages();
                images.AddRange(additionalImages);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading file");
                StatusMessage = "Error validating file";
            }

            return images;
        }

        public class InputModel
        {
            private const int DefaultCategoryId = 1;
            public int Id { get; set; }

            [Required, Display(Name = "Product Page")]
            public string RazorPageId { get; set; } = default!;

            [Required, Display(Name = "Category")]
            public int Category { get; set; } = DefaultCategoryId;

            [Required, Display(Name = "Product Quantity"), Range(1, 1000)]
            public int ProductQuantity { get; set; }

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

            [Display(Name = "Product Image")]
            public IFormFile ProductImage { get; set; } = default!;

            [Display(Name = "Product files for sale")]
            public IEnumerable<IFormFile> ProductImages { get; set; } = default!;

            [Display(Name = "On Sale?")]
            public bool IsOnSale { get; set; }

            [Display(Name = "New Product")]
            public bool IsNewProduct { get; set; } = true;

            [Display(Name = "Trending")]
            public bool IsTrending { get; set; }

            public string Gender { get; set; } = "All";

            [Display(Name = "Mini")]
            public bool HasMini { get; set; }
            [Display(Name = "Midi")]
            public bool HasMidi { get; set; }
            [Display(Name = "Maxi")]
            public bool HasMaxi { get; set; }
            public string? SelectedRingSizes { get; set; }

        }

        public class TestCaptions
        {
            private string _profileImageEditText = string.Empty;
            private string _profileImagesEditText = string.Empty;

            public TestCaptions()
            {
                TitleCaption = "Add New Product";
                BtnCaption = "Upload Product";
            }
            public string TitleCaption { get; set; }

            public string BtnCaption { get; set; }

            public bool IsEditing { get; set; }

            public string ProfileImageEditText
            {
                get => _profileImageEditText;
                set
                {
                    _profileImageEditText = value;
                    _profileImageEditText = IsEditing ? "Uploading a new image will replace the current image" : string.Empty;
                }
            }

            public string ProfileImagesEditText
            {
                get => _profileImagesEditText;
                set
                {
                    _profileImagesEditText = value;
                    _profileImagesEditText = IsEditing ? "Uploading one or more images will remove all current Images" : string.Empty;
                }
            }
        }
    }
}
