using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };

            return new List<TestUser>
                {
                  new TestUser
                  {
                    SubjectId = "818727",
                    Username = "alice",
                    Password = "alice",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Alice Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Alice"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "admin"),
                      new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                    }
                  },
                  new TestUser
                  {
                    SubjectId = "88421113",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Bob Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Bob"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "user"),
                      new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                    }
                  }
                };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }
        public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("api1"),
  
        };

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "console-client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("console-client-secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "mvc-client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.Code,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("mvc-client-secret".Sha256())
                    },
                    RedirectUris = {"https://localhost:5553/signin-oidc" },
                    // scopes that client has access to
                    AllowedScopes = { "openid", "profile", "api1" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                }
            };
        }
    }
}
