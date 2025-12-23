using Microsoft.AspNetCore.Mvc;

namespace MultiTenants.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
