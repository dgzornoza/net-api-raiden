
using NetApiRaiden1.Domain.SeedWork;

namespace NetApiRaiden1.Domain.SharedKernel;

public class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid Version { get; set; } = default!;
}
