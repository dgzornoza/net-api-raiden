using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using $ext_safeprojectname$.Domain.SharedKernel;

namespace $safeprojectname$.Domain.EntityConfigurations;

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
