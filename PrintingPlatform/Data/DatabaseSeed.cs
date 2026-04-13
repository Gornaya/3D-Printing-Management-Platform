using System;
using PrintingPlatform.Data.Entities;
using Microsoft.AspNetCore.Identity;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Data;

public static class DatabaseSeed
{
    public static void Seed(PrintingPlatformContext context)
    {
        SeedRoles(context);
        CreateAdmin(context);
        SeedProducts(context);
    }
    private static void SeedRoles(PrintingPlatformContext context)
    {
        if (!context.Roles.Any(role => role.Name == AppRoles.Admin))
            context.Roles.Add(new Role { Name = AppRoles.Admin });

        if (!context.Roles.Any(role => role.Name == AppRoles.Manager))
            context.Roles.Add(new Role { Name = AppRoles.Manager });

        if (!context.Roles.Any(role => role.Name == AppRoles.User))
            context.Roles.Add(new Role { Name = AppRoles.User });

        context.SaveChanges();
    }
    private static void CreateAdmin(PrintingPlatformContext context)
    {
        var admin = context.Users.FirstOrDefault(user => user.Email == "gornairyna@gmail.com");

        if (admin == null)
        {
            var adminUser = new User
            {
                FirstName = "Iryna",
                LastName = "Gorna",
                Email = "gornairyna@gmail.com",
                Password = "123456",
                Roles = new List<Role>
                        { context.Roles.First(role => role.Name == AppRoles.Admin)
                    }
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.Password = passwordHasher.HashPassword(adminUser, adminUser.Password);

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }

    private static void SeedProducts(PrintingPlatformContext context)
    {
        if (context.Products.Any())
        {
            return;
        }

        var products = new List<Product>
            {
                new Product
                {
                    Name = "Geometric Desk Organizer",
                    Description = "Minimal desktop organizer printed in PLA for office and home setups",
                    Price = 22.99m,
                    ImageUrl = "/assets/images/products/s1.jpg"
                },
                new Product
            {
                Name = "Modular Cable Holder",
                Description = "Snap-fit cable holder that keeps charging and USB cables in place.",
                Price = 14.50m,
                ImageUrl = "/assets/images/products/s2.jpg"
            },
            new Product
            {
                Name = "Custom Nameplate",
                Description = "Personalized 3D printed nameplate for desks, doors, or gifts.",
                Price = 19.00m,
                ImageUrl = "/assets/images/products/s3.jpg"
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}




