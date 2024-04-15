using $safeprojectname$.SeedWork;

namespace $safeprojectname$.AuditAggregate;

public class Audit : Entity, IAggregateRoot
{
    public Audit(Guid userId, AuditType type, string detail, string operation, string clientIp)
    {
        UserId = userId;
        Type = type;
        Detail = detail;
        Operation = operation;
        OperationDate = DateTime.UtcNow;
        ClientIp = clientIp;
    }

    public Guid UserId { get; private set; }

    public AuditType Type { get; private set; }

    public string Detail { get; private set; }

    public string Operation { get; private set; }

    public DateTime OperationDate { get; private set; }

    public string ClientIp { get; private set; }
}
