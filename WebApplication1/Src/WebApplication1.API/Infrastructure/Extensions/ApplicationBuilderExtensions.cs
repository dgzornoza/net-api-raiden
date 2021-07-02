﻿using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Hosting;

namespace WebApplication1.API.Infrastructure.Extensions
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
        /// <param name="env">webhost environment</param>
        /// <returns>application builder object</returns>
        public static IApplicationBuilder UseAppConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
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

            // Swagger
            if (env.IsDevelopment())
            {
                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    });
            }

            // CORS
            app.UseCors("CorsPolicy");

            return app;
        }
    }
}
