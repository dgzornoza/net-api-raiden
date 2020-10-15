using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace WebApplication1.Infrastructure.Filters.Swagger
{
    /// <summary>
    /// Filter to set Version in path for swagger api
    /// </summary>
    public class SetVersionInPathFilter : IDocumentFilter
    {
        /// <summary>
        /// Function for apply filter
        /// </summary>
        /// <param name="swaggerDoc">Open api document</param>
        /// <param name="context">Document filter context</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc == null) throw new ArgumentNullException(nameof(swaggerDoc));

            var replacements = new OpenApiPaths();

            foreach (var (key, value) in swaggerDoc.Paths)
            {
                replacements.Add(key.Replace("{version}", swaggerDoc.Info.Version, StringComparison.InvariantCulture), value);
            }

            swaggerDoc.Paths = replacements;
        }
    }
}
