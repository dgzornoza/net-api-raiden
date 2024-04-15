namespace $safeprojectname$.Infrastructure.Settings;

public record CorsSettings
{
    public IEnumerable<string> AllowedOrigins { get; init; } = default!;
}
