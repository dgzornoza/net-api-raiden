using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Polly.Retry;
using WebApplication1.Infrastructure.Domain;

namespace WebApplication1.Api.Infrastructure.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost host)
            where TContext : IEfUnitOfWork
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var pollyPolicies = services.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation($"Migrando base de datos asociada con el contexto {typeof(TContext).Name}");

                    pollyPolicies.Get<RetryPolicy>(WebApplication1.Infrastructure.Constants.Polly.WaitAndRetry)
                        .Execute(() =>
                        {
                            (context as DbContext) !.Database.Migrate();
                        });

                    logger.LogInformation($"Base de datos asociada con el contexto {typeof(TContext).Name}, ha sido migrada");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Un error ha ocurrido mientras se migra la base de datos usada en el contexto {typeof(TContext).Name}");
                }
            }

            return host;
        }
    }
}
