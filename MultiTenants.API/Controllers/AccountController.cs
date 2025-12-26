using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenants.API.Models;

namespace MultiTenants.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

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
        public async Task<IActionResult> SignUp([FromBody] SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            return result.Succeeded ? Ok(new { Success = true, Message = "SignUp successful" }) : BadRequest(result.Errors);
        }
    }
}