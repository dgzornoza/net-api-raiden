﻿
namespace $safeprojectname$.SeedWork;

/// <summary>
/// Base class for entities.
/// </summary>
public class Entity
{
    private readonly List<IDomainEvent> domainEvents = new();

    /// <summary>
    /// Get all entity domain events
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public virtual Guid Id { get; protected set; }

    /// <summary>
    /// Clear all entity domain events
    /// </summary>
    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    protected void AddDomainEvent(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);
}
