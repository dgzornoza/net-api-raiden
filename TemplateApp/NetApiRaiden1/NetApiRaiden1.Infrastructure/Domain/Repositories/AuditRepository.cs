using NetApiRaiden1.Domain.AuditAggregate;

namespace NetApiRaiden1.Infrastructure.Domain.Repositories;
public class AuditRepository : Repository<Audit>, IAuditRepository
{
    public AuditRepository(IEfUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}
