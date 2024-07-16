using Jaryway.IdentityServer.Models;

namespace Server
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] {
                new ApiScope("CoffeeAPI.read"),
                new ApiScope("CoffeeAPI.write"),
                //new ApiScope("api1", "Full access to API #1")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("CoffeeAPI")
                {
                    Scopes = new List<string> { "CoffeeAPI.read", "CoffeeAPI.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                },
                //new ApiResource("api1", "API #1") { Scopes = { "api1" } }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "CoffeeAPI.read", "CoffeeAPI.write" }
                },
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        "https://localhost:4100/signin-oidc",
                        "http://localhost:4100/signin-oidc",
                        "http://localhost:4100/private"
                    },
                    FrontChannelLogoutUri = "http://localhost:4100/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:4100/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "CoffeeAPI.read" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false,
                    //AccessTokenLifetime = 60
                },
                new Client
                {
                    ClientId = "api_swagger",
                    ClientName = "Swagger UI for api",
                    ClientSecrets = { new Secret("secret".Sha256()) }, // change me!
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = {
                        "https://localhost:5445/swagger/oauth2-redirect.html",
                        "http://localhost:5445/swagger/oauth2-redirect.html"
                    },
                    AllowedCorsOrigins = { "https://localhost:5445", "https://localhost:5445"},
                    AllowedScopes = { "CoffeeAPI.read", "CoffeeAPI.write" }
                }

            };
    }
}
