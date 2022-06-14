

namespace Slim.Data.Model
{
    public class GeneralEntity
    {
        public bool? Enabled { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }        
    }
}
