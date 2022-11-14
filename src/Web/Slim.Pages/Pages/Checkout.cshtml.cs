using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Slim.Pages.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
