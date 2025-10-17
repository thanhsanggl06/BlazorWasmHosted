using BlazorWasmHosted.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmHosted.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TodoItem configuration
        var todo = modelBuilder.Entity<TodoItem>();
        todo.ToTable("TodoItems");
        todo.HasKey(x => x.Id);
        todo.Property(x => x.Title).IsRequired().HasMaxLength(200);
        todo.Property(x => x.IsDone).HasDefaultValue(false);
        todo.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Supplier configuration
        var supplier = modelBuilder.Entity<Supplier>();
        supplier.ToTable("Suppliers");
        supplier.HasKey(x => x.Id);
        supplier.Property(x => x.SupplierCode).IsRequired().HasMaxLength(50);
        supplier.Property(x => x.SupplierName).IsRequired().HasMaxLength(200);
        supplier.Property(x => x.ContactPerson).HasMaxLength(100);
        supplier.Property(x => x.Email).HasMaxLength(100);
        supplier.Property(x => x.Phone).HasMaxLength(50);
        supplier.Property(x => x.Address).HasMaxLength(500);
        supplier.Property(x => x.City).HasMaxLength(100);
        supplier.Property(x => x.Country).HasMaxLength(100);
        supplier.Property(x => x.IsActive).HasDefaultValue(true);
        supplier.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        supplier.HasIndex(x => x.SupplierCode).IsUnique();

        // Product configuration
        var product = modelBuilder.Entity<Product>();
        product.ToTable("Products");
        product.HasKey(x => x.Id);
        product.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
        product.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
        product.Property(x => x.Category).IsRequired().HasMaxLength(100);
        product.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        product.Property(x => x.Quantity).HasDefaultValue(0);
        product.Property(x => x.InStock).HasDefaultValue(true);
        product.Property(x => x.Description).HasMaxLength(1000);
        product.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        product.HasIndex(x => x.ProductCode).IsUnique();
        
        // Configure relationship: Product -> Supplier (Many-to-One)
        product.HasOne(p => p.Supplier)
               .WithMany(s => s.Products)
               .HasForeignKey(p => p.SupplierId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
    }
}