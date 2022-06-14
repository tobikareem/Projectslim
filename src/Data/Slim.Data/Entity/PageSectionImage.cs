namespace Slim.Data.Entity
{
    public partial class PageSectionImage
    {
        public int Id { get; set; }
        public int RazorPageId { get; set; }
        public int RazorPageSectionId { get; set; }
        public int PageImageId { get; set; }

        public virtual PageImage PageImage { get; set; } = null!;
        public virtual RazorPage RazorPage { get; set; } = null!;
        public virtual PageSection RazorPageSection { get; set; } = null!;
    }
}
