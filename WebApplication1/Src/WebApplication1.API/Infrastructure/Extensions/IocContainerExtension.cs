using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.API.Settings;
using WebApplication1.Application.Behaviors;
using WebApplication1.Application.Common.Commands;

namespace WebApplication1.API.Infrastructure.Extensions
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
            // Misc
            // services.AddDbContext<IEfUnitOfWork, AppUnitOfWork>(options => options.UseSqlServer(configuration.GetConnectionString("AppConnectionString")));

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
