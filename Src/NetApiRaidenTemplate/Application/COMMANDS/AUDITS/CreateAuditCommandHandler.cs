using Microsoft.Extensions.Logging;
using $safeprojectname$.Infrastructure.Commands;
using $ext_safeprojectname$.Domain.AuditAggregate;

namespace $safeprojectname$.Commands.Audits;

public class CreateAuditCommandHandler : ICommandHandler<CreateAuditCommand>
{
    private readonly ILogger<CreateAuditCommandHandler> logger;
    private readonly IAuditRepository auditRepository;

    public CreateAuditCommandHandler(ILogger<CreateAuditCommandHandler> logger, IAuditRepository auditRepository)
    {
        this.logger = logger;
        this.auditRepository = auditRepository;
    }

    public async Task Handle(CreateAuditCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Audit> auditorias = request.Audits.Select(item => new Audit(
            item.UserId!.Value,
            (AuditType)item.AuditType!.Value,
            item.Detail,
            item.Operation,
            item.ClientIp));

        foreach (var auditoria in auditorias)
        {
            auditRepository.Add(auditoria);
            logger.LogInformation("UserId: {userId}, Type: {type}, Operation: {operation}, clientIp: {ip}, Detail: {detail}", 
                auditoria.UserId, auditoria.Type, auditoria.Operation, auditoria.ClientIp, auditoria.Detail);
        }

        try
        {
            await auditRepository.UnitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{name} Error", typeof(CreateAuditCommandHandler).Name);
        }
    }
}
