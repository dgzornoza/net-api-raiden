using System;
using WebApplication1.Domain.SharedKernel;

namespace WebApplication1.Test.Helpers.Mocks
{
    public class MockValueChecker<T> : IValueUniquenessChecker<T>
    {
        private readonly Func<T, bool> checkFunction;

        public MockValueChecker(Func<T, bool> checkFunction) => this.checkFunction = checkFunction;

        public bool IsUnique(T value)
        {
            return this.checkFunction(value);
        }
    }
}
