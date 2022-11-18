using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class WebPagePhotosModel : PageModel
    {
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IPageSection _pageSectionsBaseStore;
        private readonly ICacheService _cacheService;
        private readonly ILogger<WebPagePhotosModel> _logger;
        public List<SelectListItem> RazorPageSelectList { get; set; }
        public Image PageImage { get; set; } = default!;

        public List<InputModel> InputModels { get; set; } = new();

        public WebPagePhotosModel(IBaseStore<RazorPage> razorPagesBaseStore, IPageSection pageSectionsBaseStore, ICacheService cacheService, ILogger<WebPagePhotosModel> logger )
        {
            _razorPagesBaseStore = razorPagesBaseStore;
            _cacheService = cacheService;
            _logger = logger;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            RazorPageSelectList = new List<SelectListItem>();

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
        }

        [BindProperty(SupportsGet = true)] public InputModel InModel { get; set; } = new();
        [BindProperty] public List<InputModel> WordingModel { get; set; } = new();


        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPostSaveWording()
        {
            var pageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();
            
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

                // TODO: Set the Modified By 
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

               // return Page();
            }
            catch (Exception e)
            {
                _logger.LogError("... Exception occurred while Posting . {message}", e.Message);
               // return Page();
            }
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
    }
}
