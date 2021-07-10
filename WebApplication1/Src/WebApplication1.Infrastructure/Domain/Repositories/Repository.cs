using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.SeedWork;

namespace WebApplication1.Infrastructure.Domain.Repositories
{
    /// <summary>
    /// Repository base class
    /// </summary>
    /// <typeparam name="TEntity">Repository entity type</typeparam>
    public abstract class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        private readonly IEfUnitOfWork unitOfWork;

        protected Repository(IEfUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork => this.unitOfWork;

        protected DbSet<TEntity> DbSet => this.unitOfWork.CreateSet<TEntity>();

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await this.DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> filter)
        {
            return await this.DbSet.AsNoTracking().AnyAsync(filter);
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> filter, params string[] includes)
        {
            var queryable = this.DbSet.AsNoTracking();
            foreach (var include in includes)
            {
                queryable = queryable.Include(include);
            }

            return await queryable.AnyAsync(filter);
        }

        public virtual async Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter)
        {
            return await this.DbSet.AsNoTracking().Where(filter).ToListAsync();
        }

        public virtual async Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> mapperExpression)
        {
            IEnumerable<TResult> query = await this.DbSet.AsNoTracking()
                .Where(filter)
                .Select(mapperExpression)
                .ToListAsync();

            return query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter, params string[] includes)
        {
            var queryable = this.DbSet.AsNoTracking();
            foreach (var include in includes)
            {
                queryable = queryable.Include(include);
            }

            IEnumerable<TEntity> result = await queryable.Where(filter).ToListAsync();
            return result;
        }

        public virtual async Task<IEnumerable<TResult>> GetFiltered<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> mapperExpression, params string[] includes)
        {
            var queryable = this.DbSet.AsNoTracking();
            foreach (var include in includes)
            {
                queryable = queryable.Include(include);
            }

            IEnumerable<TResult> result = await queryable
                .Where(filter)
                .Select(mapperExpression)
                .ToListAsync();

            return result;
        }

        public virtual async Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> filter)
        {
            var element = await this.DbSet.FirstOrDefaultAsync(filter);
            return element;
        }

        public virtual async Task<TEntity> GetFirstElementFiltered(Expression<Func<TEntity, bool>> filter, bool tracking, params string[] includes)
        {
            var queryable = this.DbSet.AsQueryable();
            if (!tracking)
            {
                queryable = queryable.AsNoTracking();
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
            }

            var element = await queryable.FirstOrDefaultAsync(filter);
            return element;
        }

        public virtual async Task<int> Count()
        {
            var count = await this.DbSet.CountAsync();
            return count;
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> filter)
        {
            var count = await this.DbSet.CountAsync(filter);
            return count;
        }

        public virtual void Remove(TEntity entity)
        {
            if (entity != null)
            {
                this.unitOfWork.Attach(entity);
                this.DbSet.Remove(entity);
            }
        }

        public virtual TEntity Add(TEntity entity)
        {
            this.DbSet.Add(entity);
            return entity;
        }

        public virtual TEntity AddOrUpdate(TEntity entity)
        {
            this.DbSet.Update(entity);
            return entity;
        }

        protected DbSet<T> CreateDbSet<T>()
            where T : class, IAggregateRoot
        {
            return this.unitOfWork.CreateSet<T>();
        }
    }
}
