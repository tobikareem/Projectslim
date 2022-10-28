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
    public class CategoryModel : PageModel
    {
        public List<SelectListItem> CategorySelectList { get; set; }
        private readonly IBaseStore<Category> _categoryRepository;
        private readonly ILogger<CategoryModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEnumerable<Category> _categories;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;


        public List<SelectListItem> RazorPageSelectList { get; set; }
        
        private readonly IEnumerable<RazorPage> _razorPages;
        
        public CategoryModel(IBaseStore<Category> categoryRepository, ILogger<CategoryModel> logger, ICacheService cacheService, IBaseStore<RazorPage> razorPagesBaseStore)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _cacheService = cacheService;
            _razorPagesBaseStore = razorPagesBaseStore;

            _categories = _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryRepository.GetAll, 60);
            CategorySelectList = _categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName }).ToList();

            _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll).Where(x => SlmConstant.PagesForDropDown.Contains(x.PageName));

            RazorPageSelectList = _razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();
        }

        [BindProperty] public InputModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_categories.Any(c => c.CategoryName == Input.CategoryName))
            {
                ModelState.AddModelError(string.Empty, "Category already exists");
                _logger.LogWarning("The Category {categoryName} already exists", Input.CategoryName);
                return Page();
            }

            // TODO: Authenticated User
            var category = new Category
            {
                CategoryName = Input.CategoryName,
                CategoryDescription = Input.CategoryDescription,
                CategoryTags = Input.CategoryTag,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Test user",
                RazorPageId = Convert.ToInt32(Input.RazorPageId)
            };

            _categoryRepository.AddEntity(category, CacheKey.ProductCategories, true);

            _logger.LogInformation("Category created a new category with ID '{CategoryID}'", category.Id);

            return RedirectToPage("./AllProducts");
        }



        public class InputModel
        {

            [Required, Display(Name = "Product")]
            public string RazorPageId { get; set; } = default!;

            [Required, Display(Name = "Name the category")]
            public string CategoryName { get; set; } = default!;

            [Display(Name = "Describe the category")]
            public string CategoryDescription { get; set; } = default!;

            [Display(Name = "Tags")]
            public string CategoryTag { get; set; } = default!;
        }
    }
}
