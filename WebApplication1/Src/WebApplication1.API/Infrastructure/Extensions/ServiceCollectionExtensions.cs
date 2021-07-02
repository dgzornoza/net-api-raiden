using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.API.Infrastructure.Extensions;
using WebApplication1.API.Infrastructure.Filters;

namespace WebApplication1.API.Infrastructure.Extensions
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
                    // formato de la version sera "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";
                });

            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            string title = configuration["App:Title"];
            string description = configuration["App:Description"];
            int versions = int.TryParse(configuration["App:Versions"], out versions) ? versions : 1;

            AssemblyProductAttribute assemblyProductAttribute = typeof(Startup).Assembly.GetCustomAttribute<AssemblyProductAttribute>() ??
                throw new ApplicationException(Properties.Resources.InvalidAssemblyProductAttribute);

            AssemblyDescriptionAttribute assemblyDescriptionAttribute = typeof(Startup).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>() ??
                throw new ApplicationException(Properties.Resources.InvalidAssemblyProductAttribute);

            return services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // generar documentacion de versiones de swagger desde los controladores
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"{assemblyProductAttribute.Product} " + $"{description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ? $"{assemblyDescriptionAttribute.Description} - DEPRECATED" : assemblyDescriptionAttribute.Description,
                    });
                }

                // filtro para establecer la version por defecto usada por el documento swagger
                options.OperationFilter<SwaggerApiVersionFilter>();

                // mostrar namespace completo en la seccion schemes
                options.CustomSchemaIds(item => item.FullName);

                // seleccionar el primer endpoint en caso de exisir multiples
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, "xml"));
            });
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            // TODO: falta implementar
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
