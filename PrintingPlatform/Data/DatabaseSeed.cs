using System;
using PrintingPlatform.Data.Entities;
using Microsoft.AspNetCore.Identity;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Data;

    public static class DatabaseSeed
    {
        public static void Seed(PrintingPlatformContext context)
        {
            CreateAdmin(context);
        }
        private static void CreateAdmin (PrintingPlatformContext context)
        {
            var admin = context.Users.FirstOrDefault(u => u.Email == "gornairyna@gmail.com");

            if (admin == null)
            {
                context.Users.Add(
                new User
                {
                    FirstName = "Iryna",
                    LastName = "Gorna",
                    Email = "gornairyna@gmail.com",
                    Password = "12345678",
                    Roles = [
                        new UserRole 
                        { Name = AppRoles.Admin
                    }]
                });
                context.SaveChanges();
            }
        }
    }
    

