using Microsoft.AspNetCore.Mvc;
using MultiTenants.API.Models;

namespace MultiTenants.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.UserName == "Admin" && model.Email == "admin@test.com" || model.UserName == "User" && model.Email == "user@test.com")
            {
                return Ok(new { Success = true, Message = "Login successful" });

            }
            return Ok(new { Success = false, Message = "Invalid Username or email" });
        }
        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new { Success = true, Message = "SignUp successful" });
        }
    }
}
