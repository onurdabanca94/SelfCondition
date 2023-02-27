using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfCondition.Models;
using System.Diagnostics;

namespace SelfCondition.Controllers
{
    [Authorize] //- Controller seviyesinde eklediğimizde tümünü etkiler.
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous] // - Bazı metodlarda giriş serbest bırakmak için kullanırız. (Yukarıdaki auth metodunu controller seviyesinde tanımladıktan sonra.)
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() 
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}