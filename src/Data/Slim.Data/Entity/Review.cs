using Slim.Data.Model;

namespace Slim.Data.Entity;

public class Review : GeneralEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserReview { get; set; } = string.Empty;

    public int ProductId { get; set; } 
    public int Rating { get; set; }

    public string? Pros { get; set; } = string.Empty;

    public string? Cons { get; set; } = string.Empty;

    public virtual Product Product { get; set; } = null!;

}