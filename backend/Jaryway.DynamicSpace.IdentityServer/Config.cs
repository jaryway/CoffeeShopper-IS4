using Jaryway.IdentityServer;
using Jaryway.IdentityServer.Models;

namespace Jaryway.DynamicSpace.IdentityServer
{
    public class Config
    {
        static string[] allowedScopes =
       {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            IdentityServerConstants.StandardScopes.Email,
            "CoffeeAPI.read",
            "CoffeeAPI.write",
            "DynamicWebApi.all"
        };
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
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
                new ApiScope("DynamicWebApi.all")
                //new ApiScope("api1", "Full access to API #1")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("CoffeeAPI")
                {
                    Scopes = new List<string> { "CoffeeAPI.read", "CoffeeAPI.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" },
                },
                 new ApiResource("DynamicWebApi")
                {
                    Scopes = new List<string> { "DynamicWebApi.all" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                },
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
                    ClientSecrets = { new Secret("Secret".Sha256()) },
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        //"https://localhost:4100/",
                        "http://localhost:4100/",
                    },
                    // 用于在用户注销时通知后端应用程序(如果应用是一个后端应用程序，则用这个)
                    //BackChannelLogoutUri = "",
                    // 用于在用户注销时通知前端应用程序(如果应用是一个前端应用程序，则用这个)
                    FrontChannelLogoutUri = "http://localhost:4100/",
                    // 用于指定注销后用户应该被重定向到的位置，指用户主动注销后，希望回跳的地址
                    PostLogoutRedirectUris = { "http://localhost:4100/signout-success" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "CoffeeAPI.read", "CoffeeAPI.write", "DynamicWebApi.all" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false,                   
                    
                    // 设置 access_token 的过期时间
                     AccessTokenLifetime = 120
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
                        "http://localhost:5002/swagger/oauth2-redirect.html",
                        "https://localhost:5446/swagger/oauth2-redirect.html",
                        "http://localhost:5003/swagger/oauth2-redirect.html"
                    },
                    AllowedCorsOrigins = {
                        "https://localhost:5445",
                        "http://localhost:5002",
                        "https://localhost:5446",
                        "http://localhost:5003"
                    },
                    AllowedScopes = { "CoffeeAPI.read", "CoffeeAPI.write", "DynamicWebApi.all" }
                },

                new Client
                {
                    ClientId = "js_oidc",
                    ClientName = "JavaScript OIDC Client",
                    ClientUri = "http://localhost:4200",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    //RefreshTokenExpiration = TokenExpiration.Sliding,

                    RedirectUris =
                    {
                        "http://localhost:4200/index.html",
                        "http://localhost:4200/callback.html",
                        "http://localhost:4200/silent.html",
                        "http://localhost:4200/popup.html"
                    },

                    PostLogoutRedirectUris = { "http://localhost:4200/index.html" },
                    FrontChannelLogoutUri = "http://localhost:4200/callback.html",
                    AllowedCorsOrigins = { "https://localhost:5046", "https://localhost:5047" },

                    AllowedScopes = allowedScopes
                },
            };
    }
}
