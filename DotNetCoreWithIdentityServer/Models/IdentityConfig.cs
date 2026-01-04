using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace DotNetCoreWithIdentityServer.Models
{
    public static class IdentityConfig
    {
        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("myapi", "My Protected API")
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "web-client",
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                   
                    // AllowedGrantTypes = GrantTypes.Code,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // allows username+password login
                    AccessTokenLifetime = 3600, // 1 hour
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 1296000, // 15 days
                    AllowOfflineAccess = true,

                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    AllowedScopes = { "openid", "profile", "myapi" }
                }
            };
    }
}