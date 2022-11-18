using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Slim.Core.Model;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Pages.Pages
{
    public class HairModel : PageModel
    {
        private readonly ILogger<HairModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IPageSection _pageSectionsBaseStore;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly int _pageId;

        public HairModel(ILogger<HairModel> logger, ICacheService cacheService, IPageSection pageSectionsBaseStore, IBaseStore<RazorPage> razorPagesBaseStore)
        {
            _logger = logger;
            _cacheService = cacheService;
            _pageSectionsBaseStore = pageSectionsBaseStore;
            _razorPagesBaseStore = razorPagesBaseStore;

            var razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll);
            _pageId = razorPages
                          .FirstOrDefault(x =>
                              string.Compare(x.PageName, "Hair", StringComparison.OrdinalIgnoreCase) == 0)?.Id ??
                      0;
        }

        [BindProperty(SupportsGet = true)] public List<PageSection> PageSections { get; set; } = new();

        public void OnGet()
        {
            PageSections = _cacheService.GetOrCreate(CacheKey.GetPageSections, _pageSectionsBaseStore.GetAll).Where(x => x.RazorPageId == _pageId).ToList();

        }
    }
}
