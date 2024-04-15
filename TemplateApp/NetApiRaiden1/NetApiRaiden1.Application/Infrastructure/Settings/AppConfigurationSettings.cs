﻿namespace NetApiRaiden1.Application.Infrastructure.Settings;

public record AppConfigurationSettings()
{
    public CorsSettings Cors { get; set; } = default!;

    public bool ExecuteIdentityServerSeedData { get; set; }
}

