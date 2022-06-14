using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;
using Slim.Shared.Repositories;
using Slim.Shared.Services;

namespace Slim.Pages.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddCustomServicesExtension(this IServiceCollection services, WebApplicationBuilder builder)
        {

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'SlimDbContextConnection' not found.");
            builder.Services.AddDbContext<SlimDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<SlimDbContext>();

            builder.Services.AddRazorPages();


            #region Add Service Injection

            builder.Services.AddSingleton<ICacheService, CacheService>();

            
            builder.Services.AddScoped<IBaseStore<RazorPage>, RazorPagesRepo>();
            builder.Services.AddScoped<IBaseStore<PageSection>, PageSectionRepo>();


            #endregion



            return services;
        }
    }
}
