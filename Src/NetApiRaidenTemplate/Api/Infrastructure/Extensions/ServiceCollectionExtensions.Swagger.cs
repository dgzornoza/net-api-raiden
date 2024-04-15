using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using $safeprojectname$.Infrastructure.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace $safeprojectname$.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger<TProgram>(this IServiceCollection services)
    {
        var assemblyProductAttribute = typeof(TProgram).Assembly.GetCustomAttribute<AssemblyProductAttribute>() ??
            throw new ApplicationException(Properties.Resources.InvalidAssemblyProductAttribute);

        var assemblyDescriptionAttribute = typeof(TProgram).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>() ??
            throw new ApplicationException(Properties.Resources.InvalidAssemblyDescriptionAttribute);

        // generate swagger versions doc from controllers versions
        services.AddOptions<SwaggerGenOptions>()
            .Configure<IApiVersionDescriptionProvider>((options, provider) =>
            {
                // generate swagger versions doc from controllers
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                    {
                        Title = $"{assemblyProductAttribute.Product} " + $"{description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ? $"{assemblyDescriptionAttribute.Description} - DEPRECATED" : assemblyDescriptionAttribute.Description,
                    });
                }
            });

        return services.AddSwaggerGen(options =>
        {
            // filter to set default version used for swagger doc
            options.OperationFilter<SwaggerApiVersionFilter>();

            // show full namespace in schemes section
            options.CustomSchemaIds(item => item.FullName);

            // select first endpoint in case of multiple
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            options.IncludeXmlComments(Path.ChangeExtension(typeof(TProgram).Assembly.Location, "xml"));
        });
    }
}
