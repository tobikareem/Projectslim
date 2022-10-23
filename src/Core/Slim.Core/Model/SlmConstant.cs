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
    }
}
