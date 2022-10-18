using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Slim.Pages.Pages
{
    public class AppointmentModel : PageModel
    {
        public AppointmentModel()
        {
            
        }

        [BindProperty(SupportsGet = true)] public InputModel Input { get; set; } = new();

        public void OnGet()
        {
        }


        public class InputModel
        {
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
            [Required, Display(Name = "Date")]
            public DateTime? AppointmentDate { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Time)]
            [Required, Display(Name = "Time")]
            public DateTime? AppointmentTime { get; set; }
        }

    }



}
