using $safeprojectname$.Infrastructure.Commands;
using $safeprojectname$.Services.Audit;

namespace $safeprojectname$.Commands.Audits;

public class CreateAuditCommand : ICommand
{
    public IEnumerable<AuditDto> Audits { get; init; } = default!;
}
