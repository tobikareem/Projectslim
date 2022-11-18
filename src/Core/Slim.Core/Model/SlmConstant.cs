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
    }
}
