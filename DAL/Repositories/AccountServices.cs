using DAL.Interfaces;
using DTO;
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

            return await GenerateNewTokenAsync(signInDTO.Email, signInDTO.Password);
        }

        public async Task<TokenResponseModel> RefreshTokenAsync(string refreshToken)
        {
            var parameters = new Dictionary<string, string>
                                     {
                                         { "grant_type", "refresh_token" },
                                         { "client_id", _options.ClientId },
                                         { "client_secret", _options.ClientSecret },
                                         { "refresh_token", refreshToken }
                                     };
            return await GenerateToken(parameters);
        }

        private async Task<TokenResponseModel> GenerateNewTokenAsync(string username, string password)
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

            return await GenerateToken(parameters);
        }

        private async Task<TokenResponseModel> GenerateToken(Dictionary<string, string> parameters)
        {
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
    }
}