using Microsoft.AspNetCore.Mvc;
using MultiTenants.Web.Models;

namespace MultiTenants.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserName == "User" && model.Email == "user@test.com")
            {
                return RedirectToAction("Index", "User");
            }
            else if (model.UserName == "Admin" && model.Email == "admin@test.com")
            {
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Invalid UserName or Email");
            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            return RedirectToAction("Login");
        }
    }
}
