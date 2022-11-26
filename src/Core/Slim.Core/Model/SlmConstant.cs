namespace Slim.Core.Model
{
    public static class SlmConstant
    {
        public static List<string> PagesForDropDown
        {
            get
            {
                var pages = new List<string>
                {
                    "Hair",
                    "Lip Gloss",
                    "Lashes"
                };

                return pages;
            }
        }

        public static List<string> AdminEmailList
        {
            get
            {
                var emails = new List<string>
                {
                    "elizabeth.lucys@hotmail.com",
                    "captain@tobikareem.com",
                    "odunayoadegbaju@yahoo.com"
                };

                return emails;
            }
        }


        public const string SessionKeyName = "CartUserId";

        public static List<string> EssentialAddressModel
        {
            get
            {
                return new List<string>{
                nameof(AddressModel.FirstName),
                nameof(AddressModel.LastName),
                nameof(AddressModel.Email),
                nameof(AddressModel.Address1),
                nameof(AddressModel.ZipCode)
            };
            }
        }
        public static List<string> EssentialBillingAddressModel
        {
            get
            {
                return new List<string>{
                nameof(AddressModel.BillingAddress1),
                nameof(AddressModel.BillingAddress2),
                nameof(AddressModel.BillingZipCode)
            };
            }
        }
    }
}
