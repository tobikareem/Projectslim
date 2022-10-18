using Slim.Data.Model;

namespace Slim.Data.Entity
{
    public sealed partial class Image : GeneralEntity
    {
        public Image()
        {
            ProductImages = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public Guid ImageId { get; set; }
        public byte[] UploadedImage { get; set; } = null!;
        
        public bool IsPrimaryImage { get; set; }


        public Product Product { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; }
     
    }
}
