namespace NetApiRaiden1.Api.Settings;

public record CorsSettings
{
    public IEnumerable<string> AllowedOrigins { get; init; } = default!;
}
