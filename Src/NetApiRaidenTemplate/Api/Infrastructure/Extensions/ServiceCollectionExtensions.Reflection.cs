using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace $safeprojectname$.Infrastructure.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static void RegisterImplementationsToTypesClosing(this IServiceCollection services, Type openRequestInterface, Assembly assemblyToScan) =>
        services.RegisterImplementationsToTypesClosing(openRequestInterface, new[] { assemblyToScan });

    public static void RegisterImplementationsToTypesClosing(this IServiceCollection services, Type openRequestInterface, IEnumerable<Assembly> assembliesToScan)
    {
        var concretions = new List<Type>();
        var interfaces = new List<Type>();

        foreach (var type in assembliesToScan.SelectMany(item => item.DefinedTypes).Where(item => !item.IsOpenGeneric()).Cast<Type>())
        {
            if (type.GetInterface(openRequestInterface.FullName ?? string.Empty) == null) continue;

            var lastInterface = type.GetInterfaces().OrderByDescending(item => item.GetInterfaces().Length).FirstOrDefault();
            if (lastInterface == null) continue;

            if (type.IsConcrete())
            {
                concretions.Add(type);
            }

            if (!interfaces.Contains(lastInterface))
            {
                interfaces.Add(lastInterface);
            }
        }

        foreach (var @interface in interfaces)
        {
            var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
            if (exactMatches.Count > 1)
            {
                exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
            }

            foreach (var type in exactMatches)
            {
                services.TryAddTransient(@interface, type);
            }

            if (!@interface.IsOpenGeneric())
            {
                AddConcretionsThatCouldBeClosed(@interface, concretions, services);
            }
        }
    }

    private static bool IsOpenGeneric(this Type type) => type.IsGenericTypeDefinition || type.ContainsGenericParameters;

    private static bool IsConcrete(this Type type) => !type.IsAbstract && !type.IsInterface;

    private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
    {
        if (pluggedType == null) return false;

        if (pluggedType == pluginType) return true;

        return pluginType.IsAssignableFrom(pluggedType);
    }

    private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
    {
        var openInterface = closedInterface.GetGenericTypeDefinition();
        var arguments = closedInterface.GenericTypeArguments;

        var concreteArguments = openConcretion.GenericTypeArguments;
        return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
    }

    private static bool IsMatchingWithInterface(Type? handlerType, Type handlerInterface)
    {
        if (handlerType == null || handlerInterface == null)
        {
            return false;
        }

        if (handlerType.IsInterface)
        {
            if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
            {
                return true;
            }
        }
        else
        {
            return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
        }

        return false;
    }

    private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
    {
        foreach (var type in concretions.Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
        {
            try
            {
                services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

}
