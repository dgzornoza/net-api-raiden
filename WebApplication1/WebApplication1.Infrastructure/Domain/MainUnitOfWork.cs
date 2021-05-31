using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Infrastructure.Domain
{
    public class MainUnitOfWork : DbContext, IEfUnitOfWork
    {

        public MainUnitOfWork(DbContextOptions<MainUnitOfWork> options)
            : base(options)
        {

        }



        public DbSet<TEntity> GetSet<TEntity>()
            where TEntity : class
        {
            return this.Set<TEntity>();
        }


        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current)
            where TEntity : class
        {
            this.Entry(original).CurrentValues.SetValues(current);
        }

        public void SetModified<TEntity>(TEntity item)
            where TEntity : class
        {
            this.Entry(item).State = EntityState.Modified;
        }


        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            int result;

            using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
            result = await this.SaveChangesAsync(cancellationToken);

            scope.Complete();

            return result;
        }

        public void RollbackChanges()
        {
            this.ChangeTracker.Entries()
                        .ToList()
                        .ForEach(entry => entry.State = EntityState.Unchanged);
        }


    }
}
