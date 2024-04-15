using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using $ext_safeprojectname$.Domain.SeedWork;

namespace $safeprojectname$.Domain;

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
    /// Detach entity from tracking by the context.
    /// This method establishes only the dettached state in the specified entity and not in the rest of the entities, also not in related entities.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity to set as dettached</param>
    void Dettach<TEntity>(TEntity entity) where TEntity : class;

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

    /// <summary>
    /// Save dbcontext changes
    /// </summary>
    /// <param name="cancellationToken">
    /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
    /// </param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    /// A concurrency violation is encountered while saving to the database. A concurrency
    ///  violation occurs when an unexpected number of rows are affected during save.
    ///  This is usually because the data in the database has been modified since it was
    ///  loaded into memory.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    /// <remarks>
    /// This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
    ///  to discover any changes to entity instances before saving to the underlying database.
    ///  This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
    ///
    /// Entity Framework Core does not support multiple parallel operations being run
    ///  on the same DbContext instance. This includes both parallel execution of async
    ///  queries and any explicit concurrent use from multiple threads. Therefore, always
    ///  await async calls immediately, or use separate DbContext instances for operations
    ///  that execute in parallel. See Avoiding DbContext threading issues for more information
    ///  and examples.
    ///
    /// See Saving data in EF Core for more information and examples.    
    /// </remarks>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">
    /// Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
    /// is called after the changes have been sent successfully to the database.
    /// </param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// An error is encountered while saving to the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    /// A concurrency violation is encountered while saving to the database. A concurrency
    ///  violation occurs when an unexpected number of rows are affected during save.
    ///  This is usually because the data in the database has been modified since it was
    ///  loaded into memory.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    /// <remarks>
    /// This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
    ///  to discover any changes to entity instances before saving to the underlying database.
    ///  This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
    ///
    /// Entity Framework Core does not support multiple parallel operations being run
    ///  on the same DbContext instance. This includes both parallel execution of async
    ///  queries and any explicit concurrent use from multiple threads. Therefore, always
    ///  await async calls immediately, or use separate DbContext instances for operations
    ///  that execute in parallel. See Avoiding DbContext threading issues for more information
    ///  and examples.
    ///
    /// See Saving data in EF Core for more information and examples.    
    /// </remarks>
    int SaveChanges(bool acceptAllChangesOnSuccess);

}

