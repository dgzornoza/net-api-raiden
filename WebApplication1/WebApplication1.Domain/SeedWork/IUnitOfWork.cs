using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        void RollbackChanges();
    }
}
