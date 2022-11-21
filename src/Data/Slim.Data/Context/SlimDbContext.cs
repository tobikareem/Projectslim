using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Slim.Core.Model;
using Slim.Data.Entity;

namespace Slim.Data.Context
{
    public partial class SlimDbContext : IdentityDbContext<IdentityUser>
    {
        public SlimDbContext()
        {
        }

        public SlimDbContext(DbContextOptions<SlimDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<PageSection> PageSections { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<PageSectionResource> PageSectionResources { get; set; } = null!;
        public virtual DbSet<RazorPage> RazorPages { get; set; } = null!;
        public virtual DbSet<RazorPageResourceActionMap> RazorPageResourceActionMaps { get; set; } = null!;
        public virtual DbSet<ResourceAction> ResourceActions { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<UserPageImage> UserPageImages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);

            if (!optionsBuilder.IsConfigured)
            {
                // dotnet ef migrations add InitialSchema -o Migrations -c FortuneDbContext
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SlimWebDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image", "slm");

                entity.Property(e => e.ImageId).IsRequired().HasDefaultValue(Guid.NewGuid());
                entity.Property(e => e.UploadedImage).IsRequired();
                entity.Property(e => e.IsPrimaryImage).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "slm");

                entity.Property(e => e.RazorPageId);
                entity.Property(e => e.ProductName).IsRequired();
                entity.Property(e => e.ProductDescription).IsRequired();
                entity.Property(e => e.StandardPrice);
                entity.Property(e => e.SalePrice);
                entity.Property(e => e.ProductTags);
                entity.Property(e => e.IsOnSale).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.IsNewProduct).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.IsTrending).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.ProductQuantity);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RazorPageId)
                    .HasConstraintName("FK_Product_RazorPage");

                entity.HasMany(d => d.Images)
                    .WithOne(p => p.Product);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Product_Category");

                entity.HasMany(d => d.Comments)
                    .WithOne(p => p.Product);

                entity.HasMany(d => d.Reviews)
                    .WithOne(p => p.Product);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "slm");

                entity.Property(e => e.CategoryName).IsRequired();
                entity.Property(e => e.CategoryDescription).IsRequired();
                entity.Property(e => e.CategoryTags);
                entity.Property(e => e.RazorPageId);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasMany(d => d.Products)
                    .WithOne(p => p.Category);

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.RazorPageId)
                    .HasConstraintName("FK_Category_RazorPage").OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment", "slm");

                entity.Property(e => e.UserComment).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Email);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Comment_Product");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.ToTable("ShoppingCart", "slm");

                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Quantity);
                entity.Property(e => e.CartUserId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Product);

            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Review", "slm");

                entity.Property(e => e.UserReview).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Email);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Pros);
                entity.Property(e => e.Cons);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Review_Product");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage", "slm");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProductImage_ImageImg");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_ProductImage_ProductProd");
            });

            modelBuilder.Entity<UserPageImage>(entity =>
            {
                entity.Property(e => e.ImageId).IsRequired();
                entity.Property(e => e.UploadedImage).IsRequired();
                entity.Property(e => e.ImageDescription);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

            });

            modelBuilder.Entity<PageSection>(entity =>
            {
                entity.ToTable("PageSection", "slm");

                entity.Property(e => e.RazorPageId);
                entity.Property(e => e.PageSectionName).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(500).IsUnicode(false);
                entity.Property(e => e.HasImage).IsRequired().HasDefaultValueSql("((0))");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.PageSections)
                    .HasForeignKey(d => d.RazorPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSection_PageId");
            });

            modelBuilder.Entity<PageSectionResource>(entity =>
            {
                entity.ToTable("PageSectionResource", "slm");

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.PageSectionResources)
                    .HasForeignKey(d => d.RazorPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionResource_PageId");

                entity.HasOne(d => d.RazorPageSection)
                    .WithMany(p => p.PageSectionResources)
                    .HasForeignKey(d => d.RazorPageSectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionResource_SectionId");

                entity.HasOne(d => d.ResourceAction)
                    .WithMany(p => p.PageSectionResources)
                    .HasForeignKey(d => d.ResourceActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionResource_ResourceActionId");
            });

            modelBuilder.Entity<RazorPage>(entity =>
            {
                entity.ToTable("RazorPage", "slm");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PageName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<RazorPageResourceActionMap>(entity =>
            {
                entity.ToTable("RazorPageResourceActionMap", "slm");

                entity.HasIndex(e => e.RazorPageId, "NC_slim_PageResourceActionMap_PageID");

                entity.HasIndex(e => e.ResourceActionId, "NC_slim_PageResourceActionMap_ResourceActionID");

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.RazorPageResourceActionMaps)
                    .HasForeignKey(d => d.RazorPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RazorPageResourceActionMap_PageId");

                entity.HasOne(d => d.ResourceAction)
                    .WithMany(p => p.RazorPageResourceActionMaps)
                    .HasForeignKey(d => d.ResourceActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RazorPageResourceActionMap_ResourceActionId");
            });

            modelBuilder.Entity<ResourceAction>(entity =>
            {
                entity.ToTable("ResourceAction", "slm");

                entity.HasIndex(e => e.Enabled, "NC_slim_ResourceAction_Enabled");

                entity.HasIndex(e => new { e.ResourceAction1, e.Enabled }, "NC_slim_ResourceAction_ResourceAction_Enabled");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ResourceAction1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ResourceAction");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
