using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplication1.Api.Infrastructure.Filters
{
    public class SwaggerApiVersionFilter : IOperationFilter
    {
        private const string ApiVersionKey = "x-api-version";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiVersionParameter = operation.Parameters.SingleOrDefault(p => p.Name == ApiVersionKey);
            if (apiVersionParameter != null)
            {
                // Get attribute [ApiVersion("VV")]
                var attribute = context?.MethodInfo?.DeclaringType?
                  .GetCustomAttributes(typeof(ApiVersionAttribute), false)
                  .Cast<ApiVersionAttribute>()
                  .SingleOrDefault();

                // Extract version value and set in parameter
                var version = attribute?.Versions?.SingleOrDefault()?.ToString();
                if (version != null)
                {
                    apiVersionParameter.Example = new OpenApiString(version);
                    apiVersionParameter.Schema.Example = new OpenApiString(version);
                }
            }
        }
    }
}
