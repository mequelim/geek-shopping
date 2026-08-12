using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace GeekShopping.DuendeIdentityServer.Configuration.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        //? Roles:
        public const string Admin = "Admin";
        public const string Client = "Customer";

        public static IEnumerable<ApiResource> ApiResources =>
        [
            new ApiResource("geek_shopping", "GeekShopping API")
            {
                Scopes = { "geek_shopping" },
                UserClaims = { JwtClaimTypes.Role, "role" }
            }
        ];

        //? Identity Resources (claims):
        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new("role", "User role", [ "role" ])
        ];

        //? API Scopes (GeekShopping.Web project):
        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new(name: "geek_shopping", displayName: "GeekShopping Server"),
            new(name: "read", displayName: "Read data"),
            new(name: "write", displayName: "Write data"),
            new(name: "delete", displayName: "Delete data")
        ];

        //? Client (GeekShopping.Web project):
        public static IEnumerable<Client> GetClients(IConfiguration configuration) =>
        [
            //* Generic client:
            new()
            {
                ClientId = "client",
                ClientName = configuration["Secrets:ClientName"],
                RequireClientSecret = true,  //* Default value: true.
                ClientSecrets = { new Secret(configuration["Secrets:ClientSecrets"].Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes =
                {
                    "read",
                    "write",
                    "delete",
                    "geek_shopping"
                }
            },
            //* Advanced client:
            new()
            {
                ClientId = "geek_shopping",
                ClientName = configuration["Secrets:ClientName"],
                ClientSecrets = { new Secret(configuration["Secrets:ClientSecrets"].Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:7102/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:7102/signout-callback-oidc" },
                AllowedScopes = [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "geek_shopping",
                    "role"
                ]
            }
        ];
    }
}