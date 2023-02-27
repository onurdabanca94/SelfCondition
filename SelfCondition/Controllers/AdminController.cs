using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SelfCondition.Controllers
{
    //[Authorize(Roles ="admin, manager")] - Controller seviyesinde auth eklersek tüm actionlara giriş için kontrol eder. Ve bu roller bu action'ı çalıştırabilir diyoruz.
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
