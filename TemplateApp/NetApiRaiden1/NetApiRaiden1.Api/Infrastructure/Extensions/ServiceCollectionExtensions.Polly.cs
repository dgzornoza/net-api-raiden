using Polly.Registry;
using Polly;

namespace NetApiRaiden1.Api.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddPollyPolicies(this IServiceCollection services)
    {
        PolicyRegistry registry = new PolicyRegistry
        {
            {
                NetApiRaiden1.Infrastructure.Polly.WaitAndRetry,
                Policy.Handle<HttpRequestException>().WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                    })
            },
        };

        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        return services;
    }
}
