using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApplication1.Domain.SeedWork;

namespace $safeprojectname$.Domain
{
    /// <summary>
    /// Entity Framework Unit Of Work interface
    /// </summary>
    public interface IEfUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Create a Microsoft.EntityFrameworkCore.DbSet that can be used to query and save TEntity instances
        /// </summary>
        /// <typeparam name="TEntity">DbSet entity type</typeparam>
        /// <returns>Entity DbSet</returns>
        DbSet<TEntity> CreateSet<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Starts tracking of specified entity by the context and all related navigation entities found.
        /// <see cref="DbContext.Attach"/>
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity to attach to context</param>
        /// <returns>EntityEntry that provides access to information on tracking changes and operations for the entity.</returns>
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Sets the state of the specified entity as modified.
        /// This method establishes only the modified state in the specified entity and not in the rest of the entities, also not in related entities.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity to set as modified</param>
        void SetModified<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Sets the values ​​by copying the current object to the original.
        /// The current object can be of any type. Any property on the current object that
        /// matches the original object in the entity will be read and copied. The rest of the properties will be
        /// ignored. This allows, for example, to copy properties from DTOs
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="original">Current entity to which the matching properties will be copied</param>
        /// <param name="current">Object whose matching properties will be copied to the entity</param>
        void ApplyCurrentValues<TEntity>(TEntity original, object current)
            where TEntity : class;
    }
}
