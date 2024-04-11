using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace $safeprojectname$.Infrastructure.Extensions
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
            // middleware for manage languages
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
                    // generate swagger endpoints for all versions
                    foreach (var groupName in provider.ApiVersionDescriptions.Select(item => item.GroupName))
                    {
                        options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
                    }

                    /* $identityserver_feature$ start */
                    options.OAuthClientId("swagger.client");
                    options.OAuthAppName("API - Swagger");
                    options.OAuthScopeSeparator(" ");
                    options.OAuthUsePkce();
                    /* $identityserver_feature$ end */
                });
        }
    }
}
