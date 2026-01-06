using DAL.Interfaces;
using DotNetCoreWithIdentityServer.Models;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWithIdentityServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountService;

        public AccountController(IAccountServices accountServices)
        {
            _accountService = accountServices;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TokenResponseModel? token = await _accountService.IsUserExists(new SignInDTO() { Email = model.Email, Password = model.Password });

            if (token is not null)
            {
                return Ok(new
                {
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresIn,
                    token_type = token.TokenType,
                    refresh_token = token.RefreshToken
                });
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

            SignupDTO signupDTO = new() { UserName = model.UserName, Email = model.Email, Password = model.Password, PhoneNumber = model.PhoneNumber };
            signupDTO = await _accountService.SignupUserAsync(signupDTO);

            return signupDTO != null ? Ok(new { Success = true, Message = "SignUp successful." }) : BadRequest("Please try agian. User not successfully signup");
        }

        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromForm] string refreshToken)
        {
            var client = new HttpClient();

            var tokenResponse = await _accountService.RefreshTokenAsync(refreshToken);

            if (tokenResponse is null)
            {
                return BadRequest("Token not generated.");
            }

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                refresh_token = tokenResponse.RefreshToken,
                expires_in = tokenResponse.ExpiresIn
            });
        }
    }
}