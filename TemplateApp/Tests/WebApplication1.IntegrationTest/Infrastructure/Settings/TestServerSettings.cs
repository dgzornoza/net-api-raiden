namespace WebApplication1.IntegrationTest.Infrastructure.Settings
{
    public record TestServerSettings
    {
        public string Environment { get; init; } = default!;

        public string ApiUrl { get; init; } = default!;
    }
}
