using NetApiRaiden1.Application.Infrastructure.Commands;
using NetApiRaiden1.Application.Services.Audit;

namespace NetApiRaiden1.Application.Commands.Audits;

public class CreateAuditCommand : ICommand
{
    public IEnumerable<AuditDto> Audits { get; init; } = default!;
}
