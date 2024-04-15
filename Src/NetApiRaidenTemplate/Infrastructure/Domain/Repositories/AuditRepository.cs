using $ext_safeprojectname$.Domain.AuditAggregate;

namespace $safeprojectname$.Domain.Repositories;
public class AuditRepository : Repository<Audit>, IAuditRepository
{
    public AuditRepository(IEfUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}
