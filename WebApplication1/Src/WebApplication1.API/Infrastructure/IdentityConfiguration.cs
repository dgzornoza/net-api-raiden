using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace WebApplication1.Api.Infrastructure
{
    /// <summary>
    /// https://codewithmukesh.com/blog/identityserver4-in-aspnet-core/
    /// http://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html
    /// </summary>
    public class IdentityConfiguration
    {
        public const string ApiResourceCode = "$identityserver_resource_code$";
        public const string ApiResourceSecret = "$identityserver_resource_secret$";

        public static List<TestUser> TestUsers =>
            new ()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "dgzornoza",
                    Password = "123456789",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "David González Zornoza"),
                        new Claim(JwtClaimTypes.GivenName, "David"),
                        new Claim(JwtClaimTypes.FamilyName, "González Zornoza"),
                        new Claim(JwtClaimTypes.NickName, "dgzornoza"),
                        new Claim(JwtClaimTypes.WebSite, "https://github.com/dgzornoza"),
                    },
                },
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(IdentityScopes.ApiRead),
                new ApiScope(IdentityScopes.ApiWrite),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource(ApiResourceCode)
                {
                    Scopes = new List<string> { IdentityScopes.ApiRead, IdentityScopes.ApiWrite },
                    ApiSecrets = new List<Secret> { new Secret(ApiResourceSecret.Sha256()) },
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = $"test.client",
                    ClientName = "Test Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("$identityserver_client_secret$".Sha256()) },
                    AllowedScopes = { IdentityScopes.ApiRead },
                },
            };
    }

    public class IdentityScopes
    {
        public static readonly string ApiRead = $"{IdentityConfiguration.ApiResourceCode}.read";
        public static readonly string ApiWrite = $"{IdentityConfiguration.ApiResourceCode}.write";
    }
}
