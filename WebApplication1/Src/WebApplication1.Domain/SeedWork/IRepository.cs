using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplication1.Domain.SeedWork
{
    /// <summary>
    /// Repository Interface
    /// </summary>
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }

    /// <summary>
    /// Repository specific entity type interface
    /// </summary>
    /// <typeparam name="TEntity">Entity to use by repository</typeparam>
    public interface IRepository<TEntity> : IRepository
        where TEntity : IAggregateRoot
    {
        /// <summary>
        /// Count entities
        /// </summary>
        /// <returns>Total entities</returns>
        Task<int> Count();

        /// <summary>
        /// Count entities with filter
        /// </summary>
        /// <param name="filter">filter for counting</param>
        /// <returns>Total entites</returns>
        Task<int> Count(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>Entities list</returns>
        Task<IEnumerable<TEntity>> GetAll();

        /// <summary>
        /// Function to check if elements exist using an input filter
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <returns>True if exist elements for query, false otherwise</returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Function to check if elements exist using an input filter and related entities
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>True if exist elements for query, false otherwise</returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> filter, params string[] includes);

        /// <summary>
        /// Function to get entities by input filter
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <returns>Entities list</returns>
        Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Function to get custom mapped entities by input filter
        /// </summary>
        /// <typeparam name="TResult">Result mapped model type</typeparam>
        /// <param name="filter">Query input filter</param>
        /// <param name="mapperExpression">Mapper expression for select return model</param>
        /// <returns>Entities of type Tresult</returns>
        Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> mapperExpression);

        /// <summary>
        /// Function to get entities by input filter and related entities
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>Entities list</returns>
        Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter, params string[] includes);

        /// <summary>
        /// Function to get custom mapped entities by input filter and related entities
        /// </summary>
        /// <typeparam name="TResult">Tipo del modelo de retorno</typeparam>
        /// <param name="filter">Query input filter</param>
        /// <param name="mapperExpression">Mapper expression for select return model</param>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>Entities of type Tresult</returns>
        Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> mapperExpression, params string[] includes);

        /// <summary>
        /// Function to get the first entity by input filter
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <returns>Found entity, null otherwise</returns>
        Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Function to get the first entity by input filter and related entities with dbcontext tracking entity option
        /// </summary>
        /// <param name="filter">Query input filter</param>
        /// <param name="tracking">true to track entity by context</param>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>Found entity, null otherwise</returns>
        Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> filter, bool tracking, params string[] includes);

        /// <summary>
        /// Remove entity from repository
        /// </summary>
        /// <param name="entity">Entity to remove from repository</param>
        /// <remarks>
        /// This method does not have to remove entity from the data store, it only removes from the local repository, to remove from the data store, a UnitOfWork commit must be made
        /// </remarks>
        void Remove(TEntity entity);

        /// <summary>
        /// Add or update entity in repository
        /// </summary>
        /// <param name="entity">Entity to add or update on repository</param>
        /// <returns>Modelo entidades añadido o eliminado en el respositorio</returns>
        /// <remarks>
        /// This method does not have to add or update entity on data store, it only add or update on local repository, to add or update on data store, a UnitOfWork commit must be made
        /// </remarks>
        TEntity AddOrUpdate(TEntity entity);

        /// <summary>
        /// Add entity in repository
        /// </summary>
        /// <param name="entity">Entity to add on repository</param>
        /// <returns>Modelo entidades añadido en el respositorio</returns>
        /// <remarks>
        /// This method does not have to add entity on data store, it only add on local repository, to add on data store, a UnitOfWork commit must be made
        /// </remarks>
        TEntity Add(TEntity entity);
    }
}
