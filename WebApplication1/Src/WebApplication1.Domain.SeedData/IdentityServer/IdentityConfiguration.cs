﻿using System.Collections.Generic;
using IdentityServer4.Models;

namespace WebApplication1.Domain.SeedData.IdentityServer
{
    /// <summary>
    /// Indentity Configuration
    /// </summary>
    public class IdentityConfiguration
    {
        public const string ApiResourceCode = "$identityserver_resource_code$";
        public const string ApiResourceSecret = "$identityserver_resource_secret$";

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
                    ClientId = "swagger.client",
                    ClientName = "Swagger UI Client",
                    ClientSecrets = { new Secret("$identityserver_swagger_client_secret$".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ////AllowedGrantTypes = GrantTypes.Code,
                    ////RequirePkce = true,
                    ////RequireClientSecret = false,

                    ////RedirectUris = { "https://localhost:44319/swagger/oauth2-redirect.html" },
                    ////AllowedCorsOrigins = { "https://localhost:44319" },
                    AllowedScopes = { IdentityScopes.ApiRead, IdentityScopes.ApiWrite },
                },
                new Client
                {
                    ClientId = $"test.client",
                    ClientName = "Test Client Credentials Client",
                    ClientSecrets = { new Secret("$identityserver_test_client_secret$".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

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
