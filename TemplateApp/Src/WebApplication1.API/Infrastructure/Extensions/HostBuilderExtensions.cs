using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Registry;
using Polly.Retry;
using WebApplication1.Api.Settings;
using WebApplication1.Domain.SeedData.IdentityServer;
using WebApplication1.Infrastructure.Domain;

namespace WebApplication1.Api.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static WebApplication BuildContext(this WebApplication host)
        {
            host.MigrateDbContext<IEfUnitOfWork>((context, services) => { })
                /* $identityserver_feature$ start */
                .MigrateDbContext<PersistedGrantDbContext>((context, services) => { })
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var appConfigurationSettings = services.GetRequiredService<IOptions<AppConfigurationSettings>>().Value;

                    if (appConfigurationSettings.ExecuteIdentityServerSeedData)
                    {
                        services.GetRequiredService<ConfigurationDbSeedData>().SeedData().Wait();
                    }
                });
                /* $identityserver_feature$ end */

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

                    pollyPolicies.Get<RetryPolicy>(WebApplication1.Infrastructure.Constants.Polly.WaitAndRetry)
                        .Execute(() =>
                        {
                            (context as DbContext) !.Database.Migrate();
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
}
