using System.Reflection;
using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetApiRaiden1.Domain.SharedKernel;
using NetApiRaiden1.Infrastructure.Extensions;

namespace NetApiRaiden1.Infrastructure.Domain;


/// <summary>
/// App Entity Framework unit of work
/// </summary>
public abstract class EfUnitOfWork<TEntityConfiguration> : DbContext, IEfUnitOfWork
    where TEntityConfiguration : class
{
    private readonly IMediator mediator;

    /// <summary>
    /// Empty ctor required for generating migrations from dotnet tool
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected EfUnitOfWork()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public EfUnitOfWork(DbContextOptions options, IMediator mediator)
        : base(options)
    {
        this.mediator = mediator;
    }

    protected abstract bool UseTransactions { get; }

    public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class =>
        Set<TEntity>();

    public void ApplyCurrentValues<TEntity>(TEntity original, object current) where TEntity : class =>
        Entry(original).CurrentValues.SetValues(current);

    public void Dettach<TEntity>(TEntity entity) where TEntity : class =>
        Entry(entity).State = EntityState.Detached;

    public void SetModified<TEntity>(TEntity entity) where TEntity : class =>
        Entry(entity).State = EntityState.Modified;

    public virtual async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        int result;
        if (UseTransactions)
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

            // execute domain events before commit changes
            await mediator.DispatchDomainEventsAsync(this);
            result = await SaveChangesAsync(cancellationToken);
            scope.Complete();
        }
        else
        {
            // execute domain events before commit changes
            await mediator.DispatchDomainEventsAsync(this);
            result = await SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateAuditableEntities();

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditableEntities();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public void RollbackChanges()
    {
        this.ChangeTracker.Entries()
                    .ToList()
                    .ForEach(entry => entry.State = EntityState.Unchanged);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TEntityConfiguration).GetTypeInfo().Assembly);
    }

    private void UpdateAuditableEntities()
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.Version = Guid.NewGuid();
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.Version = Guid.NewGuid();
                    break;
            }
        }
    }
}
