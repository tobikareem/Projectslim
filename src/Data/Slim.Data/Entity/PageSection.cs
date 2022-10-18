using Slim.Data.Model;

namespace Slim.Data.Entity
{
    public sealed partial class PageSection : GeneralEntity
    {
        public PageSection()
        {
            PageSectionResources = new HashSet<PageSectionResource>();
        }

        public int Id { get; set; }
        public int RazorPageId { get; set; }
        public string PageSectionName { get; set; } = null!;
        public string? Description { get; set; }
        public bool HasImage { get; set; }

        public RazorPage RazorPage { get; set; } = null!;
        public ICollection<PageSectionResource> PageSectionResources { get; set; }
    }
}
