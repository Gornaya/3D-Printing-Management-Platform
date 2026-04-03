using Microsoft.EntityFrameworkCore;

namespace PrintingPlatform.Data.Entities;

public class PrintingPlatformContext : DbContext
{
    public PrintingPlatformContext(DbContextOptions<PrintingPlatformContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
}