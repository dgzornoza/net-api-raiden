using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace WebApplication1.Infrastructure.Extensions
{
    /// <summary>
    /// Application builder extension builder with custom app configuration
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Custom App configuration
        /// </summary>
        /// <param name="app">application builder</param>
        /// <returns>application builder</returns>
        public static IApplicationBuilder UseAppConfiguration(this IApplicationBuilder app)
        {
            // middleware for manage languages
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("es"),
                new CultureInfo("en")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            // Swagger
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                });

            // CORS
            app.UseCors("CorsPolicy");


            return app;
        }
    }
}
