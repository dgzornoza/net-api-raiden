using $safeprojectname$.Infrastructure.Authorization;
using $safeprojectname$.Infrastructure.Extensions;

namespace $safeprojectname$.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddApiVersioning()
            .AddCors()
            .AddSwagger<Program>()
            .AddPollyPolicies()
            .AddAuthentication(configuration)
            .AddAuthorization(Policies.AddPolicies);
    }
}
