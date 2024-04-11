using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Infrastructure.Extensions;

namespace WebApplication1.Infrastructure.Domain
{
    /// <summary>
    /// App Entity Framework unit of work
    /// </summary>
    public class AppUnitOfWork : DbContext, IEfUnitOfWork
    {
        private readonly IMediator mediator;

        public AppUnitOfWork(DbContextOptions<AppUnitOfWork> options, IMediator mediator)
            : base(options)
        {
            this.mediator = mediator;
        }

        public DbSet<TEntity> CreateSet<TEntity>()
            where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public void ApplyCurrentValues<TEntity>(TEntity original, object current)
            where TEntity : class
        {
            this.Entry(original).CurrentValues.SetValues(current);
        }

        public void SetModified<TEntity>(TEntity entity)
            where TEntity : class
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            int result;
            using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

            // execute domain events before commit changes
            await this.mediator.DispatchDomainEventsAsync(this);

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
