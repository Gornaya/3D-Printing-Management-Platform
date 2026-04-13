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
}