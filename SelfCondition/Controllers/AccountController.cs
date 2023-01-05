using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using SelfCondition.Entities;
using SelfCondition.Models;

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
                // login işlemleri
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
