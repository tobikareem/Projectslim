

using Slim.Data.Model;

namespace Slim.Data.Entity;

public class Comment : GeneralEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserComment { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;
}