using System;
using PrintingPlatform.Data.Entities;
using Microsoft.AspNetCore.Identity;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Data;

    public static class DatabaseSeed
    {
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
        public static void Seed(PrintingPlatformContext context)
        {
            SeedRoles(context);
            CreateAdmin(context);
        }
        private static void CreateAdmin (PrintingPlatformContext context)
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
    }

    

