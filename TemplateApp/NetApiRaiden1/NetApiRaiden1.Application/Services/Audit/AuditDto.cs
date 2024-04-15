namespace NetApiRaiden1.Application.Services.Audit;

public record AuditDto
{
    public Guid? UserId { get; init; } = default!;

    public string Detail { get; init; } = default!;

    public string Operation { get; init; } = default!;

    public string ClientIp { get; init; } = default!;

    public AuditTypeDto? AuditType { get; init; } = default!;
}
