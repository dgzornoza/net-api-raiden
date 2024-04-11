namespace WebApplication1.Api.Settings
{
    public record JwtSettings
    {
        public string Authority { get; init; } = default!;

        public string ValidAudience { get; init; } = default!;

        public string ValidIssuer { get; init; } = default!;

        public string Secret { get; init; } = default!;
    }
}
