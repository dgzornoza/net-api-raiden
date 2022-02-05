using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using $ext_safeprojectname$.Domain.SeedWork;

namespace $safeprojectname$.Extensions
{
    /// <summary>
    /// Class with MediatR extension methods.
    /// </summary>
    internal static class MediatorExtensions
    {
        /// <summary>
        /// Function to execute all events registered in the domain currently
        /// </summary>
        /// <param name="mediator">MediatR mediator object</param>
        /// <param name="dbContext">Database DbContext</param>
        /// <returns>Execution task</returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext dbContext)
        {
            var domainEntities = dbContext.ChangeTracker
                .Entries<Entity>()
                .Where(item => item.Entity.DomainEvents != null && item.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(item => item.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(item => item.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
