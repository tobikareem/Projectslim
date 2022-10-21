using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Slim.Pages.Areas.Identity.Pages.Account.Manage
{
    public class CategoryModel : PageModel
    {
        public List<SelectListItem> RazorPageSelectList { get; set; }

        public CategoryModel()
        {
            RazorPageSelectList = new List<SelectListItem>();
        }

        [BindProperty] public InputModel Input { get; set; } = new();

        public void OnGet()
        {
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
