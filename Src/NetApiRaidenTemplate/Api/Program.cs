using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using $safeprojectname$.Infrastructure.Filters;
using $safeprojectname$.Infrastructure.Extensions;
using $ext_safeprojectname$.Infrastructure.Domain;

namespace $safeprojectname$;

public static class Program
{

    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);

        // Serilog
        builder.Host.UseSerilog((context, logConfiguration) => logConfiguration.ReadFrom.Configuration(context.Configuration));

        // Add services to the container.
        builder.Services.AddControllers(configure =>
        {
            configure.Filters.Add(typeof(HttpGlobalExceptionFilter));
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddIocContainer(builder.Configuration);
        builder.Services.AddAppConfiguration(builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();

        // APP Builder
        var app = builder
            .Build()
            .BuildContext();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseAppConfiguration(builder.Environment, provider);

        /* $identityserver_feature$ start */
        app.UseAuthentication();
        app.UseAuthorization();
        /* $identityserver_feature$ end */

        app.MapControllers();

        try
        {
            Log.Information("Getting the motors running...");

            // Run the Host, and start accepting requests
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

