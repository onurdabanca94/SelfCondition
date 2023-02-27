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
                    opts.Cookie.Name = ".SelfCondition.auth"; // Bu cookie ismi ile sakla bilgileri kullan�c�n�n taray�s�nda login bilgilerini.
                    opts.ExpireTimeSpan = TimeSpan.FromDays(7);
                    //opts.SlidingExpiration = true; // Expire s�resi ilerlesin.
                    opts.SlidingExpiration = false;
                    opts.LoginPath = "/Account/Login"; // Login de�ilse buraya atar.
                    opts.LogoutPath = "/Account/Login";
                    opts.AccessDeniedPath = "/Home/AccessDenied"; // Rol kontrol�
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // s�ralama �nemli! �ncelik authentication servisi aktive et kimlik do�rula sonra authorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}