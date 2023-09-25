using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using $safeprojectname$.Infrastructure.Authorization;
using Polly.Registry;
using Polly;
// $identityserver_feature$ using System.Text;
// $identityserver_feature$ using System.Collections.Generic;
// $identityserver_feature$ using Microsoft.IdentityModel.Tokens;
// $identityserver_feature$ using Microsoft.EntityFrameworkCore;
// $identityserver_feature$ using Microsoft.AspNetCore.Authentication.JwtBearer;
// $identityserver_feature$ using $ext_safeprojectname$.Domain.SeedData.IdentityServer;
// $identityserver_feature$ using $safeprojectname$.Settings;
// $identityserver_feature$ using $ext_safeprojectname$.Infrastructure.Domain;
using $safeprojectname$.Infrastructure.Extensions;
using $safeprojectname$.Infrastructure.Filters;

namespace $safeprojectname$.Infrastructure.Extensions
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
            var assemblyProductAttribute = typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>() ??
                throw new ApplicationException(Properties.Resources.InvalidAssemblyProductAttribute);

            var assemblyDescriptionAttribute = typeof(Program).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>() ??
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

                options.IncludeXmlComments(Path.ChangeExtension(typeof(Program).Assembly.Location, "xml"));

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
                    $ext_safeprojectname$.Infrastructure.Constants.Polly.WaitAndRetry,
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
            // add the config data from DB (clients, resources)
            .AddConfigurationStore(options =>
            {
                options.DefaultSchema = "dbo";
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString, sqlOptions => {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            // add the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.DefaultSchema = "dbo";
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString, sqlOptions => {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });

            // enables automatic token cleanup
            options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            return services;
        }
        /* $identityserver_feature$ end */
    }
}
