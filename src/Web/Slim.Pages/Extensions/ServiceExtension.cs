using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Slim.Core.Model;
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
            builder.Services.AddDbContext<SlimDbContext>(options =>options.UseSqlServer(connectionString));
            
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequiredLength = 6;
                    
                })
                .AddEntityFrameworkStores<SlimDbContext>();
            builder.Services.AddRazorPages();
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            #region Add Service Injection
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddScoped<IBaseStore<RazorPage>, RazorPagesRepo>();
            builder.Services.AddScoped<IPageSection<PageSection>, PageSectionRepo>();
            builder.Services.AddScoped<IImage<Image>, ImageUploadRepo>();
            builder.Services.AddScoped<IBaseStore<Product>, ProductRepository>();
            builder.Services.AddScoped<IBaseStore<Category>, CategoryRepository>();
            builder.Services.AddScoped<IBaseStore<Review>, ReviewRepository>();
            builder.Services.AddScoped<IBaseStore<Comment>, CommentRepository>();
            #endregion


            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var config = builder.Configuration;
            services.Configure<ConnectionStrings>(config.GetSection(AppConfiguration.ConnectionStringsOptions));
            return services;
        }
    }
}
