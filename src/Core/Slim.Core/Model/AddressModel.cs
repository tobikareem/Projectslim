using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Slim.Core.Model
{
    public class AddressModel
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; } = string.Empty;

        [Display(Name = "E-mail Address"), DataType(DataType.EmailAddress)]
        public string? Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number"), DataType(DataType.PhoneNumber), Phone]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Same as Shipping Address")]
        public bool IsSameAsAddress { get; set; }

        [Display(Name = "ZIP Code")]
        public string? ZipCode { get; set; } = string.Empty;

        [Display(Name = "Address 1")]
        public string? Address1 { get; set; } = string.Empty;

        [Display(Name = "Address 2")]
        public string? Address2 { get; set; } = string.Empty;

        // Shipping Address

        [Display(Name = "ZIP Code")]
        public string? BillingZipCode { get; set; } = string.Empty;

        [Display(Name = "Address 1")]
        public string? BillingAddress1 { get; set; } = string.Empty;

        [Display(Name = "Address 2")]
        public string? BillingAddress2 { get; set; } = string.Empty;

        [Display(Name = "Product Image")]
        public IFormFile? ProfileImage { get; set; }
    }
}
