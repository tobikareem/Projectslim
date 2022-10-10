using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class WebPagePhotosModel : PageModel
    {
        private readonly SlimDbContext _context;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly ICacheService _cacheService;
        private readonly ILogger<WebPagePhotosModel> _logger;

        private readonly IBaseStore<PageSection> _pageSectionsBaseStore;

        public List<PageSection> PageSections;
        public List<InputModel> InputModels;

        public List<SelectListItem> RazorPageSelectList { get; set; }
        public WebPagePhotosModel(SlimDbContext context, IBaseStore<RazorPage> razorPagesBaseStore, ICacheService cacheService, ILogger<WebPagePhotosModel> logger, IBaseStore<PageSection> pageSectionsBaseStore)
        {
            _context = context;
            _razorPagesBaseStore = razorPagesBaseStore;
            _cacheService = cacheService;
            _logger = logger;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            
            RazorPageSelectList = new List<SelectListItem>();
            PageSections = new List<PageSection>();
            InputModels = new List<InputModel>();


            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            RazorPageSelectList = razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public InputModel InModel { get; set; } = new();


        public PageImage PageImage { get; set; } = default!;

        [HttpPost]
        public async Task<IActionResult> OnPostSaveWording(string sections)
        {
            return new JsonResult("This is me");
        }

        [HttpGet]
        public async Task<IActionResult> OnGetWording(string sections)
        {
            return new JsonResult("This is me");
        }

        public  IActionResult OnPostAllSections()
        {
            try
            {

                if (string.IsNullOrWhiteSpace(InModel.RazorPageId))
                {
                    ModelState.TryAddModelError(nameof(InModel.RazorPageId), "Please select a Page to make edits");
                    return Page();
                }
                var allSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll);

                var pageId = Convert.ToInt32(InModel.RazorPageId);
                PageSections = allSections.Where(x => x.RazorPageId == pageId).ToList();

                foreach (var inputModel in PageSections.Select(section => new InputModel
                         {
                             PageSectionName = section.PageSectionName,
                             Description = section.Description,
                             PageSectionId = section.Id,
                             HasImage = section.HasImage
                        
                         }))
                {
                    InputModels.Add(inputModel);
                }

                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError("... Exception occurred while Posting . {message}", e.Message);
                return Page();
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
