using Slim.Data.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slim.Data.Entity
{
    public partial class Product : GeneralEntity
    {

        public Product()
        {
            Images = new HashSet<Image>();
            ProductImages = new HashSet<ProductImage>();
            Comments = new HashSet<Comment>();
            Reviews = new HashSet<Review>();
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int Id { get; set; }
        public int RazorPageId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string? ProductTags { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsNewProduct { get; set; }
        public bool IsTrending { get; set; }
        public int ProductQuantity { get; set; }
        public int? CategoryId { get; set; }
        [NotMapped] public bool IsProductInCart { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual RazorPage RazorPage { get; set; }
        public virtual Category Category { get; set; }

    }
}
