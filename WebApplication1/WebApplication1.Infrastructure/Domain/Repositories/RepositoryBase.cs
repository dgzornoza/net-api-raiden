
using WebApplication1.Domain.SeedWork;

namespace WebApplication1.Infrastructure.Domain.Repositories
{
    public abstract class RepositoryBase : IRepository
    {
        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; private set; }
    }
}
