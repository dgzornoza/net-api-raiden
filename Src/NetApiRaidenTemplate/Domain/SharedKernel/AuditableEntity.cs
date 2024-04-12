
using $safeprojectname$.SeedWork;

namespace $safeprojectname$.SharedKernel;

public class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid Version { get; set; } = default!;
}
