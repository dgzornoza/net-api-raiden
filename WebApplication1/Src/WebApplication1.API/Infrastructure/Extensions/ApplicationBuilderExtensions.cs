using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;

namespace WebApplication1.API.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppConfiguration(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.AddLocalization();

            if (env.IsDevelopment())
            {
                app.AddSwagger(provider);
            }

            app.UseCors("CorsPolicy");

            return app;
        }

        private static void AddLocalization(this IApplicationBuilder app)
        {
            // middleware para gestionar idiomas
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("es"),
                new CultureInfo("en"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            });
        }

        private static void AddSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    // generar endpoints swagger para todas las versiones disponibles
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }
    }
}
