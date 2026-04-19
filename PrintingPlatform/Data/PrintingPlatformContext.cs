using Microsoft.EntityFrameworkCore;
using PrintingPlatform.Data.Entities;

namespace PrintingPlatform.Data;

public class PrintingPlatformContext : DbContext
{
    public PrintingPlatformContext(DbContextOptions<PrintingPlatformContext> options)
    : base(options)
{
}

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(orderEntity => orderEntity.Items)
                .WithOne(orderItemEntity => orderItemEntity.Order)
                .HasForeignKey(orderItemEntity => orderItemEntity.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
}
