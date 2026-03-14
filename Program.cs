using Microsoft.EntityFrameworkCore;
using StudentSpot.Models;

namespace StudentSpot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===============================
            // MVC + API + JSON Cycle Fix
            // ===============================
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // ===============================
            // Database Connection
            // ===============================
            builder.Services.AddDbContext<myData>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("Students")));

            // ===============================
            // Session (For MVC Login)
            // ===============================
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ===============================
            // HttpClient (Optional)
            // ===============================
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // ===============================
            // Development Settings
            // ===============================
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Session MUST be before MVC
            app.UseSession();

            // ===============================
            // MVC Default Route
            // ===============================
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customer}/{action=Index}/{id?}");

            // ===============================
            // API Controllers
            // ===============================
            app.MapControllers();

            app.Run();
        }
    }
}