using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slim.Data.Model;

namespace Slim.Data.Entity
{
    public class UserPageImage : GeneralEntity
    {
        [Key]
        public Guid ImageId { get; set; }
        public byte[] UploadedImage { get; set; } = null!;
        public string? ImageName { get; set; }
        public string ImageDescription { get; set; } = string.Empty;
    }
}
