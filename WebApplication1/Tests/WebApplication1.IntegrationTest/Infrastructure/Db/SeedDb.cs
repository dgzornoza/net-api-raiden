using System;
using System.Threading.Tasks;

namespace WebApplication1.IntegrationTest.Infrastructure.Db
{
    public static class SeedDb
    {
        public static async Task CreateTestDbSeed(IServiceProvider serviceProvider)
        {
            try
            {
                // TODO: implement seed functions
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test {typeof(SeedDb)} Error: {ex.Message}");
            }
        }
    }
}
