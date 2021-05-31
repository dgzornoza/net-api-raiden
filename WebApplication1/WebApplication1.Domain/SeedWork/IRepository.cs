using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplication1.Domain.SeedWork
{
    /// <summary>
    /// Interface con la declaración de un repositorio.
    /// </summary>
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }

    }

    /// <summary>
    /// Interface con la declaración de un repositorio para una entidad especifica
    /// </summary>
    /// <typeparam name="TEntity">Entidad a usar por el repositorio</typeparam>
    public interface IRepository<TEntity> : IRepository
        where TEntity : IAggregateRoot
    {
        /// <summary>
        /// Función para contar las entidades
        /// </summary>
        /// <returns>número total de entidades</returns>
        Task<int> Count();

        /// <summary>
        /// Función para contar las entidades
        /// </summary>
        /// <param name="predicate">filtro de entrada para la consulta</param>
        /// <returns>número total de entidades</returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Función para obtener todas las entidades
        /// </summary>
        /// <returns>Lista con todas las entidades</returns>
        Task<IEnumerable<TEntity>> GetAll();

        /// <summary>
        /// Función para verificar si existen elementos mediante un filtro de entrada
        /// </summary>
        /// <param name="predicate">filtro de entrada para la consulta</param>
        /// <returns>True si existen elementos, false en caso contrario</returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Función para verificar si existen elementos mediante un filtro de entrada
        /// </summary>
        /// <param name="predicate">filtro de entrada para la consulta</param>
        /// <param name="includes">Propiedades de navegación a incluir</param>
        /// <returns>True si existen elementos, false en caso contrario</returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> predicate, params string[] includes);

        /// <summary>
        /// Función para obtener una lista de entidades mediante un filtro de entrada
        /// </summary>
        /// <param name="predicate">Filtro de entrada para obtener las entidades</param>
        /// <returns>Lista de entidades</returns>
        Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Función para obtener una lista de entidades personalizadas mediante un filtro de entrada
        /// </summary>
        /// <typeparam name="TResult">Tipo del modelo de retorno</typeparam>
        /// <param name="predicate">Filtro de entrada para obtener las entidades</param>
        /// <param name="selectExpression">expresión para establecer el modelo de retorno</param>
        /// <returns>lista de entidades dadas por <see cref="selectExpression"/></returns>
        Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectExpression);

        /// <summary>
        /// Función para obtener una lista de entidades mediante un filtro de entrada
        /// </summary>
        /// <param name="predicate">Filtro de entrada para obtener las entidades</param>
        /// <param name="includes">Propiedades de navegación a incluir</param>
        /// <returns>Lista de entidades</returns>
        Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> predicate, params string[] includes);

        /// <summary>
        /// Función para obtener una lista de entidades personalizadas mediante un filtro de entrada
        /// </summary>
        /// <typeparam name="TResult">Tipo del modelo de retorno</typeparam>
        /// <param name="predicate">Filtro de entrada para obtener las entidades</param>
        /// <param name="selectExpression">expresión para establecer el modelo de retorno</param>
        /// <param name="includes">Propiedades denavegación a incluir</param>
        /// <returns>lista de entidades dadas por <see cref="selectExpression"/></returns>
        Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectExpression, params string[] includes);


        /// <summary>
        /// Función para obtener el primer elemento encontrado mediante el filtro de entrada
        /// </summary>
        /// <param name="predicate">Filtro para obtener el primer elemento</param>
        /// <returns>Elemento encontrado, null en caso de no encontrar ninguno</returns>
        Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Función para obtener el primer elemento encontrado mediante el filtro de entrada con posibilidad de incluir propiedades de navegacion
        /// </summary>
        /// <param name="predicate">Filtro para obtener el primer elemento</param>
        /// <param name="tracking">Realiza el seguimiento de la entidad en el contexto del respositorio</param>
        /// <param name="includes">Propiedades de navegacion a incluir</param>
        /// <returns>Elemento encontrado, null en caso de no encontrar ninguno</returns>
        Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> predicate, bool tracking, params string[] includes);

        /// <summary>
        /// Función para eliminar una entidad del repositorio
        /// </summary>
        /// <param name="item">Entidad a eliminar</param>
        /// <remarks>
        /// Este metodo no tiene que eliminar la entidad del almacen de datos, solo lo elimina del repositorio local, para eliminarlo del almacen de datos, deberá realizarse un commit del UnitOfWork
        /// </remarks>
        void Remove(TEntity item);

        /// <summary>
        /// Función para añadir o actualizar un modelo de entidad
        /// </summary>
        /// <param name="edm">Modelo de entidad a añadir o modificar</param>
        /// <returns>Modelo entidades añadido o eliminado en el respositorio</returns>
        /// <remarks>
        /// Esto metodo no tiene que actualiza/eliminar la entidad en el almacen de datos, solo lo realiza en el repositorio local, para actualizar/eliminar del almacen de datos, deberá realizarse un commit del UnitOfWork
        /// </remarks>
        TEntity AddOrUpdate(TEntity edm);

        /// <summary>
        /// Función para añadir un modelo de entidad
        /// </summary>
        /// <param name="edm">Modelo de entidad a añadir</param>
        /// <returns>Modelo entidades añadido en el respositorio</returns>
        /// <remarks>
        /// Este metodo no tiene que añadir la entidad en el almacen de datos, solo lo realiza en el repositorio local, para añadirlo en el almacen de datos, deberá realizarse un comit del UnitOfWork
        /// </remarks>
        TEntity Add(TEntity edm);
    }
}
