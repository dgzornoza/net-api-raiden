using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApplication1.Domain.SeedWork;

namespace WebApplication1.Infrastructure.Domain
{
    public interface IEfUnitOfWork : IUnitOfWork
    {
        DbSet<TEntity> GetSet<TEntity>()
            where TEntity : class;

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class;

        void SetModified<TEntity>(TEntity entity)
            where TEntity : class;

        void ApplyCurrentValues<TEntity>(TEntity original, TEntity current)
            where TEntity : class;
    }
}
