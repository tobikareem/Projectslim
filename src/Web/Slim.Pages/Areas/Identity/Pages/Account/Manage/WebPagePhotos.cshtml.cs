using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Pages.Extensions;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class WebPagePhotosModel : PageModel
    {
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IPageSection _pageSectionsBaseStore;
        private readonly ICacheService _cacheService;
        private readonly ILogger<WebPagePhotosModel> _logger;
        private readonly IBaseStore<UserPageImage> _userPagesBaseStore;
        public List<SelectListItem> RazorPageSelectList { get; set; }
        public Image PageImage { get; set; } = default!;

        public List<InputModel> InputModels { get; set; } = new();

        public WebPagePhotosModel(IBaseStore<RazorPage> razorPagesBaseStore, IPageSection pageSectionsBaseStore, ICacheService cacheService, ILogger<WebPagePhotosModel> logger, IBaseStore<UserPageImage> userPagesBaseStore)
        {
            _razorPagesBaseStore = razorPagesBaseStore;
            _cacheService = cacheService;
            _logger = logger;
            _userPagesBaseStore = userPagesBaseStore;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            RazorPageSelectList = new List<SelectListItem>();

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
        }

        [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();
        [BindProperty] public List<InputModel> WordingModel { get; set; } = new();
        [BindProperty] public ImageModel ImgModel { get; set; } = new();
        [TempData] public string StatusMessage { get; set; } = string.Empty;


        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPostSaveWording()
        {
            try
            {
                var pageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();

                if (WordingModel == null)
                {
                    return Page();
                }

                var wording = pageSections.Where(x => !x.HasImage && x.RazorPageId == int.Parse(WordingModel.First().RazorPageId)).ToList();
                var hasChanges = false;

                for (var i = 0; i < WordingModel.Count; i++)
                {
                    if (wording[i].Description == WordingModel[i].Description)
                    {
                        continue;
                    }
                    wording[i].Description = WordingModel[i].Description;
                    wording[i].ModifiedDate = DateTime.UtcNow;

                    wording[i].ModifiedBy = User?.Identity?.Name;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    _pageSectionsBaseStore.UpdatePageSections(pageSections);
                }
                var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
                var currentPage = razorPages.First(x => x.Id == int.Parse(WordingModel.First().RazorPageId)).Url;
                return RedirectToPage($"{currentPage}");
            }
            catch (Exception e)
            {
                _logger.LogError("... Error Saving wording to the database. {message}", e.Message);
            }

            return Page();
        }


        public void OnPostAllSections()
        {
            try
            {

                if (string.IsNullOrWhiteSpace(InModel.RazorPageId))
                {
                    ModelState.TryAddModelError(nameof(InModel.RazorPageId), "Please select a Page to make edits");
                    // return Page();
                }

                var allSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll);

                InputModels = allSections.Where(x => x.RazorPageId == int.Parse(InModel.RazorPageId)).Select(s => new InputModel
                {
                    PageSectionName = s.PageSectionName,
                    Description = s.Description,
                    PageSectionId = s.Id,
                    HasImage = s.HasImage,
                    RazorPageId = InModel.RazorPageId
                }).ToList();

                WordingModel = InputModels.Where(x => !x.HasImage).ToList();

                GetHomePageImages();

                // return Page();
            }
            catch (Exception e)
            {
                _logger.LogError("... Exception occurred while Posting . {message}", e.Message);
                // return Page();
            }
        }

        public async Task<IActionResult> OnPostSavePhotos()
        {
            var images = _cacheService.GetOrCreate(CacheKey.GetHomePageImages, _userPagesBaseStore.GetAll, 60).ToList();
            if (ImgModel.BagImage != null)
            {
                var bagImage = await ProcessImageAsync(ImgModel.BagImage, "HomePage", "HomePage Bag Image");

                var existing = images.FirstOrDefault(x => x.ImageDescription == "HomePage Bag Image");
                if (existing == null)
                {
                    _userPagesBaseStore.AddEntity(bagImage, CacheKey.GetHomePageImages, true);
                }
                else
                {
                    existing.UploadedImage = bagImage.UploadedImage;
                    _userPagesBaseStore.UpdateEntity(existing, CacheKey.GetHomePageImages, true);
                }
            }

            if (ImgModel.ShoeImage != null)
            {
                var shoeImage = await ProcessImageAsync(ImgModel.ShoeImage, "HomePage", "HomePage Shoe Image");

                var existing = images.FirstOrDefault(x => x.ImageDescription == "HomePage Shoe Image");
                if (existing == null)
                {
                    _userPagesBaseStore.AddEntity(shoeImage, CacheKey.GetHomePageImages, true);
                }
                else
                {
                    existing.UploadedImage = shoeImage.UploadedImage;
                    _userPagesBaseStore.UpdateEntity(existing, CacheKey.GetHomePageImages, true);
                }
            }

            if (ImgModel.AccessoryImage != null)
            {
                var accessImage = await ProcessImageAsync(ImgModel.AccessoryImage, "HomePage", "HomePage Accessory Image");

                var existing = images.FirstOrDefault(x => x.ImageDescription == "HomePage Accessory Image");
                if (existing == null)
                {
                    _userPagesBaseStore.AddEntity(accessImage, CacheKey.GetHomePageImages, true);
                }
                else
                {
                    existing.UploadedImage = accessImage.UploadedImage;
                    _userPagesBaseStore.UpdateEntity(existing, CacheKey.GetHomePageImages, true);
                }
            }

            return RedirectToPage("/Index");
        }

        private void GetHomePageImages()
        {
            var images = _cacheService.GetOrCreate(CacheKey.GetHomePageImages, _userPagesBaseStore.GetAll, 60);

            var homePageImages = images?.Where(x => x.ImageName == "HomePage").ToList();

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

        public string GetImageStr(byte[]? image)
        {
            var imgSrc = string.Empty;
            return imgSrc.GetImageSrc(image); ;
        }

        private async Task<UserPageImage> ProcessImageAsync(IFormFile formFile, string imgName, string imgDescription)
        {
            var image = new UserPageImage();

            try
            {
                await using var ms = new MemoryStream();
                await formFile.CopyToAsync(ms);

                // upload the file is less than 2MB

                if (ms.Length < 2097152)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim();
                    var index = fileName.Value.LastIndexOf(".", StringComparison.Ordinal);
                    var fileExtension = fileName.Value[(index + 1)..];

                    if (fileExtension.ToLowerInvariant() is "jpg" or "png" or "jpeg" or "heic")
                    {
                        image.ImageId = Guid.NewGuid();
                        image.UploadedImage = ms.ToArray();
                        image.ImageDescription = imgDescription;
                        image.Enabled = true;
                        image.ImageName = imgName;
                        image.CreatedDate = DateTime.UtcNow;
                        image.CreatedBy = User.Identity?.Name;
                    }
                    else
                    {
                        StatusMessage = "Please upload a valid image file (jpg, png, jpeg, heic)";
                    }
                }
            }
            catch (Exception e)
            {
                StatusMessage = "Error uploading file for product" + e.Message;
                throw;
            }

            return image;
        }

        public class InputModel
        {

            [Required, Display(Name = "Please, Select the Page you want to edit.")]
            public string RazorPageId { get; set; } = default!;

            public string? PageSectionName { get; set; }

            public string? Description { get; set; }

            public int PageSectionId { get; set; }
            public bool HasImage { get; set; }

        }

        public class ImageModel
        {
            public IFormFile? BagImage { get; set; } = default!;
            public IFormFile? ShoeImage { get; set; } = default!;
            public IFormFile? AccessoryImage { get; set; } = default!;

            public string ImageBag { get; set; } = string.Empty;
            public string ImageShoe { get; set; } = string.Empty;
            public string ImageAccess { get; set; } = string.Empty;
        }
    }
}
