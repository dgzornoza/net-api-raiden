using NetApiRaiden1.Api.Infrastructure.Authorization;
using NetApiRaiden1.Api.Infrastructure.Extensions;

namespace NetApiRaiden1.Api.Infrastructure.Extensions;

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
