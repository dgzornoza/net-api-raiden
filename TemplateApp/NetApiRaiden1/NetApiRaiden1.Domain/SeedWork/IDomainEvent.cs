using MediatR;

namespace NetApiRaiden1.Domain.SeedWork;

/// <summary>
/// Domain events interface
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime TriggeredOn { get; }
}
