using Microsoft.EntityFrameworkCore;

namespace Slim.Data.Entity
{
    public partial class SlimDbContext : DbContext
    {
        public SlimDbContext()
        {
        }

        public SlimDbContext(DbContextOptions<SlimDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PageImage> PageImages { get; set; } = null!;
        public virtual DbSet<PageSection> PageSections { get; set; } = null!;
        public virtual DbSet<PageSectionImage> PageSectionImages { get; set; } = null!;
        public virtual DbSet<PageSectionResource> PageSectionResources { get; set; } = null!;
        public virtual DbSet<RazorPage> RazorPages { get; set; } = null!;
        public virtual DbSet<RazorPageResourceActionMap> RazorPageResourceActionMaps { get; set; } = null!;
        public virtual DbSet<ResourceAction> ResourceActions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SlimWebDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageImage>(entity =>
            {
                entity.ToTable("PageImage");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ActualImage).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PageImageName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PageSection>(entity =>
            {
                entity.ToTable("PageSection");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PageSectionName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.PageSections)
                    .HasForeignKey(d => d.RazorPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSection_PageId");
            });

            modelBuilder.Entity<PageSectionImage>(entity =>
            {
                entity.ToTable("PageSectionImage");

                entity.HasOne(d => d.PageImage)
                    .WithMany(p => p.PageSectionImages)
                    .HasForeignKey(d => d.PageImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionImage_PageImageId");

                entity.HasOne(d => d.RazorPage)
                    .WithMany(p => p.PageSectionImages)
                    .HasForeignKey(d => d.RazorPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionImage_PageId");

                entity.HasOne(d => d.RazorPageSection)
                    .WithMany(p => p.PageSectionImages)
                    .HasForeignKey(d => d.RazorPageSectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageSectionImage_SectionId");
            });

            modelBuilder.Entity<PageSectionResource>(entity =>
            {
                entity.ToTable("PageSectionResource");

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
                entity.ToTable("RazorPage");

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
                entity.ToTable("RazorPageResourceActionMap");

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
                entity.ToTable("ResourceAction");

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
