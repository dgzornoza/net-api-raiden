using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Reflection;
using WebApplication1.Infrastructure.Filters.Swagger;

namespace WebApplication1.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions class for manage third party services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Extension method for add third party services to container
        /// </summary>
        /// <param name="services">Services container collection</param>
        /// <param name="configuration">App configuration</param>
        /// <returns>Services container collection</returns>
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddCors()
                .AddSwagger(configuration)
                .AddAutomapper();
        }

        /// <summary>
        /// Extension method for add CORS services
        /// </summary>
        /// <param name="services">Services container collection</param>
        /// <returns>Services container collection</returns>
        private static IServiceCollection AddCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        /// <summary>
        /// Extension method for add Swagger services
        /// </summary>
        /// <param name="services">Services container collection</param>
        /// <param name="configuration">App configuration</param>
        /// <returns>Services container collection</returns>
        private static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSwaggerGen(options =>
            {
                options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{(e.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor).ActionName}");

                // configure version 1
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = configuration["App:Title"],
                    Version = configuration["App:Version"],
                    Description = configuration["App:Description"]
                });

                // delete 'version' param
                options.OperationFilter<RemoveVersionParameterFilter>();
                // add version to path
                options.DocumentFilter<SetVersionInPathFilter>();

                options.CustomSchemaIds(x => x.FullName);

                // select first operation if exists multiples
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                options.IncludeXmlComments(Path.ChangeExtension(typeof(Startup).Assembly.Location, "xml"));
            });

        }

        /// <summary>
        /// Extension method for add Automapper services
        /// </summary>
        /// <param name="services">Services container collection</param>
        /// <returns>Services container collection</returns>
        private static IServiceCollection AddAutomapper(this IServiceCollection services)
        {
            // Mappers
            services.AddAutoMapper(new[]
            {
                Assembly.GetAssembly(typeof(Startup))
                // ... incluir mas ensamblados
            });

            return services;
        }
    }
}
