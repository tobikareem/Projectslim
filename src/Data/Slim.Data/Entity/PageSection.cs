namespace Slim.Data.Entity
{
    public sealed partial class PageSection
    {
        public PageSection()
        {
            PageSectionImages = new HashSet<PageSectionImage>();
            PageSectionResources = new HashSet<PageSectionResource>();
        }

        public int Id { get; set; }
        public int RazorPageId { get; set; }
        public string PageSectionName { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Enabled { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public RazorPage RazorPage { get; set; } = null!;
        public ICollection<PageSectionImage> PageSectionImages { get; set; }
        public ICollection<PageSectionResource> PageSectionResources { get; set; }
    }
}
