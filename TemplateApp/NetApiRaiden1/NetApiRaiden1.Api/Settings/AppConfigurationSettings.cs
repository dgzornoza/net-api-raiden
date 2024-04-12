namespace NetApiRaiden1.Api.Settings;

public record AppConfigurationSettings()
{
    public CorsSettings Cors { get; set; } = default!;

    public bool ExecuteIdentityServerSeedData { get; set; }
}

