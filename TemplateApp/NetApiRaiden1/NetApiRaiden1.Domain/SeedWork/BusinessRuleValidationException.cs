namespace NetApiRaiden1.Domain.SeedWork;

/// <summary>
/// Exception class for domain business rules
/// </summary>
public class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }

    /// <summary>
    /// Get business broken rule
    /// </summary>
    public IBusinessRule BrokenRule { get; }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
    }
}
