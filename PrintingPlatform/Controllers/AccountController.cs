using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models.Account;

namespace PrintingPlatform.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            return View();
        }
    }
}