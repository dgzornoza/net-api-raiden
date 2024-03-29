﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Polly.Retry;
// $identityserver_feature$ using $ext_safeprojectname$.Domain.SeedData.IdentityServer;
// $identityserver_feature$ using IdentityServer4.EntityFramework.DbContexts;
// $identityserver_feature$ using Microsoft.Extensions.Options;
// $identityserver_feature$ using $safeprojectname$.Settings;
using $ext_safeprojectname$.Infrastructure.Domain;

namespace $safeprojectname$.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static WebApplication BuildContext(this WebApplication host)
        {
            host.MigrateDbContext<IEfUnitOfWork>((context, services) => { });
            /* $identityserver_feature$ start */
            host.MigrateDbContext<PersistedGrantDbContext>((context, services) => { })
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

                    pollyPolicies.Get<RetryPolicy>($ext_safeprojectname$.Infrastructure.Constants.Polly.WaitAndRetry)
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
