using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            // Connection String
            // services.AddDbContext<Data.Interfaces.IUnitOfWorkPrincipal, Data.Repository.UnitOfWorkPrincipal>(options => options.UseSqlServer(configuration.GetConnectionString("AppSqlServerConnection")));

            // Mappers
            ////services.AddAutoMapper(Assembly.GetAssembly(typeof(Application.Mappers.MappingProfile)));

            // Services
            ////services.AddScoped<ILanguageService, LanguageService>();

            // Repositories
            ////services.AddScoped<ILanguageRepository, LanguageRepository>();

            return services;
        }
    }
}
