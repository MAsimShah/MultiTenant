using DAL.Interfaces;
using DTO;
using Duende.IdentityModel.Client;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DAL.Repositories
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IdentityServerOptions _options;

        public AccountServices(
            UserManager<ApplicationUser> userManager,
            HttpClient httpClient,
            IOptions<IdentityServerOptions> options) // <-- use IOptions<T>
        {
            _userManager = userManager;
            _httpClient = httpClient;
            _options = options.Value; // get the actual bound object
        }

        public async Task<SignupDTO> SignupUserAsync(SignupDTO model)
        {
            ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email, PasswordHash = model.Password, PhoneNumber = model.PhoneNumber };

            IdentityResult result = await _userManager.CreateAsync(user, password: user.PasswordHash);

            if (!result.Succeeded)
                throw new Exception("User not successfully created.");

            return model;
        }

        public async Task<TokenResponseModel> IsUserExists(SignInDTO signInDTO)
        {

            if (string.IsNullOrEmpty(signInDTO.Email) || string.IsNullOrEmpty(signInDTO.Password))
            {
                throw new Exception("Email or Password is empty.");
            }

            var user = await _userManager.FindByEmailAsync(signInDTO.Email);

            if (user is null)
            {
                throw new Exception("User not found.");
            }

            bool isAuthenticatedUser = await _userManager.CheckPasswordAsync(user, signInDTO.Password);

            if (!isAuthenticatedUser)
            {
                throw new Exception("Invalid credentials.");
            }

            return await GetTokenAsync(signInDTO.Email, signInDTO.Password);
        }

        private async Task<TokenResponseModel> GetTokenAsync(string username, string password)
        {
            var parameters = new Dictionary<string, string>
                                     {
                                         { "grant_type", "password" },
                                         { "client_id", _options.ClientId },
                                         { "client_secret", _options.ClientSecret },
                                         { "username", username },
                                         { "password", password },
                                         { "scope", _options.Scope }
                                     };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _options.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            HttpResponseMessage? response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string? content = await response.Content.ReadAsStringAsync();
            TokenResponseModel? tokenResponse = JsonSerializer.Deserialize<TokenResponseModel>(content);

            if (tokenResponse is null)
            {
                throw new Exception("Token deserialization failed.");
            }

            return tokenResponse;
        }


        private async Task SignIn(SignupDTO model)
        {
            // Validate user
            var user = await _userManager.FindByEmailAsync(model.Email);
            //if (user == null)
            //    return BadRequest(new { Message = "Invalid email or password" });

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            //if (!passwordValid)
            //    return BadRequest(new { Message = "Invalid email or password" });

            // Create a token request (Resource Owner Password flow)
            //var tokenRequest = new Duende.IdentityServer.Models.TokenRequest
            //{
            //    GrantType = "password",
            //    ClientId = "web-client",
            //    ClientSecret = "super-secret",
            //    Parameters =
            //{
            //    { "username", model.Email },
            //    { "password", model.Password },
            //    { "scope", "openid profile myapi offline_access" } // offline_access = refresh token
            //}
            //};

            // Normally, clients call /connect/token directly.
            // For internal call, we can simulate with HttpClient or call IdentityServer's Token endpoint
            // Here, simplest approach: direct client calls /connect/token externally.

            //return Ok(new
            //{
            //    Message = "Login successful",
            //    TokenEndpoint = "https://localhost:5001/connect/token",
            //    Info = "Client should call this endpoint with Resource Owner Password flow to get access & refresh token."
            //});
        }
    }
}
