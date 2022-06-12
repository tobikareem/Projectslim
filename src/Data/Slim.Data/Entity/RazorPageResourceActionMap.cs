namespace Slim.Data.Entity
{
    public partial class RazorPageResourceActionMap
    {
        public int Id { get; set; }
        public int RazorPageId { get; set; }
        public int ResourceActionId { get; set; }

        public virtual RazorPage RazorPage { get; set; } = null!;
        public virtual ResourceAction ResourceAction { get; set; } = null!;
    }
}
