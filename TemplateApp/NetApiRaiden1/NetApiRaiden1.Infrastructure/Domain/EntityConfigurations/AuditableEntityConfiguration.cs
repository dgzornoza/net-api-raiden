using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NetApiRaiden1.Domain.SharedKernel;

namespace NetApiRaiden1.Infrastructure.Domain.EntityConfigurations;
/// <summary>
/// Entity configuration base for configure audatble entities
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class AuditableEntityConfiguration<TEntity> : EntityConfiguration<TEntity>, IEntityTypeConfiguration<TEntity>
    where TEntity : AuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(item => item.Version).IsConcurrencyToken();
    }
}
