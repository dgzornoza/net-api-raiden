using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
/* $identityserver_feature$ start */
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Registry;
using WebApplication1.Api.Infrastructure.Authorization;
/* $identityserver_feature$ end */
using WebApplication1.Api.Infrastructure.Extensions;
using WebApplication1.Api.Infrastructure.Filters;
using WebApplication1.Api.Settings;
using WebApplication1.Infrastructure.Domain;

namespace WebApplication1.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddApiVersioning()
                .AddCors()
                .AddSwagger()
                .AddPollyPolicies()
                /* $identityserver_feature$ start */
                .AddIdentityServer(configuration)
                /* $identityserver_feature$ end */
                .AddAuthentication(configuration)
                .AddAuthorization(Policies.AddPolicies);
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

        private static IServiceCollection AddSwagger(this IServiceCollection services)
        {
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
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo()
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
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            // AuthorizationUrl = new Uri("https://localhost:44319/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:44319/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { IdentityScopes.ApiRead, nameof(IdentityScopes.ApiRead) },
                                { IdentityScopes.ApiWrite, nameof(IdentityScopes.ApiWrite) },
                            },
                        },
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

        private static IServiceCollection AddPollyPolicies(this IServiceCollection services)
        {
            PolicyRegistry registry = new PolicyRegistry
            {
                {
                    WebApplication1.Infrastructure.Constants.Polly.WaitAndRetry,
                    Policy.Handle<HttpRequestException>().WaitAndRetry(new[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        })
                },
            };

            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            /* $identityserver_feature$ start */
            var jwtSettings = configuration.GetObject<JwtSettings>(AppSettingsKeys.Jwt);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.Authority = jwtSettings.Authority;
                options.Audience = jwtSettings.ValidAudience;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.ValidAudience,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };
            });
            /* $identityserver_feature$ end */

            return services;
        }

        /* $identityserver_feature$ start */
        private static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(AppSettingsKeys.AppConnectionString);

            // configure identity server
            var migrationsAssembly = typeof(AppUnitOfWork).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer(options =>
            {
                ////options.UserInteraction = new UserInteractionOptions()
                ////{
                ////    LogoutUrl = "set logout url",
                ////    LoginUrl = "set login url",
                ////    LoginReturnUrlParameter = "returnUrl",
                ////};
            })
            .AddDeveloperSigningCredential()
            .AddTestUsers(IdentityConfiguration.TestUsers)
            // add the config data from DB (clients, resources)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            // add the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                // enables automatic token cleanup
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            return services;
        }

        /* $identityserver_feature$ end */
    }
}
