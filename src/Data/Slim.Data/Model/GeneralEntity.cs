

namespace Slim.Data.Model
{
    public class GeneralEntity
    {
        public bool? Enabled { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }        
    }
}
