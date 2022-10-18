namespace Slim.Data.Entity
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ImageId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual Image? Image { get; set; }
    }
}
