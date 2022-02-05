using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Infrastructure.Domain;
using WebApplication1.IntegrationTest.Infrastructure.Db;
using WebApplication1.IntegrationTest.Infrastructure.Settings;

namespace WebApplication1.IntegrationTest
{
    public class TestServer : IDisposable
    {
        private static readonly object LockMigration = new ();
        private static bool isMigratedDb = false;

        private readonly Microsoft.AspNetCore.TestHost.TestServer server;
        private bool disposedValue = false;

        public TestServer()
        {
            var testServerSettings = Configuration.GetValue<TestServerSettings>(AppSettingsKeys.TestServer);

            // inicializar respawn de BBDD
            this.DbRespawn = new RespawnDb(Configuration);

            // inicializar servidor web de test
            var builder = new WebHostBuilder()
                .UseEnvironment(testServerSettings.Environment)
                .UseStartup<TestStartup>()
                .UseConfiguration(Configuration);

            this.server = new Microsoft.AspNetCore.TestHost.TestServer(builder)
            {
                BaseAddress = new Uri(testServerSettings.ApiUrl),
            };
            this.Client = this.server.CreateClient();

            // contexto de BBDD
            this.UnitOfWork = (AppUnitOfWork)this.server.Host.Services.GetRequiredService<IEfUnitOfWork>();
            if (!isMigratedDb)
            {
                lock (LockMigration)
                {
                    this.UnitOfWork.Database.Migrate();
                    isMigratedDb = true;
                }
            }

            // Incializar infrastructura para las pruebas
            Task.Run(() => SeedDb.CreateTestDbSeed(this.server.Host.Services)).Wait();
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.testing.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

        public AppUnitOfWork UnitOfWork { get; }

        public HttpClient Client { get; }

        public RespawnDb DbRespawn { get; }

        public IServiceProvider Services => this.server.Host.Services;

        public void Dispose()
        {
            // No modificar este codigo. Poner el codigo de limpieza en Dispose(bool disposing).
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Client.Dispose();
                    this.server.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
