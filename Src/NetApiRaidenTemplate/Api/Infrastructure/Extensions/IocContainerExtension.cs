﻿using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using $safeprojectname$.Settings;
using $ext_safeprojectname$.Application.Behaviors;
using $ext_safeprojectname$.Application.Common.Commands;
using $ext_safeprojectname$.Domain.SeedData.IdentityServer;
using $ext_safeprojectname$.Infrastructure.Domain;

namespace $safeprojectname$.Infrastructure.Extensions
{
    /// <summary>
    /// Extension class for manage Application Inversion Of Control container
    /// </summary>
    public static class IocContainerExtension
    {
        /// <summary>
        /// Extension method for manage Application Inversion Of Control container
        /// </summary>
        /// <param name="services">Services container collection</param>
        /// <param name="configuration">App configuration</param>
        /// <returns>Services container collection object</returns>
        public static IServiceCollection AddIocContainer(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<IEfUnitOfWork, AppUnitOfWork>(options => options.UseSqlServer(configuration.GetConnectionString(AppSettingsKeys.AppConnectionString),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppUnitOfWork).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }));

            /* $identityserver_feature$ start */
            // IdentityServer
            services.AddTransient<ConfigurationDbSeedData>();
            /* $identityserver_feature$ end */

            // MediatR
            services.AddMediatR(typeof(ICommand).GetTypeInfo().Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Validators
            services.AddValidatorsFromAssembly(typeof(ICommand).GetTypeInfo().Assembly);

            // Repositories
            // ...

            // API services
            // ...

            // Configurations
            services.AddOptions<AppConfigurationSettings>().Bind(configuration.GetSection(AppSettingsKeys.AppConfiguration));
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(AppSettingsKeys.Jwt));

            return services;
        }
    }
}
