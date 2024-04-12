using $safeprojectname$.SeedWork;

namespace $safeprojectname$.SharedKernel.Rules;

/// <summary>
/// Class with the definition of base rule to validate the uniqueness of values ​​in entities
/// </summary>
/// <typeparam name="T">Tipo del valor a validar</typeparam>
public abstract class ValueMustBeUniqueRule<T> : IBusinessRule
{
    protected ValueMustBeUniqueRule(IValueUniquenessChecker<T> valueUniquenessChecker, T value)
    {
        ValueUniquenessChecker = valueUniquenessChecker ?? throw new ArgumentNullException(nameof(valueUniquenessChecker));
        Value = value;
    }

    public abstract string Message { get; }

    protected T Value { get; set; }

    protected IValueUniquenessChecker<T> ValueUniquenessChecker { get; set; }

    public virtual bool IsBroken() => !ValueUniquenessChecker.IsUnique(Value);
}
