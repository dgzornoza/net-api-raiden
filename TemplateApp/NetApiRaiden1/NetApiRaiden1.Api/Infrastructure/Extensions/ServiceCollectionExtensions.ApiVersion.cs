using Asp.Versioning;

namespace NetApiRaiden1.Api.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        var apiVersioningBuilder = services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1, 0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        });


        apiVersioningBuilder.AddOData(options =>
        {
            // version by query string, header, or media type
            options.AddRouteComponents("odata");
        });

        // version format "'v'major[.minor][-status]"
        string versionFormat = "'v'VVV";
        apiVersioningBuilder.AddODataApiExplorer(options =>
        {
            options.GroupNameFormat = versionFormat;
        }).AddApiExplorer(options =>
        {
            // version format "'v'major[.minor][-status]"
            options.GroupNameFormat = versionFormat;
        });

        return services;
    }
}
