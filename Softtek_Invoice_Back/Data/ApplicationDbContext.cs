using Microsoft.EntityFrameworkCore;
using Softtek_Invoice_Back.Domain.Entities;

namespace Softtek_Invoice_Back.Data;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BranchId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ItemId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Add indexes for better query performance
            entity.HasIndex(e => e.BranchId).HasDatabaseName("IX_Orders_BranchId");
            entity.HasIndex(e => e.ItemId).HasDatabaseName("IX_Orders_ItemId");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Orders_Status");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Orders_CreatedAt");
        });
    }
}

