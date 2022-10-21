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
        public virtual DbSet<ProductImage> ProductImages{ get; set; } = null!;
        public virtual DbSet<PageSectionResource> PageSectionResources { get; set; } = null!;
        public virtual DbSet<RazorPage> RazorPages { get; set; } = null!;
        public virtual DbSet<RazorPageResourceActionMap> RazorPageResourceActionMaps { get; set; } = null!;
        public virtual DbSet<ResourceAction> ResourceActions { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;

        public virtual DbSet<Category> Categories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
            
            if (!optionsBuilder.IsConfigured)
            {
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
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "slm");

                entity.Property(e => e.CategoryName).IsRequired();
                entity.Property(e => e.CategoryDescription).IsRequired();
                entity.Property(e => e.CategoryTags);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy);
                entity.Property(e => e.ModifiedBy);
                entity.Property(e => e.Enabled).IsRequired().HasDefaultValueSql("((1))");

                entity.HasMany(d => d.Products)
                    .WithOne(p => p.Category);
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
