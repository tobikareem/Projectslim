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
        private readonly IBaseStore<Category> _categoryRepository;
        private readonly ILogger<CategoryModel> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEnumerable<Category> _categories;
        private readonly IBaseStore<RazorPage> _razorPagesBaseStore;
        private readonly IEnumerable<RazorPage> _razorPages;

        public List<SelectListItem> RazorPageSelectList { get; set; } = new();
        public List<SelectListItem> CategorySelectList { get; set; } = new();

        public CategoryModel(IBaseStore<Category> categoryRepository, ILogger<CategoryModel> logger, ICacheService cacheService, IBaseStore<RazorPage> razorPagesBaseStore)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _cacheService = cacheService;
            _razorPagesBaseStore = razorPagesBaseStore;

            _categories = _cacheService.GetOrCreate(CacheKey.ProductCategories, _categoryRepository.GetAll, 60);
            _razorPages = _cacheService.GetOrCreate(CacheKey.GetRazorPages, _razorPagesBaseStore.GetAll).Where(x => x.PageName != "Home");
        }

        [BindProperty] public InputModel Input { get; set; } = new();
        [TempData] public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            Init();
        }

        public IActionResult OnPostAsync()
        {

            if (string.IsNullOrWhiteSpace(Input.CategoryName))
            {
                StatusMessage = "Error. Category Name can not be empty.";
                Init();
                return Page();
            }

            if (_categories.Any(c => c.CategoryName == Input.CategoryName))
            {
                ModelState.AddModelError(string.Empty, "Category already exists");
                _logger.LogWarning("The Category {categoryName} already exists", Input.CategoryName);
                Init();
                return Page();
            }

            var category = new Category
            {
                CategoryName = Input.CategoryName,
                CategoryDescription = Input.CategoryDescription,
                CategoryTags = Input.CategoryTag,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name,
                RazorPageId = Convert.ToInt32(Input.RazorPageId)
            };

            _categoryRepository.AddEntity(category, CacheKey.ProductCategories, true);

            _logger.LogInformation("Category created a new category with ID '{CategoryID}'", category.Id);

            return RedirectToPage("./AddNewProduct");
        }

        public IActionResult OnPostEditCategoryInfo()
        {
            var category = _categories.FirstOrDefault(c => c.Id == Convert.ToInt32(Input.CategoryEditId));

            var hasChanges = false;

            if (Input.CanDelete && category != null)
            {
                _categoryRepository.DeleteEntity(category, CacheKey.ProductCategories, true);
                return RedirectToPage("./AddNewProduct");
            }

            if (category == null)
            {
                Init();
                return Page();
            }

            if (category.CategoryName != Input.CategoryEditName)
            {
                hasChanges = true;
                category.CategoryName = Input.CategoryEditName;
            }

            if (category.CategoryDescription != Input.CategoryEditDescription)
            {
                hasChanges = true;
                category.CategoryDescription = Input.CategoryEditDescription;
            }

            if (category.CategoryTags != Input.CategoryEditTag)
            {
                hasChanges = true;
                category.CategoryTags = Input.CategoryEditTag;
            }

            if (hasChanges)
            {
                _categoryRepository.UpdateEntity(category, CacheKey.ProductCategories, true);
            }
            return RedirectToPage("./AddNewProduct");
        }

        private void Init()
        {
            // start by showing bag information first
            var pageIdForBag = _razorPages.FirstOrDefault(x => string.Compare(x.PageName, "Bags", StringComparison.OrdinalIgnoreCase) == 0)?.Id ?? 1;
            RazorPageSelectList = _razorPages.Select(page => new SelectListItem { Text = page.PageName, Value = page.Id.ToString() }).ToList();

            var editCategory = _categories.Where(x => x.RazorPageId == pageIdForBag).ToList();
            CategorySelectList =editCategory.Select(category => new SelectListItem { Text = category.CategoryName, Value = category.Id.ToString() }).ToList();

            Input = new InputModel
            {
                CategoryEditName = editCategory.FirstOrDefault()?.CategoryName ?? string.Empty,
                CategoryEditDescription = editCategory.FirstOrDefault()?.CategoryDescription ?? string.Empty,
                CategoryEditTag = editCategory.FirstOrDefault()?.CategoryTags ?? string.Empty
            };
        }

        public class InputModel
        {

            [Required, Display(Name = "Select a Product Type")]
            public string RazorPageId { get; set; } = default!;

            [Required, Display(Name = "Name the category")]
            public string CategoryName { get; set; } = default!;

            [Display(Name = "Describe the category")]
            public string CategoryDescription { get; set; } = default!;

            [Display(Name = "Tags")]
            public string CategoryTag { get; set; } = default!;

            [Display(Name = "Select Old Category to Edit")]
            public string CategoryEditId { get; set; } = default!;

            [Display(Name = "New Category to Edit")]
            public string CategoryEditType { get; set; } = default!;

            [Display(Name = "New Category Name")]
            public string CategoryEditName { get; set; } = default!;

            [Display(Name = "New Category Description")]
            public string CategoryEditDescription { get; set; } = default!;

            [Display(Name = "New Category Tags")]
            public string CategoryEditTag { get; set; } = default!;

            [Display(Name = "Delete existing category")]
            public bool CanDelete { get; set; }
        }
    }
}
