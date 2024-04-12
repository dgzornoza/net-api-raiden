namespace $safeprojectname$.SeedWork;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        TriggeredOn = DateTime.Now;
    }

    /// <summary>
    /// Date and time on event ocurred
    /// </summary>
    public DateTime TriggeredOn { get; }
}
