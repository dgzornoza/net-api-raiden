using System;
using MediatR;

namespace $safeprojectname$.SeedWork
{
    /// <summary>
    /// Domain events interface
    /// </summary>
    public interface IDomainEvent : INotification
    {
        DateTime TriggeredOn { get; }
    }
}
