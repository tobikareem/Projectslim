namespace Slim.Core.Model
{
    public class ImageData
    {

        public int Id { get; set; }
        public Guid ImageId { get; set; }
        public byte[] UploadedImage { get; set; } = null!;

        public bool IsPrimaryImage { get; set; }

        public bool Enabled { get; set; }
    }
}
