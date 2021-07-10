using System;
using WebApplication1.Domain.SeedWork;

namespace WebApplication1.Domain.SharedKernel
{
    /// <summary>
    /// Class with the definition of base rule to validate the uniqueness of values ​​in entities
    /// </summary>
    /// <typeparam name="T">Tipo del valor a validar</typeparam>
    public abstract class ValueMustBeUniqueRule<T> : IBusinessRule
    {
        public ValueMustBeUniqueRule(IValueUniquenessChecker<T> valueUniquenessChecker, T value)
        {
            this.ValueUniquenessChecker = valueUniquenessChecker ?? throw new ArgumentNullException(nameof(valueUniquenessChecker));
            this.Value = value;
        }

        public abstract string Message { get; }

        protected T Value { get; set; }

        protected IValueUniquenessChecker<T> ValueUniquenessChecker { get; set; }

        public virtual bool IsBroken() => !this.ValueUniquenessChecker.IsUnique(this.Value);
    }
}
