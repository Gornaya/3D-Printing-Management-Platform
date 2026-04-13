using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PrintingPlatform.Data.Entities;
using PrintingPlatform.Data;


namespace PrintingPlatform;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

        builder.Services.AddDistributedMemoryCache();
        
        builder.Services.AddSession(options=>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
             
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<PrintingPlatformContext>
        (options => options.UseLazyLoadingProxies()
            .UseSqlite(builder.Configuration.GetConnectionString
            ("DefaultConnection")));

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
             var context = scope.ServiceProvider.
             GetRequiredService<PrintingPlatformContext>();
             DatabaseSeed.Seed(context);
        }
        
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Home/Error404");

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
