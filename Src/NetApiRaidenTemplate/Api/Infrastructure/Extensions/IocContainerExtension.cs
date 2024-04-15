using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using $ext_safeprojectname$.Application.Behaviors;
using $ext_safeprojectname$.Application.Infrastructure.Commands;
using $ext_safeprojectname$.Application.Infrastructure.Settings;
using $ext_safeprojectname$.Application.Services.Audit;
using $ext_safeprojectname$.Domain.SeedWork;
using $ext_safeprojectname$.Infrastructure.Domain;
using $ext_safeprojectname$.Infrastructure.Domain.Repositories;

namespace $safeprojectname$.Infrastructure.Extensions;

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
