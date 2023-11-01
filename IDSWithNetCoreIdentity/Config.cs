using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IDSWithNetCoreIdentity
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
                    SubjectId = "fffb509f-d521-4074-ab3d-662ce4eac378",
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
                new ApiResource()
                new ApiResource("api1", "My API"),
                new ApiResource
                {
                    Name = "api2",

                    // secret for using introspection endpoint
                    //ApiSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    // include the following using claims in access token (in addition to subject id)
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email },

                    // this API defines two scopes
                    //Scopes =
                    //{
                    //    "api2.full_access",
                    //    "api2.read_only"
                    //}
                }
            };
            
        }
        public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("api1"),
            new ApiScope()
            {
                Name = "api2.full_access",
                DisplayName = "Full access to API 2"
            },
            new ApiScope
            {
                Name = "api2.read_only",
                DisplayName = "Read only access to API 2"
            }
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
                    AllowedScopes = { "openid", "profile", "api1", "api2.full_access" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                }
            };
        }


        public static void InitializeConfigurationDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if(!context.ApiScopes.Any())
                {
                    foreach (var scope in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }    
            }
        }
    }
}
