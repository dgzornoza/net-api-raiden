using System.Reflection;

namespace $safeprojectname$.SeedWork;

/// <summary>
/// Enumeration class as substitute language enums
/// </summary>
public abstract class Enumeration : IComparable
{
    private readonly int value;
    private readonly string displayName;

    protected Enumeration()
    {
        displayName = string.Empty;
    }

    protected Enumeration(int value, string displayName)
    {
        this.value = value;
        this.displayName = displayName;
    }

    public int Value
    {
        get { return value; }
    }

    public string DisplayName
    {
        get { return displayName; }
    }

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration, new()
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            var instance = new T();
            if (info.GetValue(instance) is T locatedValue)
            {
                yield return locatedValue;
            }
        }
    }

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value)
        where T : Enumeration, new()
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Value == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName)
        where T : Enumeration, new()
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
        return matchingItem;
    }

    public override string ToString()
    {
        return DisplayName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = value.Equals(otherValue.Value);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public int CompareTo(object? obj)
    {
        return Value.CompareTo(((Enumeration?)obj)?.Value);
    }

    private static T Parse<T, TValue>(TValue value, string description, Func<T, bool> predicate)
        where T : Enumeration, new()
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem is null)
        {
            var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
            throw new InvalidCastException(message);
        }

        return matchingItem;
    }
}
