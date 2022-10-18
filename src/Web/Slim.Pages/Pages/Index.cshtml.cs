using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using System.ComponentModel.DataAnnotations;

namespace Slim.Pages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IPageSection<PageSection> _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly int _pageId;
        

        public IndexModel(ILogger<IndexModel> logger, ICacheService cacheService, IPageSection<PageSection> pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _pageId = razorPages
                         .FirstOrDefault(x =>
                             string.Compare(x.PageName, "Home", StringComparison.OrdinalIgnoreCase) == 0)?.Id ??
                     0;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();

        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).ToList();

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