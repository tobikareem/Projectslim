using System.Security.Principal;
using Slim.Data.Model;

namespace Slim.Data.Entity;

public class ProductDetail : GeneralEntity
{

    public ProductDetail()
    {
        
    }

    public int Id { get; set; }
    
    public string ShoeSize { get; set; } = string.Empty;
    public string JewelrySize { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public bool HasMini { get; set; }
    public bool HasMidi { get; set; }
    public bool HasMaxi { get; set; }
    
    public int ProductId { get; set; }

    public virtual Product Product { get; set; }


}