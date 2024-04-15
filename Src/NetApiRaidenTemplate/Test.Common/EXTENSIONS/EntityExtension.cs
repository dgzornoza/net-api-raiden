using $ext_safeprojectname$.Domain.SeedWork;
using System.Collections;
using System.Reflection;

namespace $safeprojectname$.Extensions;

public static class EntityExtension
{
    public static IEnumerable<IDomainEvent> GetAllDomainEvents(this Entity aggregate)
    {
        var domainEvents = aggregate.DomainEvents != null ?
            new List<IDomainEvent>(aggregate.DomainEvents) :
            new List<IDomainEvent>();

        // get all navigation properties for get all events
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        IEnumerable<FieldInfo> fields = aggregate.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        if (aggregate.GetType().BaseType is not null)
        {
            fields = fields.Concat(aggregate.GetType().BaseType!.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public));
        }
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        // add recursive domain events in all navigation properties
        foreach (var field in fields)
        {
            domainEvents.AddRange(TryGetAllDomainEventsFromNavigationProperty(field, aggregate));
        }

        return domainEvents;
    }

    private static IEnumerable<IDomainEvent> TryGetAllDomainEventsFromNavigationProperty(FieldInfo fieldInfo, Entity aggregate)
    {
        var result = new List<IDomainEvent>();

        if (fieldInfo.FieldType.IsAssignableFrom(typeof(Entity)))
        {
            var entity = fieldInfo.GetValue(aggregate) as Entity;
            if (entity is not null)
            {
                result.AddRange(entity.GetAllDomainEvents().ToList());
            }
        }
        else if (fieldInfo.FieldType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.GetValue(aggregate) is IEnumerable enumerable)
        {
            foreach (var item in enumerable.OfType<Entity>().Where(item => item != null))
            {
                result.AddRange(item.GetAllDomainEvents().ToList());
            }
        }

        return result;
    }
}
