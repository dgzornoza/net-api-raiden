using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
/* $identityserver_feature$ start */
using Microsoft.OpenApi.Models;
/* $identityserver_feature$ end */
using WebApplication1.Api.Infrastructure.Extensions;
using WebApplication1.Api.Infrastructure.Filters;

namespace WebApplication1.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddApiVersioning()
                .AddCors()
                .AddSwagger(configuration)
                .AddAuthentication()
                .AddAuthorization();
        }

        private static IServiceCollection AddCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        private static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
                x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // version format "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";
                });

            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var title = configuration["App:Title"];
            var description = configuration["App:Description"];
            int versions = int.TryParse(configuration["App:Versions"], out versions) ? versions : 1;

            var assemblyProductAttribute = typeof(Startup).Assembly.GetCustomAttribute<AssemblyProductAttribute>() ??
                throw new ApplicationException(Properties.Resources.InvalidAssemblyProductAttribute);

            var assemblyDescriptionAttribute = typeof(Startup).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>() ??
                throw new ApplicationException(Properties.Resources.InvalidAssemblyDescriptionAttribute);

            return services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // generate swagger versions doc from controllers
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"{assemblyProductAttribute.Product} " + $"{description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ? $"{assemblyDescriptionAttribute.Description} - DEPRECATED" : assemblyDescriptionAttribute.Description,
                    });
                }

                // filter to set default version used for swagger doc
                options.OperationFilter<SwaggerApiVersionFilter>();

                // show full namespace in schemes section
                options.CustomSchemaIds(item => item.FullName);

                // select first endpoint in case of multiple
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, "xml"));

                /* $identityserver_feature$ start */

                // swagger oauth2 endpoint definition
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:44319/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:44319/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { IdentityScopes.ApiRead, nameof(IdentityScopes.ApiRead) },
                                { IdentityScopes.ApiWrite, nameof(IdentityScopes.ApiWrite) },
                            },
                        },
                    },
                });

                // include access token on AuthorizeAttribute endpoints
                options.OperationFilter<SwaggerAuthorizeOperationFilter>();
                /* $identityserver_feature$ end */
            });
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            // TODO: to implement
            return services;
        }

        private static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            ////services.AddAuthorization(options =>
            ////{
            ////    options.AddPolicy(AuthorizationPolicies.AdminUsers, policy => policy
            ////        .RequireAuthenticatedUser()
            ////        .RequireClaim(
            ////            ClaimTypes.Roles,
            ////            "admin"));
            ////});

            return services;
        }
    }
}
