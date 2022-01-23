using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Api.Infrastructure.Authorization;
using WebApplication1.Api.Settings;
using WebApplication1.Application.Behaviors;
using WebApplication1.Application.Common.Commands;
using WebApplication1.Infrastructure.Domain;

namespace WebApplication1.Api.Infrastructure.Extensions
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
            var connectionString = configuration.GetConnectionString("AppConnectionString");
            services.AddDbContext<IEfUnitOfWork, AppUnitOfWork>(options => options.UseSqlServer(configuration.GetConnectionString("AppConnectionString")));

            /* $identityserver_feature$ start */
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
            services.AddOptions<AppConfigurationSettings>().Bind(configuration.GetSection("AppConfiguration"));

            return services;
        }
    }
}
