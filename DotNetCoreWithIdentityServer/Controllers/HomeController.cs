using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWithIdentityServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index() => Ok(new { Success = true, Message = "Welcome to the protected Home Index!" });
    }
}
