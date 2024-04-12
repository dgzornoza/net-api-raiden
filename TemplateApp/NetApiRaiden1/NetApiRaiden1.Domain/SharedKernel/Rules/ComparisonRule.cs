using NetApiRaiden1.Domain.SeedWork;

namespace NetApiRaiden1.Domain.SharedKernel.Rules;

/// <summary>
/// Clase con la definición de regla base para validar la comparación de valores en entidades
/// </summary>
/// <typeparam name="T">Tipo del valor a validar</typeparam>
public class ComparisonRule<T> : IBusinessRule where T : IComparable<T>, IEquatable<T>
{
    public ComparisonRule(ComparisonOperator comparisonOperator, T sourceValue, T targetValue, string errorMessage)
    {
        ComparisonOperator = comparisonOperator;
        SourceValue = sourceValue;
        TargetValue = targetValue;
        Message = errorMessage;
    }

    public virtual string Message { get; }

    public virtual bool IsBroken() => ComparisonOperator switch
    {
        ComparisonOperator.Equal => !SourceValue.Equals(TargetValue),
        ComparisonOperator.NotEqual => SourceValue.Equals(TargetValue),
        ComparisonOperator.GreaterThan => SourceValue.CompareTo(TargetValue) <= 0,
        ComparisonOperator.GreaterThanOrEqual => SourceValue.CompareTo(TargetValue) < 0,
        ComparisonOperator.LessThan => SourceValue.CompareTo(TargetValue) >= 0,
        ComparisonOperator.LessThanOrEqual => SourceValue.CompareTo(TargetValue) > 0,
        _ => throw new NotImplementedException(),
    };

    protected T SourceValue { get; set; }
    protected T TargetValue { get; set; }

    protected ComparisonOperator ComparisonOperator { get; set; }
}
