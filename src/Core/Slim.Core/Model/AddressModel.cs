using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slim.Core.Model
{
    public class AddressModel
    {
        [Required, Display(Name = "First Name"), StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Last Name"), StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, Display(Name = "E-mail Address"), DataType(DataType.EmailAddress), StringLength(50), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Display(Name = "Phone Number"), DataType(DataType.PhoneNumber), StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, Display(Name = "Same as Shipping Address")]
        public bool IsSameAsAddress { get; set; }

        [Required, Display(Name = "ZIP Code"), StringLength(50)]
        public string ZipCode { get; set; } = string.Empty;

        [Required, Display(Name = "Address 1"), StringLength(50)]
        public string Address1 { get; set; } = string.Empty;

        [Display(Name = "Address 2"), StringLength(50)]
        public string Address2 { get; set; } = string.Empty;

        // Shipping Address

        [Required, Display(Name = "ZIP Code"), StringLength(50)]
        public string BillingZipCode { get; set; } = string.Empty;

        [Required, Display(Name = "Address 1"), StringLength(50)]
        public string BillingAddress1 { get; set; } = string.Empty;

        [Display(Name = "Address 2"), StringLength(50)]
        public string BillingAddress2 { get; set; } = string.Empty;

        // Profile Image
        //[Display(Name = "Product Image")]
        //public IFormFile ProfileImage { get; set; }
    }
}
