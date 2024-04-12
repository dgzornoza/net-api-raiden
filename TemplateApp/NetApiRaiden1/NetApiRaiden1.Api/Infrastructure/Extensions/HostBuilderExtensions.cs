using Microsoft.EntityFrameworkCore;
using NetApiRaiden1.Infrastructure.Domain;
using Polly.Registry;
using Polly.Retry;

namespace NetApiRaiden1.Api.Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static WebApplication BuildContext(this WebApplication host)
    {
        host.MigrateDbContext<IEfUnitOfWork>((context, services) => 
        {
            // TODO: Seed Data
        });

        return host;
    }

    private static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider>? postMigrateAction = null)
        where TContext : notnull
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var pollyPolicies = services.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
            var context = services.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation($"Migrating Database from context {typeof(TContext).Name}");

                pollyPolicies.Get<RetryPolicy>(NetApiRaiden1.Infrastructure.Polly.WaitAndRetry)
                    .Execute(() =>
                    {
                        (context as DbContext)!.Database.Migrate();
                        postMigrateAction?.Invoke(context, services);
                    });

                logger.LogInformation($"Migrated Database from context {typeof(TContext).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An Error ocurred on migrate database from context {typeof(TContext).Name}");
            }
        }

        return host;
    }
}
