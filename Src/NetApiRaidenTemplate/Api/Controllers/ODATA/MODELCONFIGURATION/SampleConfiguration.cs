using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.OData.ModelBuilder;
using $ext_safeprojectname$.Application.Queries.Samples.QueryableSamples;

namespace $safeprojectname$.Controllers.OData.ModelConfiguration;

public class SampleConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        switch (apiVersion.MajorVersion)
        {
            case 1:
                ConfigureV1(builder);
                break;
            default:
                ConfigureCurrent(builder);
                break;
        }
    }

    private static EntityTypeConfiguration<QueryableSamplesItemDto> ConfigureCurrent(ODataModelBuilder builder) =>
        builder.EntitySet<QueryableSamplesItemDto>("Samples").EntityType;

    private void ConfigureV1(ODataModelBuilder builder)
    {
        // sample ignore properties for this version
        // ConfigureCurrent(builder).Ignore(p => p.Email).Ignore(p => p.Phone);
        ConfigureCurrent(builder);
    }
}
