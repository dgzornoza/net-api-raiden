using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApplication1.API
{
    /// <summary>
    /// Main entry program class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application configuration
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// App main entry point
        /// </summary>
        /// <param name="args">Application arguments</param>
        /// <returns>Task result</returns>
        public static async Task Main(string[] args)
        {
            // Initialize Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");
                IHost host = CreateHostBuilder(args).Build();

                // Run the Host, and start accepting requests
                await host.RunAsync();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Method for create application host builder
        /// </summary>
        /// <param name="args">Application arguments</param>
        /// <returns>Host builder created</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, config) =>
                {
                    config.ClearProviders();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseConfiguration(Configuration);
                    webBuilder.UseSerilog();
                });
    }
}
