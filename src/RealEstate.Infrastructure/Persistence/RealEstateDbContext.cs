using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Persistence.EF.Models;

namespace RealEstate.Infrastructure.Persistence;

public partial class RealEstateDbContext : DbContext
{
    public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyImage> PropertyImages { get; set; }

    public virtual DbSet<PropertyTrace> PropertyTraces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.IdOwner).HasName("PK__Owner__D3261816BC17A923");

            entity.ToTable("Owner");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Photo).HasMaxLength(300);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.IdProperty).HasName("PK__Property__842B6AA72B354426");

            entity.ToTable("Property");

            entity.HasIndex(e => e.CodeInternal, "UQ__Property__F25D9D35396F62F6").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CodeInternal).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdOwnerNavigation).WithMany(p => p.Properties)
                .HasForeignKey(d => d.IdOwner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Property_Owner");
        });

        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(e => e.IdPropertyImage).HasName("PK__Property__018BACD51741A9DD");

            entity.ToTable("PropertyImage");

            entity.Property(e => e.Caption).HasMaxLength(200);
            entity.Property(e => e.Enabled).HasDefaultValue(true);
            entity.Property(e => e.FileUrl).HasMaxLength(300);

            entity.HasOne(d => d.IdPropertyNavigation).WithMany(p => p.PropertyImages)
                .HasForeignKey(d => d.IdProperty)
                .HasConstraintName("FK_PropertyImage_Property");
        });

        modelBuilder.Entity<PropertyTrace>(entity =>
        {
            entity.HasKey(e => e.IdPropertyTrace).HasName("PK__Property__373407C962E34EB8");

            entity.ToTable("PropertyTrace");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdPropertyNavigation).WithMany(p => p.PropertyTraces)
                .HasForeignKey(d => d.IdProperty)
                .HasConstraintName("FK_PropertyTrace_Property");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
