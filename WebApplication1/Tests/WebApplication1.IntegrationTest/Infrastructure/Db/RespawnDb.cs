using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Respawn;
using WebApplication1.IntegrationTest.Infrastructure.Settings;

namespace WebApplication1.IntegrationTest.Infrastructure.Db
{
    public class RespawnDb
    {
        private static readonly Checkpoint Checkpoint = new ()
        {
            // Ignore tables from reset DDBB
            TablesToIgnore = new[]
            {
                "__EFMigrationsHistory",
            },
        };

        private readonly string connectionString;

        public RespawnDb(IConfiguration configuration)
        {
            this.connectionString = configuration[AppSettingsKeys.AppConnectionString];
        }

        public async Task Reset()
        {
            await Checkpoint.Reset(this.connectionString);
        }
    }
}
