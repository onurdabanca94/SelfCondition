using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SelfCondition.Entities;

namespace SelfCondition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<DatabaseContext>(opts => {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                //opts.UseLazyLoadingProxies();
            });

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Cookie.Name = ".SelfCondition.auth"; // Bu cookie ismi ile sakla bilgileri kullanýcýnýn tarayýsýnda login bilgilerini.
                    opts.ExpireTimeSpan = TimeSpan.FromDays(7);
                    //opts.SlidingExpiration = true; // Expire süresi ilerlesin.
                    opts.SlidingExpiration = false;
                    opts.LoginPath = "/Account/Login"; // Login deðilse buraya atar.
                    opts.LogoutPath = "/Account/Login";
                    opts.AccessDeniedPath = "/Home/AccessDenied"; // Rol kontrolü
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // sýralama önemli! öncelik authentication servisi aktive et kimlik doðrula sonra authorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}