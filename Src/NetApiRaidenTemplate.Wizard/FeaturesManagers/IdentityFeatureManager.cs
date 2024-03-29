﻿using System.Collections.Generic;
using EnvDTE;

namespace NetApiRaidenTemplate.Wizard.FeaturesManagers
{
    public class IdentityFeatureManager : FeatureManagerBase
    {
        public IdentityFeatureManager(Solution solution) : base(solution)
        {
        }

        protected override string FeatureName { get; } = "identityserver_feature";

        protected override IEnumerable<string> ExclusiveFeatureFiles { get; } = new List<string>()
        {
            @"Api\Controllers\AuthenticateController_id4.cs",
            @"Api\Infrastructure\Filters\SwaggerAuthorizeOperationFilter_id4.cs",
            @"Domain.SeedData\IdentityServer\ConfigurationDbSeedData.cs",
            @"Domain.SeedData\IdentityServer\IdentityConfiguration.cs",
        };

        protected override IDictionary<string, string> FeatureFileNamesMappers { get; } = new Dictionary<string, string>
        {
            {@"Api\Controllers\AuthenticateController_id4.cs", @"Api\Controllers\AuthenticateController.cs"},
            {@"Api\Infrastructure\Filters\SwaggerAuthorizeOperationFilter_id4.cs", @"Api\Infrastructure\Filters\SwaggerAuthorizeOperationFilter.cs"},
        };
    }
}
