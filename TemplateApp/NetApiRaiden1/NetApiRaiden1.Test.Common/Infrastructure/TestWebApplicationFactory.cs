using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetApiRaiden1.Infrastructure.Domain;
using NetApiRaiden1.Test.Common.Handlers;
using NetApiRaiden1.Test.Common.Models;

namespace NetApiRaiden1.Test.Common.Infrastructure;

public class TestWebApplicationFactory<TProgram, TTestDbContext> : WebApplicationFactory<TProgram>
    where TProgram : class
    where TTestDbContext : DbContext, IEfUnitOfWork
{
    public static readonly string ConnectionString = "Data Source=TestDb.db";

    public HttpClient CreateAuthClient(AuthModel? authModel = null)
    {
        var client = CreateClient(ClientOptions);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
        if (authModel?.UserName != null)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.UserName, authModel.UserName);
        }
        if (authModel?.UserRole != null)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.UserRole, authModel.UserRole);
        }

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // eliminar el esquema de autenticacion por defecto para añadir el handler de autenticacion de test
            TestWebApplicationFactory<TProgram, TTestDbContext>.RemoveDefaultAuthenticationScheme(services);
            services.Configure<TestAuthHandlerOptions>(options =>
                {
                    options.DefaultUserName = "DefaultUser";
                    options.DefaultUserRole = "DefaultRole";
                });

            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
        });

        builder.ConfigureServices(services =>
        {
            // eliminar todos los DbContexts de la aplicacion para añadir el DbContext de test
            RemoveAllDbContextsFromServices(services);

            services.AddDbContext<IEfUnitOfWork, TTestDbContext>((container, options) =>
            {
                var projectAssemblyName = typeof(TTestDbContext).Assembly.GetName().Name;
                options.UseSqlite(ConnectionString, x => x.MigrationsAssembly(projectAssemblyName));
            });

            MigrateDbContext<IEfUnitOfWork>(services);
        });

        builder.UseEnvironment("Development");
    }

    private static void RemoveDefaultAuthenticationScheme(IServiceCollection services)
    {
        var descriptors = services.Where(d =>
            d.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>)).ToList();

        descriptors.ForEach(d => services.Remove(d));
    }

    private void RemoveAllDbContextsFromServices(IServiceCollection services)
    {
        // reverse operation of AddDbContext<XDbContext> which removes DbContexts from services
        var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
        descriptors.ForEach(d => services.Remove(d));

        descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContext)).ToList();
        descriptors.ForEach(d => services.Remove(d));

        descriptors = services.Where(d => d.ServiceType.GetInterface(nameof(IEfUnitOfWork)) != null).ToList();
        descriptors.ForEach(d => services.Remove(d));

        descriptors = services.Where(d => d.ServiceType.Name == nameof(IEfUnitOfWork)).ToList();
        descriptors.ForEach(d => services.Remove(d));
    }

    public void MigrateDbContext<TContext>(IServiceCollection serviceCollection) where TContext : IEfUnitOfWork
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetService<TContext>();

        try
        {
            var dbContext = context as DbContext;
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            if (dbContext!.Database.IsSqlServer())
            {
                throw new Exception("Use Sqlite instead of sql server!");
            }

            dbContext.Database.EnsureDeleted();

            dbContext.Database.Migrate();

            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            throw;
        }
    }
}
