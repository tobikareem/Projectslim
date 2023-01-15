using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class WebPagesModel : PageModel
    {
        private readonly IBaseStore<RazorPage> _store;
        private readonly ILogger<WebPagesModel> _logger;
        private readonly ICacheService _cacheService;
        public List<SelectListItem> RazorPageSelectList { get; set; }
        public List<RazorPage> RazorPages { get; set; }

        public WebPagesModel(IBaseStore<RazorPage> store, ILogger<WebPagesModel> logger, ICacheService cacheService)
        {
            _store = store;
            _logger = logger;
            _cacheService = cacheService;

            StatusMessage = string.Empty;
            RazorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _store.GetAll, 60).Skip(1).ToList();
            RazorPageSelectList = RazorPages.Select(x => new SelectListItem { Text = x.PageName, Value = x.Id.ToString() }).ToList();
        }

        [BindProperty] public InputModel Input { get; set; } = new();
        [TempData] public string StatusMessage { get; set; }
        
        public void OnGet()
        {
            if (!RazorPages.Any())
            {
                return;
            }

            var detail = RazorPages.First();
            Input = new InputModel
            {
                Id =detail.Id,
                PageName = detail.PageName,
                Description = detail.Description,
                Url = detail.Url
            };
        }

        public JsonResult OnGetPageSelected(int id)
        {
            var page = RazorPages.FirstOrDefault(x => x.Id == id);

            if (page == null)
            {
                return new JsonResult(null);
            }

            var result = new
            {
                pageName = page.PageName,
                description = page.Description,
                url = page.Url
            };
            return new JsonResult(result);
        }

        public IActionResult OnPostEditPages()
        {
            var page = RazorPages.FirstOrDefault(x => x.Id == Input.Id);

            if (page == null)
            {
                return Page();
            }

            page.PageName = Input.PageName;
            page.Description = Input.Description;
            page.Url = Input.Url;

            _store.UpdateEntity(page, CacheKey.GetRazorPages, true);

            return RedirectToPage("./WebPagePhotos");
        }

        public IActionResult OnPostCreatePages()
        {
            var page = RazorPages.FirstOrDefault(x => x.PageName == Input.PageName);

            if (page != null)
            {
                StatusMessage = "Error. Same name already exists.";
                return Page();
            }

            page = new RazorPage
            {
                PageName = Input.PageName,
                Description = Input.Description,
                Url = Input.Url
            };

            _store.AddEntity(page, CacheKey.GetRazorPages, true);
            return RedirectToPage("./WebPagePhotos");
        }

        public class InputModel
        {
            public int Id { get; set; }

            [Display(Name = "New Product Type")]
            public string PageName { get; set; } = null!;
            public string? Description { get; set; }
            public string? Url { get; set; }
        }
    }
}
