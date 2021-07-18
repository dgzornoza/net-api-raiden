using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Domain.SeedWork;

namespace $safeprojectname$.Domain.EntityConfigurations
{
    /// <summary>
    /// Entity configuration base for configure entities
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    internal abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : Entity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
