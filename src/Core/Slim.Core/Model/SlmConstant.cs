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

        public static List<string> LashesCategories
        {
            get
            {
                var pages = new List<string>
                {
                    "Natural",
                    "Whipsy and Fluffy"
                };

                return pages;
            }
        }

        public static List<string> LipCategories
        {
            get
            {
                var pages = new List<string>
                {
                    "Crystal",
                    "Cocao",
                    "Forse"
                };

                return pages;
            }
        }
    }
}
