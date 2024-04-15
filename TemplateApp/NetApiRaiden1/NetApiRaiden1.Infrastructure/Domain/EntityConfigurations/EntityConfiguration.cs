using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetApiRaiden1.Domain.SeedWork;

namespace NetApiRaiden1.Infrastructure.Domain.EntityConfigurations;


/// <summary>
/// Entity configuration base for configure entities
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).ValueGeneratedOnAdd();
    }
}
