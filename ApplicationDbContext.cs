using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);

        modelBuilder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(120).IsRequired();

        modelBuilder.Entity<Product>()
            .Property(p => p.Code).HasMaxLength(20).IsRequired();

        modelBuilder.Entity<Product>()
            .Property(p => p.Active).HasDefaultValue(true);

        modelBuilder.Entity<Category>()
            .ToTable("Categories");
    }

}