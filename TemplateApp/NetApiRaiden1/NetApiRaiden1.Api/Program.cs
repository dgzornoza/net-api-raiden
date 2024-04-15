using System.Reflection;
using System.Text.Json;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OData;
using NetApiRaiden1.Api.Infrastructure.Extensions;
using NetApiRaiden1.Api.Infrastructure.Filters;
using Serilog;

namespace NetApiRaiden1.Api;

public partial class Program
{
    private static void Main(string[] args)
    {
        // HACK: Solo crear log estatico si se ejecuta este ensamblado como principal, en los tests no se puede crear por que da error.
        if (Assembly.GetEntryAssembly()!.FullName == typeof(Program).GetTypeInfo().Assembly.FullName)
        {
            Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
        }

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
        }).AddOData((options, provider) =>
        {
            options.Select().Filter().OrderBy().SetMaxTop(50).Count();
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

        // this method get all api descriptions and inject 'IApiVersionDescriptionProvider' for call all OData 'IModelConfiguration'
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseAppConfiguration(builder.Environment, apiVersionDescriptionProvider);

        app.UseAuthentication();
        app.UseAuthorization();

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
