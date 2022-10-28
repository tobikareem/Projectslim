

using Slim.Data.Model;

namespace Slim.Data.Entity
{
    public sealed class Category : GeneralEntity
    {

        public Category()
        {
            Products = new HashSet<Product>();
        }
        
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string CategoryDescription { get; set; } = string.Empty;

        public string CategoryTags { get; set; } = string.Empty;

        public int RazorPageId { get; set; }

        public ICollection<Product> Products { get; set; }

        public RazorPage RazorPage { get; set; }

    }
}
