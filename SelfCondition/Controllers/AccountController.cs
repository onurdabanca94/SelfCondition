using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using SelfCondition.Entities;
using SelfCondition.Models;
using System.Security.Claims;

namespace SelfCondition.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
                string saltedPassword = model.Password + md5Salt;
                string hashedPassword = saltedPassword.MD5();

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
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
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
                string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
                string saltedPassword = model.Password + md5Salt;
                string hashedPassword = saltedPassword.MD5(); // MD5 -> installed NuGet package extension. - hashed password included our prop.

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
            return View();
        }
    }
}
