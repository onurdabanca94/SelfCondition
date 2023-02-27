using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using SelfCondition.Entities;
using SelfCondition.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SelfCondition.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        [AllowAnonymous] // - Bu metoda giriş izni herkese açık olmalıdır.
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = DoMD5HashedString(model.Password);

                User user = _databaseContext.Users.SingleOrDefault(x => x.Username.ToLower() == model.Username.ToLower() && x.Password == hashedPassword);
                if (user != null)
                {
                    if (user.Locked)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Username is locked.");
                        return View(model);
                    }

                    List<Claim> claims = new List<Claim>();
                    //claims.Add(new Claim("Id", user.Id.ToString())); -- String bir ifade yerine aşağıdaki hazır gelen claimTypes içerisinden bilgi alınabilir yaptık.
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.Fullname ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    // Bire çok tablo tutarsak bir user'ın birden çok rolü varsa (admin + müşteri + müdür gibi.) bütün hepsini çektikten sonra foreach dönüp hepsini aynı claim ismi ile ekleyebiliriz user'a.

                    claims.Add(new Claim("Username", user.Username));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); //CookieAuth kullanıyoruz.
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); //Yukarıdaki prensiplere göre login işlemini gerçekle.

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is incorrect.");
                }
            }
            return View(model);
        }

        private string DoMD5HashedString(string s)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string salted = s + md5Salt;
            string hashed = salted.MD5();
            return hashed;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.Username == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Username is already exists.");
                    View(model);
                }

                //string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
                //string saltedPassword = model.Password + md5Salt;
                //string hashedPassword = saltedPassword.MD5(); // MD5 -> installed NuGet package extension. - hashed password included our prop.

                string hashedPassword = DoMD5HashedString(model.Password);

                User user = new()
                {
                    Username = model.Username,
                    Password = hashedPassword
                };

                _databaseContext.Users.Add(user);
                int affectedRowsCount = _databaseContext.SaveChanges(); //SaveChanges returns int -> it tells how many insert data in db.
                if (affectedRowsCount == 0)
                {
                    ModelState.AddModelError("", "User can not be added.");
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }

            return View(model);
        }
        public IActionResult Profile()
        {
            ProfileInfoLoader();

            return View();
        }

        private void ProfileInfoLoader()
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

            ViewData["Fullname"] = user.Fullname;
        }

        [HttpPost]
        public IActionResult ProfileChangeFullName([Required][StringLength(50)] string? fullname) //model kullanmadan input'a name="fullname" verdiğimizde ilgili dataya erişim sağlayabiliriz.
        {
            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)); //FindFirst dersek cümle sonuna .Value eklememiz gerekliydi bu metod ile gerek kalmadı.
                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

                user.Fullname = fullname;
                _databaseContext.SaveChanges();

                return RedirectToAction(nameof(Profile));
            }

            ProfileInfoLoader();
            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangePassword([Required][MinLength(6)][MaxLength(16)] string? password) //model kullanmadan input'a name="fullname" verdiğimizde ilgili dataya erişim sağlayabiliriz.
        {
            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)); //FindFirst dersek cümle sonuna .Value eklememiz gerekliydi bu metod ile gerek kalmadı.
                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

                string hashedPassword = DoMD5HashedString(password);

                user.Password = hashedPassword;
                _databaseContext.SaveChanges();

                //return RedirectToAction(nameof(Profile));
                ViewData["result"] = "PasswordChanged";
            }

            ProfileInfoLoader();
            return View("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
