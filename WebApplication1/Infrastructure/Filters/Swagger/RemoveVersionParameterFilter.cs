using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace WebApplication1.Infrastructure.Filters.Swagger
{
    /// <summary>
    /// Filter for remove version parameter in swagger
    /// </summary>
    public class RemoveVersionParameterFilter : IOperationFilter
    {
        /// <summary>
        /// Function for apply filter
        /// </summary>
        /// <param name="operation">Open api operation</param>
        /// <param name="context">Operation filter context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameterVersion = operation?.Parameters?.SingleOrDefault(param => param.Name.Equals("version"));
            operation?.Parameters?.Remove(parameterVersion);
        }
    }
}
