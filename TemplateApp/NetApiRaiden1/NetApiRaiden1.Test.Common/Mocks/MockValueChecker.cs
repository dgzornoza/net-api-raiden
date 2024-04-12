
using NetApiRaiden1.Domain.SharedKernel.Rules;

namespace NetApiRaiden1.Test.Common.Mocks;

public class MockValueChecker<T> : IValueUniquenessChecker<T>
{
    private readonly Func<T, bool> checkFunction;

    public MockValueChecker(Func<T, bool> checkFunction) => this.checkFunction = checkFunction;

    public bool IsUnique(T value)
    {
        return checkFunction(value);
    }
}
