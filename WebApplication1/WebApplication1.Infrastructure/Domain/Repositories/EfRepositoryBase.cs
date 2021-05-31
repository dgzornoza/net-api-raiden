using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication1.Domain.SeedWork;

namespace WebApplication1.Infrastructure.Domain.Repositories
{

    public abstract class EfRepositoryBase<TEntity> : RepositoryBase, IRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        protected EfRepositoryBase(IEfUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        protected DbSet<TEntity> DbSet => (UnitOfWork as IEfUnitOfWork).GetSet<TEntity>();



        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().AnyAsync(predicate);
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var queryable = DbSet.AsNoTracking();
            foreach (var include in includes) queryable = queryable.Include(include);

            return await queryable.AnyAsync(predicate);
        }

        public virtual async Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectExpression)
        {
            IEnumerable<TResult> query = await DbSet.AsNoTracking()
                .Where(predicate)
                .Select(selectExpression)
                .ToListAsync();

            return query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var queryable = DbSet.AsNoTracking();
            foreach (var include in includes) queryable = queryable.Include(include);

            IEnumerable<TEntity> result = await queryable.Where(predicate).ToListAsync();
            return result;
        }

        public virtual async Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selectExpression, params string[] includes)
        {
            var queryable = DbSet.AsNoTracking();
            foreach (var include in includes) queryable = queryable.Include(include);

            IEnumerable<TResult> result = await queryable
                .Where(predicate)
                .Select(selectExpression)
                .ToListAsync();

            return result;
        }

        public virtual async Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> predicate)
        {
            var element = await DbSet.FirstOrDefaultAsync(predicate);
            return element;
        }

        public virtual async Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> predicate, bool tracking, params string[] includes)
        {
            var queryable = DbSet.AsQueryable();
            if (!tracking) queryable = queryable.AsNoTracking();

            if (includes != null) foreach (var include in includes) queryable = queryable.Include(include);

            var element = await queryable.FirstOrDefaultAsync(predicate);
            return element;
        }


        public virtual IQueryable<TEntity> GetQueryable(params string[] includes)
        {
            var queryable = DbSet.AsNoTracking();
            foreach (var include in includes) queryable = queryable.Include(include);

            return queryable;
        }


        public virtual async Task<int> Count()
        {
            var set = DbSet;
            var count = await set.CountAsync();
            return count;
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            var set = DbSet;
            var count = await set.CountAsync(predicate);
            return count;
        }

        public virtual void Remove(TEntity item)
        {
            if (item != null)
            {
                (UnitOfWork as IEfUnitOfWork).Attach(item);
                DbSet.Remove(item);
            }
        }

        public virtual TEntity Add(TEntity edm)
        {
            DbSet.Add(edm);
            return edm;
        }

        public virtual TEntity AddOrUpdate(TEntity edm)
        {
            DbSet.Update(edm);
            return edm;
        }


        protected DbSet<T> GetDbSet<T>()
            where T : class, IAggregateRoot
        {
            return (UnitOfWork as IEfUnitOfWork).GetSet<T>();
        }
    }
}
