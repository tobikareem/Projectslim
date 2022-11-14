using Slim.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Slim.Data.Entity
{
    public class ShoppingCart : GeneralEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // the Id of the user that is associated with item to purchase.
        // This Id will be stored as a session variable
        public string CartUserId { get; set; } = string.Empty; 
        public int Quantity { get; set; } = 0;

        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
