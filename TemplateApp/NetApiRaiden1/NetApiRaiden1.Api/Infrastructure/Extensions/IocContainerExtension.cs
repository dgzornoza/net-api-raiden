using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetApiRaiden1.Application.Behaviors;
using NetApiRaiden1.Application.Infrastructure.Commands;
using NetApiRaiden1.Application.Infrastructure.Settings;
using NetApiRaiden1.Application.Services.Audit;
using NetApiRaiden1.Domain.SeedWork;
using NetApiRaiden1.Infrastructure.Domain;
using NetApiRaiden1.Infrastructure.Domain.Repositories;

namespace NetApiRaiden1.Api.Infrastructure.Extensions;

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
        services.AddDbContext<IEfUnitOfWork, AppUnitOfWork>(options => 
            options.UseSqlServer(configuration.GetConnectionString(AppSettingsKeys.AppConnectionString),
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppUnitOfWork).GetTypeInfo().Assembly.GetName().Name);
            }));

        // Seeding
        // TODO: Add seeding

        // HttpContext
        services.AddHttpContextAccessor();

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ICommand).GetTypeInfo().Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Validators
        services.AddValidatorsFromAssembly(typeof(ICommand).GetTypeInfo().Assembly);

        // Infraestructura transversal
        services.AddSingleton<IAuditBufferService, AuditBufferService>();

        // Repositories
        services.RegisterImplementationsToTypesClosing(typeof(IRepository<>),
            [typeof(AuditRepository).GetTypeInfo().Assembly]);

        // API services
        // ...

        // Configurations
        services.AddOptions<AppConfigurationSettings>().Bind(configuration.GetSection(AppSettingsKeys.AppConfiguration));

        return services;
    }
}
