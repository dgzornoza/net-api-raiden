using System;

namespace WebApplication1.Domain.SeedWork
{
    public class BusinessRuleValidationException : Exception
    {
        public BusinessRuleValidationException(IBusinessRule brokenRule)
            : base(brokenRule.Message)
        {
            BrokenRule = brokenRule;
        }

        public IBusinessRule BrokenRule { get; }

        public override string ToString()
        {
            return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
        }
    }
}
