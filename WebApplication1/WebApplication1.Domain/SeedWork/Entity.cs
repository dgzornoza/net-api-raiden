using System;

namespace WebApplication1.Domain.SeedWork
{
    /// <summary>
    /// Clase base para entidades.
    /// </summary>
    public abstract class Entity
    {
        public virtual Guid Id { get; protected set; }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }
    }
}
