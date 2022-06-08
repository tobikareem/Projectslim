using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Slim.Data.Context
{
    public class SlimDbContext : IdentityDbContext
    {
        public SlimDbContext(DbContextOptions<SlimDbContext> options)
            : base(options)
        {
        }
    }
}