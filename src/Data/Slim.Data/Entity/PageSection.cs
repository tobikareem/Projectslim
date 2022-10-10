using Slim.Data.Model;

namespace Slim.Data.Entity
{
    public partial class PageSection : GeneralEntity
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
        public bool HasImage { get; set; }

        public virtual RazorPage RazorPage { get; set; } = null!;
        public virtual ICollection<PageSectionImage> PageSectionImages { get; set; }
        public virtual ICollection<PageSectionResource> PageSectionResources { get; set; }
    }
}
