namespace Slim.Data.Entity
{
    public sealed partial class RazorPage
    {
        public RazorPage()
        {
            PageSectionImages = new HashSet<PageSectionImage>();
            PageSectionResources = new HashSet<PageSectionResource>();
            PageSections = new HashSet<PageSection>();
            RazorPageResourceActionMaps = new HashSet<RazorPageResourceActionMap>();
        }

        public int Id { get; set; }
        public string PageName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Url { get; set; }
        public bool? Enabled { get; set; }

        public ICollection<PageSectionImage> PageSectionImages { get; set; }
        public ICollection<PageSectionResource> PageSectionResources { get; set; }
        public ICollection<PageSection> PageSections { get; set; }
        public ICollection<RazorPageResourceActionMap> RazorPageResourceActionMaps { get; set; }
    }
}
