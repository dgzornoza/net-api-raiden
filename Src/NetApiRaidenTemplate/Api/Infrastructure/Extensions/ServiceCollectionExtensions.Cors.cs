using $safeprojectname$.Infrastructure.Extensions;
using $ext_safeprojectname$.Application.Infrastructure.Settings;

namespace $safeprojectname$.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfigurationSettings = configuration.GetObject<AppConfigurationSettings>(AppSettingsKeys.AppConfiguration);

        return services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(appConfigurationSettings.Cors.AllowedOrigins.ToArray())
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowCredentials());
        });
    }
}
